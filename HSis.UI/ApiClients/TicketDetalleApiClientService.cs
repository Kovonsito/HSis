using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.ApiClients
{
    public class TicketDetalleApiClientService(HttpClient httpClient) : ITicketDetalleService
    {

        public async Task<List<TicketDetalleDto>> ObtenerDetallesTicketAsync(int idTicket)
        {
            return await httpClient.GetFromJsonAsync<List<TicketDetalleDto>>($"api/TicketDetalle/ticket/{idTicket}") ?? [];
        }

        public async Task<TicketDetalleDto?> ObtenerDetallePorIdAsync(int idTicket, int idMaterial)
        {
            return await httpClient.GetFromJsonAsync<TicketDetalleDto>($"api/TicketDetalle/ticket/{idTicket}/material/{idMaterial}");
        }


        public async Task AgregarMaterialATicketAsync(TicketDetalleDto detTicketDto)
        {
            var response = await httpClient.PostAsJsonAsync("api/TicketDetalle", detTicketDto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task ActualizarDetalleTicketAsync(TicketDetalleDto detTicketDto)
        {
            var response = await httpClient.PutAsJsonAsync("api/TicketDetalle", detTicketDto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task EliminarMaterialDeTicketAsync(int idTicket, int idMaterial)
        {
            var response = await httpClient.DeleteAsync($"api/TicketDetalle/ticket/{idTicket}/material/{idMaterial}");
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task<decimal> ObtenerCostoTotalMaterialesTicketAsync(int idTicket)
        {
            return await httpClient.GetFromJsonAsync<decimal>($"api/TicketDetalle/ticket/{idTicket}/costo-total");
        }

    }
}
