#nullable enable
using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using Microsoft.Extensions.Logging;

namespace HSis.UI.Services;

public sealed class OutboxSyncWorker : IDisposable
{
    private readonly ILocalDataStore _localDataStore;
    private readonly HttpClient _httpClient;
    private readonly IBusEventosNotificaciones _eventBus;
    private readonly ILogger<OutboxSyncWorker> _logger;
    private readonly SemaphoreSlim _syncLock = new(1, 1);
    private readonly System.Threading.Timer _timer;
    private bool _conectado;
    private bool _disposed;

    public event EventHandler? OnSincronizacionCompletada;

    public OutboxSyncWorker(
        ILocalDataStore localDataStore,
        HttpClient httpClient,
        IBusEventosNotificaciones eventBus,
        ILogger<OutboxSyncWorker> logger)
    {
        _localDataStore = localDataStore;
        _httpClient = httpClient;
        _eventBus = eventBus;
        _logger = logger;

        _eventBus.OnEstadoConexionCambiado += EnEstadoConexionCambiado;
        _timer = new System.Threading.Timer(AlDispararTimer, null, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(30));
    }

    private void EnEstadoConexionCambiado(object? sender, EstadoConexionEventArgs e)
    {
        _conectado = e.Conectado;
        if (_conectado)
        {
            _ = Task.Run(() => SincronizarPendientesAsync());
        }
    }

    private void AlDispararTimer(object? state)
    {
        if (_conectado)
        {
            _ = SincronizarPendientesAsync();
        }
    }

    public async Task<int> SincronizarPendientesAsync(CancellationToken cancellationToken = default)
    {
        if (!await _syncLock.WaitAsync(0, cancellationToken))
        {
            return 0; // Ya hay una sincronización en curso
        }

        try
        {
            var pendientes = await _localDataStore.ObtenerOperacionesPendientesAsync();
            if (pendientes.Count == 0)
            {
                return 0;
            }

            _logger.LogInformation("Iniciando sincronización de {Count} operaciones offline pendientes...", pendientes.Count);
            int sincronizadas = 0;

            foreach (var op in pendientes)
            {
                if (cancellationToken.IsCancellationRequested) break;

                try
                {
                    using var request = new HttpRequestMessage(new HttpMethod(op.MetodoHttp), op.RutaEndpoint);
                    request.Headers.Add(ConstantesSincronizacion.HeaderIdempotencia, op.IdOperacion.ToString());

                    if (!string.IsNullOrWhiteSpace(op.PayloadJson))
                    {
                        request.Content = new StringContent(op.PayloadJson, Encoding.UTF8, "application/json");
                    }

                    using var response = await _httpClient.SendAsync(request, cancellationToken);

                    if (response.IsSuccessStatusCode)
                    {
                        int? idReal = null;
                        if (op.TipoOperacion == "CREAR_TICKET")
                        {
                            try
                            {
                                var creado = await response.Content.ReadFromJsonAsync<TicketDto>(cancellationToken: cancellationToken);
                                if (creado != null)
                                {
                                    idReal = creado.IdTicket;
                                    await _localDataStore.GuardarTicketIndividualCacheAsync(creado, esOffline: false);
                                }
                            }
                            catch
                            {
                            }
                        }

                        await _localDataStore.MarcarOperacionSincronizadaAsync(op.IdOperacion, idReal);
                        sincronizadas++;
                        _logger.LogInformation("Operación {IdOperacion} ({Tipo}) sincronizada con éxito.", op.IdOperacion, op.TipoOperacion);
                    }
                    else if ((int)response.StatusCode is >= 400 and < 500 && (int)response.StatusCode is not (408 or 429))
                    {
                        var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                        _logger.LogWarning("Operación {IdOperacion} rechazada por servidor ({StatusCode}): {Error}", op.IdOperacion, response.StatusCode, errorBody);
                        await _localDataStore.MarcarOperacionFallidaAsync(op.IdOperacion, $"Error {(int)response.StatusCode}: {errorBody}");
                    }
                    else
                    {
                        _logger.LogWarning("Error transitorio del servidor al sincronizar {IdOperacion} ({StatusCode}). Se detiene el lote.", op.IdOperacion, response.StatusCode);
                        break; // Error transitorio o caída de red: detener lote
                    }
                }
                catch (Exception ex) when (ex is HttpRequestException or System.Net.Sockets.SocketException or TaskCanceledException)
                {
                    _logger.LogWarning("Pérdida de conectividad durante la sincronización: {Message}", ex.Message);
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error inesperado al procesar operación {IdOperacion}", op.IdOperacion);
                    await _localDataStore.MarcarOperacionFallidaAsync(op.IdOperacion, ex.Message);
                }
            }

            if (sincronizadas > 0)
            {
                OnSincronizacionCompletada?.Invoke(this, EventArgs.Empty);
            }

            return sincronizadas;
        }
        finally
        {
            _syncLock.Release();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;
        _disposed = true;
        _eventBus.OnEstadoConexionCambiado -= EnEstadoConexionCambiado;
        _timer.Dispose();
        _syncLock.Dispose();
    }
}
