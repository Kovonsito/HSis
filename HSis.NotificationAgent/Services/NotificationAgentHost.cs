using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Desktop.Infrastructure;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HSis.NotificationAgent.Services;

internal sealed class NotificationAgentHost(
    IConfiguration configuration,
    HttpClient apiClient,
    TokenApiClient tokenApiClient,
    IAlmacenamientoCredencialesLocal credentials,
    PresenciaAplicacion applicationPresence,
    NativeNotificationService nativeNotifications,
    ILogger<NotificationAgentHost> logger) : IAsyncDisposable
{
    private static readonly TimeSpan RecentNotificationWindow = TimeSpan.FromMinutes(2);
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    private readonly IConfiguration _configuration = configuration;
    private readonly HttpClient _apiClient = apiClient;
    private readonly TokenApiClient _tokenApiClient = tokenApiClient;
    private readonly IAlmacenamientoCredencialesLocal _credentials = credentials;
    private readonly PresenciaAplicacion _applicationPresence = applicationPresence;
    private readonly NativeNotificationService _nativeNotifications = nativeNotifications;
    private readonly ILogger<NotificationAgentHost> _logger = logger;
    private readonly object _stateLock = new();
    private readonly HashSet<int> _seenNotificationIds = [];
    private readonly Dictionary<string, DateTimeOffset> _recentFingerprints = [];
    private readonly CancellationTokenSource _stopping = new();
    private readonly string _stateFilePath = ConfiguracionEscritorio.ObtenerRutaDatos(
        "notification-agent-state.json");

    private HubConnection? _connection;
    private Task? _supervisionTask;
    private string _token = string.Empty;
    private (string Username, string Password)? _storedCredentials;
    private bool _stateFileExists;
    private int _lastNotificationId;
    private int _started;

    public async Task<bool> StartAsync(CancellationToken cancellationToken = default)
    {
        if (Interlocked.Exchange(ref _started, 1) != 0)
        {
            return true;
        }

        CargarEstado();

        try
        {
            _nativeNotifications.Register();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "No se pudieron registrar las notificaciones nativas; el agente continuará con REST y SignalR.");
        }

        _supervisionTask = EjecutarSupervisionAsync(_stopping.Token);
        return true;
    }

    private async Task<bool> AutenticarAsync(CancellationToken cancellationToken)
    {
        if (!_storedCredentials.HasValue)
        {
            return false;
        }

        try
        {
            var login = await _tokenApiClient.AutenticarAsync(
                _storedCredentials.Value.Username,
                _storedCredentials.Value.Password,
                cancellationToken);
            if (login is null || string.IsNullOrWhiteSpace(login.Token))
            {
                return false;
            }

            _token = login.Token;
            return true;
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "No se pudo autenticar el agente de notificaciones.");
            return false;
        }
    }

    private async Task IniciarSignalRAsync(CancellationToken cancellationToken)
    {
        if (_connection is not null &&
            _connection.State is HubConnectionState.Connected or
            HubConnectionState.Connecting or
            HubConnectionState.Reconnecting)
        {
            return;
        }

        var connectionUrl = _configuration["SignalR:ServerUrl"];
        if (string.IsNullOrWhiteSpace(connectionUrl))
        {
            _logger.LogWarning("No se configuró SignalR:ServerUrl; el agente usará únicamente REST.");
            return;
        }

        var connection = new HubConnectionBuilder()
            .WithUrl(connectionUrl, options =>
            {
                options.AccessTokenProvider = () => Task.FromResult<string?>(_token);
            })
            .WithAutomaticReconnect([
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2),
                TimeSpan.FromSeconds(5),
                TimeSpan.FromSeconds(10),
                TimeSpan.FromSeconds(30)])
            .Build();

        connection.On<NotificacionDto>(
            "ReceiveNotification",
            notification => ProcesarNotificacionAsync(notification, _stopping.Token));

        connection.Reconnecting += error =>
        {
            _logger.LogWarning(error, "El agente está reconectando SignalR.");
            return Task.CompletedTask;
        };
        connection.Reconnected += connectionId =>
        {
            _logger.LogInformation("El agente reconectó SignalR con ID {ConnectionId}.", connectionId);
            return Task.CompletedTask;
        };
        connection.Closed += error =>
        {
            if (!_stopping.IsCancellationRequested)
            {
                _logger.LogWarning(error, "SignalR cerró la conexión del agente.");
            }

            return Task.CompletedTask;
        };

        try
        {
            await connection.StartAsync(cancellationToken);
            _connection = connection;
            _logger.LogInformation("Agente de notificaciones conectado a SignalR.");
        }
        catch (Exception ex) when (ex is HttpRequestException or HubException or TaskCanceledException)
        {
            _logger.LogWarning(ex, "No se pudo iniciar SignalR; continúa el sondeo REST.");
            await connection.DisposeAsync();
        }
    }

    private async Task EjecutarSupervisionAsync(CancellationToken cancellationToken)
    {
        var intervalSeconds = _configuration.GetValue("NotificationAgent:PollIntervalSeconds", 30);
        var interval = TimeSpan.FromSeconds(Math.Clamp(intervalSeconds, 10, 300));
        var retryInitialSeconds = _configuration.GetValue("NotificationAgent:RetryInitialSeconds", 10);
        var retryMaxSeconds = _configuration.GetValue("NotificationAgent:RetryMaxSeconds", 120);
        var retryInitial = TimeSpan.FromSeconds(Math.Clamp(retryInitialSeconds, 5, 60));
        var retryMax = TimeSpan.FromSeconds(Math.Clamp(retryMaxSeconds, 30, 600));
        var retryDelay = retryInitial;
        var initializationPending = !_stateFileExists;

        while (!cancellationToken.IsCancellationRequested)
        {
            try
            {
                var credentials = _credentials.GetCredentials();
                if (!EqualityComparer<(string Username, string Password)?>.Default.Equals(
                        credentials,
                        _storedCredentials))
                {
                    _storedCredentials = credentials;
                    _token = string.Empty;
                    await DetenerSignalRAsync();
                    _logger.LogInformation("Las credenciales locales del agente han cambiado; se renovará la sesión.");
                }

                if (!_storedCredentials.HasValue)
                {
                    _logger.LogInformation("No hay credenciales locales; el agente seguirá esperando el inicio de sesión de la UI.");
                    await Task.Delay(retryDelay, cancellationToken);
                    retryDelay = IncrementarRetardo(retryDelay, retryMax);
                    continue;
                }

                if (string.IsNullOrWhiteSpace(_token) && !await AutenticarAsync(cancellationToken))
                {
                    await Task.Delay(retryDelay, cancellationToken);
                    retryDelay = IncrementarRetardo(retryDelay, retryMax);
                    continue;
                }

                await IniciarSignalRAsync(cancellationToken);
                var sincronizada = await SincronizarAsync(initializationPending, cancellationToken);
                if (sincronizada)
                {
                    initializationPending = false;
                    retryDelay = retryInitial;
                    await Task.Delay(interval, cancellationToken);
                }
                else
                {
                    await Task.Delay(retryDelay, cancellationToken);
                    retryDelay = IncrementarRetardo(retryDelay, retryMax);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error en el ciclo de supervisión del agente de notificaciones.");
                _token = string.Empty;
                await DetenerSignalRAsync();
                await Task.Delay(retryDelay, cancellationToken);
                retryDelay = IncrementarRetardo(retryDelay, retryMax);
            }
        }
    }

    private async Task<bool> SincronizarAsync(bool inicializacion, CancellationToken cancellationToken)
    {
        var notificaciones = await ObtenerNuevasAsync(cancellationToken);
        if (notificaciones is null)
        {
            return false;
        }

        foreach (var notification in notificaciones.OrderBy(n => n.IdNotificacion))
        {
            if (inicializacion)
            {
                RegistrarComoProcesada(notification);
            }
            else
            {
                await ProcesarNotificacionAsync(notification, cancellationToken);
            }

            RegistrarUltimoId(notification.IdNotificacion);
        }

        GuardarEstado();
        return true;
    }

    private async Task<IReadOnlyList<NotificacionDto>?> ObtenerNuevasAsync(CancellationToken cancellationToken)
    {
        int desdeId;
        lock (_stateLock)
        {
            desdeId = _lastNotificationId;
        }

        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            $"api/Notificaciones/nuevas?desdeId={desdeId}");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        using var response = await _apiClient.SendAsync(request, cancellationToken);

        if (response.StatusCode == HttpStatusCode.Unauthorized && await AutenticarAsync(cancellationToken))
        {
            await DetenerSignalRAsync();
            using var retry = new HttpRequestMessage(
                HttpMethod.Get,
                $"api/Notificaciones/nuevas?desdeId={desdeId}");
            retry.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
            using var retryResponse = await _apiClient.SendAsync(retry, cancellationToken);
            if (!retryResponse.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "El servidor rechazó el sondeo REST después de renovar el token: {StatusCode}.",
                    (int)retryResponse.StatusCode);
                return null;
            }

            return await retryResponse.Content.ReadFromJsonAsync<List<NotificacionDto>>(cancellationToken);
        }

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogWarning(
                "El servidor rechazó la sincronización incremental del agente: {StatusCode}.",
                (int)response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<List<NotificacionDto>>(cancellationToken);
    }

    private static TimeSpan IncrementarRetardo(TimeSpan actual, TimeSpan maximo)
        => TimeSpan.FromSeconds(Math.Min(actual.TotalSeconds * 2, maximo.TotalSeconds));

    private Task ProcesarNotificacionAsync(
        NotificacionDto notification,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (!RegistrarComoProcesada(notification))
        {
            return Task.CompletedTask;
        }

        var timeoutSeconds = _configuration.GetValue("NotificationAgent:PresenceTimeoutSeconds", 15);
        var uiActiva = _applicationPresence.EstaActiva(
            TimeSpan.FromSeconds(Math.Clamp(timeoutSeconds, 5, 120)));
        if (!uiActiva && _configuration.GetValue("NotificationAgent:EnableNativeNotifications", true))
        {
            _nativeNotifications.Show(notification);
        }

        return Task.CompletedTask;
    }

    private bool RegistrarComoProcesada(NotificacionDto notification)
    {
        var fingerprint = CrearFingerprint(notification);
        var ahora = DateTimeOffset.UtcNow;

        lock (_stateLock)
        {
            LimpiarFingerprintsAntiguos(ahora);

            if (notification.IdNotificacion > 0 &&
                _seenNotificationIds.Contains(notification.IdNotificacion))
            {
                return false;
            }

            if (_recentFingerprints.TryGetValue(fingerprint, out var recibida) &&
                ahora - recibida <= RecentNotificationWindow)
            {
                if (notification.IdNotificacion > 0)
                {
                    _seenNotificationIds.Add(notification.IdNotificacion);
                }

                GuardarEstadoBloqueado();
                return false;
            }

            _recentFingerprints[fingerprint] = ahora;
            if (notification.IdNotificacion > 0)
            {
                _seenNotificationIds.Add(notification.IdNotificacion);
            }

            GuardarEstadoBloqueado();
            return true;
        }
    }

    private void CargarEstado()
    {
        lock (_stateLock)
        {
            try
            {
                _stateFileExists = File.Exists(_stateFilePath);
                if (!_stateFileExists)
                {
                    return;
                }

                var state = JsonSerializer.Deserialize<AgentState>(
                    File.ReadAllText(_stateFilePath),
                    JsonOptions);
                if (state is null)
                {
                    return;
                }

                foreach (var id in state.SeenNotificationIds.Where(id => id > 0))
                {
                    _seenNotificationIds.Add(id);
                }

                _lastNotificationId = Math.Max(
                    state.LastNotificationId,
                    _seenNotificationIds.DefaultIfEmpty().Max());
            }
            catch (IOException ex)
            {
                _logger.LogDebug(ex, "No se pudo cargar el estado del agente.");
            }
            catch (JsonException ex)
            {
                _logger.LogDebug(ex, "El estado del agente no tiene un formato válido.");
            }
        }
    }

    private void GuardarEstado()
    {
        lock (_stateLock)
        {
            GuardarEstadoBloqueado();
        }
    }

    private void GuardarEstadoBloqueado()
    {
        try
        {
            var directory = Path.GetDirectoryName(_stateFilePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var ids = _seenNotificationIds.Order().TakeLast(500).ToArray();
            var lastNotificationId = Math.Max(_lastNotificationId, ids.DefaultIfEmpty().Max());
            var state = new AgentState(lastNotificationId, ids);
            var temporaryPath = $"{_stateFilePath}.{Environment.ProcessId}.tmp";
            File.WriteAllText(temporaryPath, JsonSerializer.Serialize(state, JsonOptions));
            File.Move(temporaryPath, _stateFilePath, overwrite: true);
            _stateFileExists = true;
        }
        catch (IOException ex)
        {
            _logger.LogDebug(ex, "No se pudo guardar el estado del agente.");
        }
        catch (UnauthorizedAccessException ex)
        {
            _logger.LogDebug(ex, "No se pudo guardar el estado del agente.");
        }
    }

    private void LimpiarFingerprintsAntiguos(DateTimeOffset ahora)
    {
        foreach (var item in _recentFingerprints
                     .Where(item => ahora - item.Value > RecentNotificationWindow)
                     .ToArray())
        {
            _recentFingerprints.Remove(item.Key);
        }
    }

    private static string CrearFingerprint(NotificacionDto notification)
        => $"{notification.TicketId}|{notification.MaterialId}|{notification.Tipo}|{notification.Mensaje}";

    private void RegistrarUltimoId(int notificationId)
    {
        if (notificationId <= 0)
        {
            return;
        }

        lock (_stateLock)
        {
            _lastNotificationId = Math.Max(_lastNotificationId, notificationId);
        }
    }

    private async Task DetenerSignalRAsync()
    {
        var connection = Interlocked.Exchange(ref _connection, null);
        if (connection is null)
        {
            return;
        }

        try
        {
            await connection.StopAsync();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "No se pudo detener SignalR antes de renovar la conexión del agente.");
        }

        await connection.DisposeAsync();
    }

    public async ValueTask DisposeAsync()
    {
        _stopping.Cancel();
        if (_supervisionTask is not null)
        {
            try
            {
                await _supervisionTask;
            }
            catch (OperationCanceledException)
            {
            }
        }

        await DetenerSignalRAsync();

        _nativeNotifications.Dispose();
        _stopping.Dispose();
    }

    private sealed record AgentState(int LastNotificationId, int[] SeenNotificationIds);
}
