using System.Net.Http;
using System.Net.Http.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients
{
    public class TicketApiClientService(HttpClient httpClient) : ITicketService
    {
        public async Task<List<TicketDto>> ObtenerTicketsAsync()
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>("api/Tickets") ?? [];
        }

        public async Task<TicketDto?> ObtenerTicketPorIdAsync(int id)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<TicketDto>($"api/Tickets/{id}");
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorSLAAsync(bool esUrgente)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/sla?esUrgente={esUrgente}") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/estatus/{Uri.EscapeDataString(estatus)}") ?? [];
        }

        public async Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<int>($"api/Tickets/sla/count?esUrgente={esUrgente}");
        }

        public async Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<int>($"api/Tickets/estatus/{Uri.EscapeDataString(estatus)}/count");
        }

        public async Task<List<HistorialCambiosDto>> ObtenerHistorialPorTicketAsync(int idTicket)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<HistorialCambiosDto>>($"api/Tickets/{idTicket}/historial") ?? [];
        }

        public async Task ActualizarTicketAsync(TicketUpdateDto ticketDto)
        {
            using var response = await httpClient.PutAsJsonAsync($"api/Tickets/{ticketDto.IdTicket}", ticketDto);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorUsuarioAsync(int idUsuario)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/usuario/{idUsuario}") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/asignados") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsCerradosPorTecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/cerrados") ?? [];
        }

        public async Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync()
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>("api/Tickets/disponibles") ?? [];
        }

        public async Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto)
        {
            using var response = await httpClient.PostAsJsonAsync("api/Tickets", ticketDto);
            return await response.ReadRequiredJsonWithDetailsAsync<TicketDto>("ticket creado");
        }

        public async Task<List<TicketDto>> ObtenerTicketsFiltradosAsync(TicketFilterDto filtros)
        {
            using var response = await httpClient.PostAsJsonAsync("api/Tickets/filtrar/todos", filtros);
            return await response.ReadFromJsonWithDetailsAsync<List<TicketDto>>() ?? [];
        }

        public async Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize)
        {
            using var response = await httpClient.PostAsJsonAsync($"api/Tickets/filtrar?pageNumber={pageNumber}&pageSize={pageSize}", filtros);
            return await response.ReadFromJsonWithDetailsAsync<PaginatedResultDto<TicketDto>>() ?? new PaginatedResultDto<TicketDto>();
        }

        public async Task<ReporteKpisDto> ObtenerReporteKpisAsync(DateTime inicio, DateTime fin)
        {
            var inicioStr = Uri.EscapeDataString(inicio.ToString("o"));
            var finStr = Uri.EscapeDataString(fin.ToString("o"));
            return await httpClient.GetFromJsonWithDetailsAsync<ReporteKpisDto>($"api/Tickets/kpis?inicio={inicioStr}&fin={finStr}") ?? new ReporteKpisDto();
        }

        public async Task<bool> RegistrarCalificacionAsync(int idTicket, int calificacion, string? comentario)
        {
            var request = new { Calificacion = calificacion, Comentario = comentario };
            using var response = await httpClient.PostAsJsonAsync($"api/Tickets/{idTicket}/calificar", request);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return true;
        }

        public async Task<double> ObtenerPromedioCalificacionTecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<double>($"api/Tickets/tecnico/{idTecnico}/promedio-calificacion");
        }

        public async Task<List<TicketDto>> ObtenerFeedbackTecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/feedback") ?? [];
        }

        public async Task<DashboardResumenDto> ObtenerResumenDashboardAsync(int? idTecnico = null)
        {
            var url = idTecnico.HasValue ? $"api/Tickets/dashboard-resumen?idTecnico={idTecnico.Value}" : "api/Tickets/dashboard-resumen";
            return await httpClient.GetFromJsonWithDetailsAsync<DashboardResumenDto>(url) ?? new DashboardResumenDto();
        }

        public async Task<IndicadoresTecnicoDto> ObtenerIndicadoresTecnicoAsync(int idTecnico)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<IndicadoresTecnicoDto>($"api/Tickets/tecnico/{idTecnico}/indicadores")
                   ?? new IndicadoresTecnicoDto(0, 0, 0, 0.0);
        }

        public async Task<ResumenClienteDto> ObtenerResumenClienteAsync(int idUsuario)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<ResumenClienteDto>($"api/Tickets/usuario/{idUsuario}/resumen")
                   ?? new ResumenClienteDto(0, 0, []);
        }
    }
}
