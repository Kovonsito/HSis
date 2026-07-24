using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.ApiClients
{
    public class TicketApiClientService(HttpClient httpClient) : ITicketService
    {

        public async Task<List<TicketDto>> ObtenerTicketsAsync()
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>("api/Tickets") ?? [];
        }

        public async Task<TicketDto?> ObtenerTicketPorIdAsync(int id)
        {
            return await httpClient.GetFromJsonAsync<TicketDto>($"api/Tickets/{id}");
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorSLAAsync(bool esUrgente)
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>($"api/Tickets/sla?esUrgente={esUrgente}") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus)
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>($"api/Tickets/estatus/{Uri.EscapeDataString(estatus)}") ?? [];
        }

        public async Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente)
        {
            return await httpClient.GetFromJsonAsync<int>($"api/Tickets/sla/count?esUrgente={esUrgente}");
        }

        public async Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus)
        {
            return await httpClient.GetFromJsonAsync<int>($"api/Tickets/estatus/{Uri.EscapeDataString(estatus)}/count");
        }

        public async Task<List<object>> ObtenerHistorialPorTicketAsync(int idTicket)
        {
            return await httpClient.GetFromJsonAsync<List<object>>($"api/Tickets/{idTicket}/historial") ?? [];
        }

        public async Task ActualizarTicketAsync(TicketUpdateDto ticketDto)
        {
            var response = await httpClient.PutAsJsonAsync($"api/Tickets/{ticketDto.IdTicket}", ticketDto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorUsuarioAsync(int idUsuario)
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>($"api/Tickets/usuario/{idUsuario}") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/asignados") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsCerradosPorTecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/cerrados") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync()
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>("api/Tickets/disponibles") ?? [];
        }

        public async Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto)
        {
            var response = await httpClient.PostAsJsonAsync("api/Tickets", ticketDto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadFromJsonAsync<TicketDto>() ?? new TicketDto();
        }

        public async Task<List<TicketDto>> ObtenerTicketsFiltradosAsync(TicketFilterDto filtros)
        {
            var response = await httpClient.PostAsJsonAsync("api/Tickets/filtrar/todos", filtros);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadFromJsonAsync<List<TicketDto>>() ?? [];
        }

        public async Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize)
        {
            var response = await httpClient.PostAsJsonAsync($"api/Tickets/filtrar?pageNumber={pageNumber}&pageSize={pageSize}", filtros);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadFromJsonAsync<PaginatedResultDto<TicketDto>>() ?? new PaginatedResultDto<TicketDto>();
        }

        public async Task<ReporteKpisDto> ObtenerReporteKpisAsync(DateTime inicio, DateTime fin)
        {
            var inicioStr = Uri.EscapeDataString(inicio.ToString("o"));
            var finStr = Uri.EscapeDataString(fin.ToString("o"));
            return await httpClient.GetFromJsonAsync<ReporteKpisDto>($"api/Tickets/kpis?inicio={inicioStr}&fin={finStr}") ?? new ReporteKpisDto();
        }

        public async Task<bool> RegistrarCalificacionAsync(int idTicket, int calificacion, string? comentario)
        {
            var request = new { Calificacion = calificacion, Comentario = comentario };
            var response = await httpClient.PostAsJsonAsync($"api/Tickets/{idTicket}/calificar", request);
            return response.IsSuccessStatusCode;
        }

        public async Task<double> ObtenerPromedioCalificacionTecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonAsync<double>($"api/Tickets/tecnico/{idTecnico}/promedio-calificacion");
        }

        public async Task<List<TicketDto>> ObtenerFeedbackTecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/feedback") ?? [];
        }
    }
}
