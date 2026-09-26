using HSis.Contracts.Services;
using HSis.Contracts.DTOs;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HSis.UI.Services
{
    public class ClienteSignalRNotificaciones(
        IConfiguration configuration,
        ILogger<ClienteSignalRNotificaciones> logger,
        IBusEventosNotificaciones? eventBus = null) : IClienteSignalRNotificaciones
    {
        private readonly IConfiguration _configuration = configuration;
        private readonly ILogger<ClienteSignalRNotificaciones> _logger = logger;
        private readonly IBusEventosNotificaciones? _eventBus = eventBus;
        private readonly SemaphoreSlim _lifecycleLock = new(1, 1);
        private HubConnection? _connection;
        private int _detenerSolicitado;

        // Eventos expuestos para que la UI responda a los cambios
        public event Action<NotificacionDto>? OnNotificationReceived;
        public event Action? OnConnected;
        public event Action? OnDisconnected;
        public event Action? OnReconnecting;
        public event Action<string?>? OnReconnected;

        public bool IsConnected => _connection?.State == HubConnectionState.Connected;

        public async Task IniciarAsync(string token)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(token);

            await _lifecycleLock.WaitAsync();
            try
            {
                Volatile.Write(ref _detenerSolicitado, 1);
                await DetenerInternoAsync(notificarDetencion: false);
                Volatile.Write(ref _detenerSolicitado, 0);

                var connectionUrl = _configuration["SignalR:ServerUrl"] ?? "http://localhost:5000/notificationHub";

                if (_logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("Iniciando conexión de SignalR a {Url}", connectionUrl);
                }

                var connection = CrearConexion(connectionUrl, token);
                _connection = connection;

                try
                {
                    await connection.StartAsync();
                    _logger.LogInformation("SignalR conectado correctamente.");
                    OnConnected?.Invoke();
                    _eventBus?.PublicarEstadoConexion(true);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al iniciar conexión inicial de SignalR.");
                    if (ReferenceEquals(_connection, connection))
                    {
                        _connection = null;
                    }

                    try
                    {
                        await connection.DisposeAsync();
                    }
                    catch (Exception disposeException)
                    {
                        _logger.LogWarning(disposeException, "No se pudo liberar la conexión inicial de SignalR.");
                    }

                    OnDisconnected?.Invoke();
                    _eventBus?.PublicarEstadoConexion(false, "⚠️ No se pudo conectar con el servidor de notificaciones.");
                }
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        private HubConnection CrearConexion(string connectionUrl, string token)
        {
            var connection = new HubConnectionBuilder()
                .WithUrl(connectionUrl, options =>
                {
                    options.AccessTokenProvider = () => Task.FromResult<string?>(token);
                })
                .WithAutomaticReconnect([
                    TimeSpan.FromSeconds(0),
                    TimeSpan.FromSeconds(2),
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(30)
                ])
                .Build();

            connection.Closed += error =>
            {
                if (Volatile.Read(ref _detenerSolicitado) != 0 || !ReferenceEquals(_connection, connection))
                {
                    return Task.CompletedTask;
                }

                _logger.LogWarning(error, "SignalR agotó los intentos automáticos de reconexión.");
                OnDisconnected?.Invoke();
                _eventBus?.PublicarEstadoConexion(false, "⚠️ No se pudo restablecer el servidor de notificaciones.");
                return Task.CompletedTask;
            };

            connection.Reconnecting += error =>
            {
                _logger.LogInformation(error, "SignalR intentando reconectar automáticamente...");
                OnReconnecting?.Invoke();
                _eventBus?.PublicarEstadoConexion(false, "⚠️ Intentando reconectar con el servidor de notificaciones...");
                return Task.CompletedTask;
            };

            connection.Reconnected += connectionId =>
            {
                _logger.LogInformation("SignalR reconectado correctamente. ID: {ConnectionId}", connectionId);
                OnConnected?.Invoke();
                OnReconnected?.Invoke(connectionId);
                _eventBus?.PublicarEstadoConexion(true);
                return Task.CompletedTask;
            };

            connection.On<NotificacionDto>("ReceiveNotification", notification =>
            {
                _logger.LogInformation(
                    "Notificación recibida: Tipo={Tipo}, TicketId={TicketId}, NotificacionId={NotificacionId}",
                    notification.Tipo,
                    notification.TicketId,
                    notification.IdNotificacion);
                OnNotificationReceived?.Invoke(notification);
                _eventBus?.PublicarNotificacion(notification);
            });

            return connection;
        }

        public async Task DetenerAsync()
        {
            await _lifecycleLock.WaitAsync();
            try
            {
                Volatile.Write(ref _detenerSolicitado, 1);
                await DetenerInternoAsync(notificarDetencion: true);
            }
            finally
            {
                _lifecycleLock.Release();
            }
        }

        private async Task DetenerInternoAsync(bool notificarDetencion)
        {
            var connection = _connection;
            _connection = null;

            if (connection is not null)
            {
                try
                {
                    await connection.StopAsync();
                }
                finally
                {
                    await connection.DisposeAsync();
                }
            }

            if (notificarDetencion)
            {
                OnDisconnected?.Invoke();
                _eventBus?.PublicarEstadoConexion(false, "Servidor de notificaciones detenido.");
            }
        }

    }
}
