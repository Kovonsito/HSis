using System.Net.Http;
using System.Net.Http.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients;

public sealed class NotificacionesApiClientService(HttpClient httpClient) : INotificacionesApiClient
{
    public async Task<NotificacionesResumenDto> ObtenerResumenAsync(
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonWithDetailsAsync<NotificacionesResumenDto>(
            "api/Notificaciones",
            cancellationToken)
        ?? new NotificacionesResumenDto([], 0);

    public async Task<IReadOnlyList<NotificacionDto>> ObtenerNuevasAsync(
        int desdeId,
        CancellationToken cancellationToken = default)
        => await httpClient.GetFromJsonWithDetailsAsync<List<NotificacionDto>>(
            $"api/Notificaciones/nuevas?desdeId={desdeId}",
            cancellationToken)
        ?? [];

    public async Task MarcarComoLeidaAsync(
        int idNotificacion,
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync(
            $"api/Notificaciones/{idNotificacion}/leida",
            content: null,
            cancellationToken);
        await response.EnsureSuccessStatusCodeWithDetailsAsync();
    }

    public async Task<int> MarcarTodasComoLeidasAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.PutAsync(
            "api/Notificaciones/leidas",
            content: null,
            cancellationToken);
        return await response.ReadRequiredJsonWithDetailsAsync<int>(
            "cantidad de notificaciones actualizadas",
            cancellationToken);
    }

    public async Task LimpiarTodasAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await httpClient.DeleteAsync(
            "api/Notificaciones",
            cancellationToken);
        await response.EnsureSuccessStatusCodeWithDetailsAsync();
    }
}
