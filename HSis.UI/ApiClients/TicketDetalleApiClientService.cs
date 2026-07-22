using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.Services;
using HSis.Data.Models;

namespace HSis.UI.ApiClients
{
    public class TicketDetalleApiClientService(HttpClient httpClient) : ITicketDetalleService
    {

        public async Task<List<DetTicket>> ObtenerDetallesTicketAsync(int idTicket)
        {
            return await httpClient.GetFromJsonAsync<List<DetTicket>>($"api/TicketDetalle/ticket/{idTicket}") ?? [];
        }

        public async Task<DetTicket?> ObtenerDetallePorIdAsync(int idTicket, int idMaterial)
        {
            return await httpClient.GetFromJsonAsync<DetTicket>($"api/TicketDetalle/ticket/{idTicket}/material/{idMaterial}");
        }

        public async Task AgregarMaterialATicketAsync(DetTicket detTicket)
        {
            var response = await httpClient.PostAsJsonAsync("api/TicketDetalle", detTicket);
            response.EnsureSuccessStatusCode();
        }

        public async Task ActualizarDetalleTicketAsync(DetTicket detTicket)
        {
            var response = await httpClient.PutAsJsonAsync("api/TicketDetalle", detTicket);
            response.EnsureSuccessStatusCode();
        }

        public async Task EliminarMaterialDeTicketAsync(int idTicket, int idMaterial)
        {
            var response = await httpClient.DeleteAsync($"api/TicketDetalle/ticket/{idTicket}/material/{idMaterial}");
            response.EnsureSuccessStatusCode();
        }

        public async Task<decimal> ObtenerCostoTotalMaterialesTicketAsync(int idTicket)
        {
            return await httpClient.GetFromJsonAsync<decimal>($"api/TicketDetalle/ticket/{idTicket}/costo-total");
        }
    }
}
