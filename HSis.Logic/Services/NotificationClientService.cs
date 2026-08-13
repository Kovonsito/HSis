using System.Collections.Concurrent;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HSis.Logic.Services
{
    public class NotificationClientService(
        IConfiguration configuration,
        ILogger<NotificationClientService> logger,
        INotificationEventBus? eventBus = null) : INotificationClientService
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<NotificationClientService> _logger = logger;
        private readonly INotificationEventBus? _eventBus = eventBus;
        private HubConnection? _connection;
        private bool _isConnecting = false;
        private readonly ConcurrentQueue<Func<HubConnection, Task>> _pendingNotifications = new();

        // Eventos expuestos para que la UI responda a los cambios (compatibilidad legacy)
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
                _eventBus?.PublicarEstadoConexion(false, "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar...");
                await IniciarReconexionBackgroundAsync();
            };

            _connection.Reconnecting += (error) =>
            {
                _logger.LogInformation(error, "SignalR intentando reconectar automáticamente...");
                OnReconnecting?.Invoke();
                _eventBus?.PublicarEstadoConexion(false, "⚠️ Intentando reconectar con el servidor de notificaciones...");
                return Task.CompletedTask;
            };

            _connection.Reconnected += (connectionId) =>
            {
                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("SignalR reconectado correctamente. ID: {ConnectionId}", connectionId);
                }
                OnConnected?.Invoke();
                OnReconnected?.Invoke(connectionId);
                _eventBus?.PublicarEstadoConexion(true);
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
                _eventBus?.PublicarNotificacion(ticketId, tipo, mensaje);
            });

            try
            {
                await _connection.StartAsync();
                _logger.LogInformation("SignalR conectado correctamente.");
                OnConnected?.Invoke();
                _eventBus?.PublicarEstadoConexion(true);
                _ = ProcesarNotificacionesPendientesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al iniciar conexión inicial de SignalR.");
                OnDisconnected?.Invoke();
                _eventBus?.PublicarEstadoConexion(false, "⚠️ Conectando al servidor de notificaciones...");
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
                    _eventBus?.PublicarEstadoConexion(false, "⚠️ Intentando reconectar con el servidor de notificaciones...");
                    try
                    {
                        await _connection.StartAsync();
                        _logger.LogInformation("Conexión de SignalR reestablecida en segundo plano.");
                        OnConnected?.Invoke();
                        _eventBus?.PublicarEstadoConexion(true);
                        _ = ProcesarNotificacionesPendientesAsync();
                        break;
                    }
                    catch
                    {
                        await Task.Delay(5000);
                    }
                }
                _isConnecting = false;
            });
        }

        private async Task ProcesarNotificacionesPendientesAsync()
        {
            while (_pendingNotifications.TryDequeue(out var action))
            {
                if (_connection?.State == HubConnectionState.Connected)
                {
                    try
                    {
                        await action(_connection);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error enviando notificación pendiente.");
                    }
                }
            }
        }

        public async Task DetenerAsync()
        {
            if (_connection != null)
            {
                await _connection.StopAsync();
                await _connection.DisposeAsync();
                _connection = null;
                OnDisconnected?.Invoke();
                _eventBus?.PublicarEstadoConexion(false, "Servidor de notificaciones detenido.");
            }
        }

        public async Task NotificarCambioTicketAsync(int ticketId, string tipoAccion, string mensaje)
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _connection.InvokeAsync("EnviarNotificacionTicket", ticketId, tipoAccion, mensaje);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al invocar EnviarNotificacionTicket.");
                    EncolarNotificacion(conn => conn.InvokeAsync("EnviarNotificacionTicket", ticketId, tipoAccion, mensaje));
                }
            }
            else
            {
                _logger.LogWarning("Conexión no activa. Encolando notificación.");
                EncolarNotificacion(conn => conn.InvokeAsync("EnviarNotificacionTicket", ticketId, tipoAccion, mensaje));
            }
        }

        public async Task NotificarCambioEstatusAsync(int ticketId, string nuevoEstatus)
        {
            if (_connection?.State == HubConnectionState.Connected)
            {
                try
                {
                    await _connection.InvokeAsync("EnviarCambioEstatus", ticketId, nuevoEstatus);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al invocar EnviarCambioEstatus.");
                    EncolarNotificacion(conn => conn.InvokeAsync("EnviarCambioEstatus", ticketId, nuevoEstatus));
                }
            }
            else
            {
                _logger.LogWarning("Conexión no activa. Encolando notificación.");
                EncolarNotificacion(conn => conn.InvokeAsync("EnviarCambioEstatus", ticketId, nuevoEstatus));
            }
        }

        public async Task NotificarTicketCreadoAsync(int ticketId, string ticketFolio, string titulo)
        {
            await NotificarCambioTicketAsync(ticketId, "Creado", $"Nuevo ticket #{ticketFolio}: {titulo}");
        }


        public async Task NotificarCambioEstatusTicketAsync(int clientUserId, int ticketId, string ticketFolio, string newStatus)
        {
            await NotificarCambioEstatusAsync(ticketId, newStatus);
        }


        public async Task NotificarCalificacionTicketAsync(int technicianUserId, int ticketId, string ticketFolio, int rating, string comment)
        {
            await NotificarCambioTicketAsync(ticketId, "Calificado", $"Ticket #{ticketFolio} calificado con {rating} estrellas: {comment}");
        }


        private void EncolarNotificacion(Func<HubConnection, Task> action)
        {
            _pendingNotifications.Enqueue(action);
        }

    }

}

