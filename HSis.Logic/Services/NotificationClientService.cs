using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;

namespace HSis.Logic.Services;

public class NotificationClientService(IConfiguration configuration, ILogger<NotificationClientService> logger) : INotificationClientService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<NotificationClientService> _logger = logger;
    private HubConnection? _connection;
    private bool _isConnecting = false;
    private readonly ConcurrentQueue<Func<HubConnection, Task>> _pendingNotifications = new();

    // Eventos expuestos para que la UI responda a los cambios
    public event Action<string, int, string>? OnNotificationReceived;
    public event Action? OnConnected;
    public event Action? OnDisconnected;
    public event Action? OnReconnecting;
    public event Action<string?>? OnReconnected;

    public bool IsConnected => _connection?.State == HubConnectionState.Connected;

    public async Task IniciarAsync(int userId, string role)
    {
        if (_connection != null)
        {
            await DetenerAsync();
        }

        var baseUrl = _configuration["SignalR:ServerUrl"] ?? "http://localhost:5000/notificationHub";
        var connectionUrl = $"{baseUrl}?userId={userId}&role={Uri.EscapeDataString(role)}";

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Iniciando conexión de SignalR a {Url}", baseUrl);
        }

        _connection = new HubConnectionBuilder()
            .WithUrl(connectionUrl)
            .WithAutomaticReconnect([
                TimeSpan.FromSeconds(0),
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)
            ])
            .Build();

        // Registrar escuchadores de eventos de la conexión
        _connection.Closed += async (error) =>
        {
            _logger.LogWarning(error, "Conexión de SignalR cerrada.");
            OnDisconnected?.Invoke();
            await IniciarReconexionBackgroundAsync();
        };

        _connection.Reconnecting += (error) =>
        {
            _logger.LogInformation(error, "SignalR intentando reconectar automáticamente...");
            OnReconnecting?.Invoke();
            return Task.CompletedTask;
        };

        _connection.Reconnected += (connectionId) =>
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("SignalR reconectado correctamente. ID: {ConnectionId}", connectionId);
            }
            OnReconnected?.Invoke(connectionId);
            _ = ProcesarNotificacionesPendientesAsync();
            return Task.CompletedTask;
        };

        // Escuchar la recepción de notificaciones generales enviadas por el Hub
        _connection.On<string, int, string>("ReceiveNotification", (tipo, ticketId, mensaje) =>
        {
            if (_logger.IsEnabled(LogLevel.Information))
            {
                _logger.LogInformation("Notificación recibida: Tipo={Tipo}, TicketId={TicketId}", tipo, ticketId);
            }
            OnNotificationReceived?.Invoke(tipo, ticketId, mensaje);
        });

        try
        {
            await _connection.StartAsync();
            _logger.LogInformation("SignalR conectado correctamente.");
            OnConnected?.Invoke();
            _ = ProcesarNotificacionesPendientesAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error al iniciar conexión inicial de SignalR.");
            OnDisconnected?.Invoke();
            // Iniciar ciclo de reintento en segundo plano para no bloquear el inicio de la app
            await IniciarReconexionBackgroundAsync();
        }
    }

    private async Task IniciarReconexionBackgroundAsync()
    {
        if (_isConnecting) return;
        _isConnecting = true;

        _ = Task.Run(async () =>
        {
            while (_connection != null && _connection.State == HubConnectionState.Disconnected)
            {
                _logger.LogInformation("Intentando conectar al Hub de SignalR en segundo plano...");
                OnReconnecting?.Invoke();
                try
                {
                    await _connection.StartAsync();
                    _logger.LogInformation("Conexión de SignalR reestablecida en segundo plano.");
                    OnConnected?.Invoke();
                    _ = ProcesarNotificacionesPendientesAsync();
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Fallo en reintento de conexión de SignalR: {Message}", ex.Message);
                    // Esperar antes del siguiente reintento
                    await Task.Delay(TimeSpan.FromSeconds(10));
                }
            }
            _isConnecting = false;
        });

        await Task.CompletedTask;
    }

    public async Task DetenerAsync()
    {
        if (_connection != null)
        {
            try
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al detener conexión de SignalR.");
            }
            finally
            {
                _connection = null;
                OnDisconnected?.Invoke();
            }
        }
    }

    private async Task ProcesarNotificacionesPendientesAsync()
    {
        if (_connection == null || !IsConnected) return;

        if (_logger.IsEnabled(LogLevel.Information))
        {
            _logger.LogInformation("Procesando {Count} notificaciones pendientes...", _pendingNotifications.Count);
        }

        while (_pendingNotifications.TryDequeue(out var notificationAction))
        {
            try
            {
                await notificationAction(_connection);
                _logger.LogInformation("Notificación pendiente enviada con éxito.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar notificación pendiente. Re-encolando.");
                _pendingNotifications.Enqueue(notificationAction);
                break; // Detener hasta la siguiente reconexión exitosa
            }
        }
    }

    // Métodos para enviar notificaciones a través del Hub
    public async Task NotifyTicketStatusChangedAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus)
    {
        Task action(HubConnection conn) => conn.InvokeAsync("NotifyTicketStatusChanged", clientUserId, ticketId, ticketFolio, newStatus);

        if (IsConnected && _connection != null)
        {
            try
            {
                await action(_connection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invocar NotifyTicketStatusChanged en el Hub. Guardando en cola pendiente.");
                _pendingNotifications.Enqueue(action);
            }
        }
        else
        {
            _logger.LogInformation("Conexión desconectada. Guardando notificación de cambio de estatus en cola pendiente.");
            _pendingNotifications.Enqueue(action);
        }
    }

    public async Task NotifyTicketRatedAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment)
    {
        Task action(HubConnection conn) => conn.InvokeAsync("NotifyTicketRated", technicianUserId, ticketId, ticketFolio, rating, comment);

        if (IsConnected && _connection != null)
        {
            try
            {
                await action(_connection);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al invocar NotifyTicketRated en el Hub. Guardando en cola pendiente.");
                _pendingNotifications.Enqueue(action);
            }
        }
        else
        {
            _logger.LogInformation("Conexión desconectada. Guardando notificación de calificación en cola pendiente.");
            _pendingNotifications.Enqueue(action);
        }
    }
}
