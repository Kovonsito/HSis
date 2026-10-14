#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Sockets;
using System.Text.Json;
using System.Threading.Tasks;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients
{
    public class TicketApiClientService(HttpClient httpClient, ILocalDataStore? localDataStore = null) : ITicketService
    {
        private static bool EsFalloConexion(Exception ex) =>
            ex is HttpRequestException or SocketException or TaskCanceledException;

        public async Task<List<TicketDto>> ObtenerTicketsAsync()
        {
            try
            {
                var tickets = await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>("api/Tickets") ?? [];
                if (localDataStore != null && tickets.Count > 0)
                {
                    _ = localDataStore.GuardarTicketsCacheAsync(tickets);
                }
                return tickets;
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                return await localDataStore.ObtenerTicketsCacheAsync();
            }
        }

        public async Task<TicketDto?> ObtenerTicketPorIdAsync(int id)
        {
            try
            {
                var ticket = await httpClient.GetFromJsonWithDetailsAsync<TicketDto>($"api/Tickets/{id}");
                if (localDataStore != null && ticket != null)
                {
                    _ = localDataStore.GuardarTicketIndividualCacheAsync(ticket);
                }
                return ticket;
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                return await localDataStore.ObtenerTicketCachePorIdAsync(id);
            }
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorSLAAsync(bool esUrgente)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/sla?esUrgente={esUrgente}") ?? [];
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var todos = await localDataStore.ObtenerTicketsCacheAsync();
                return esUrgente
                    ? todos.Where(t => t.Prioridad == ConstantesPrioridad.URGENTE).ToList()
                    : todos.Where(t => t.Prioridad != ConstantesPrioridad.URGENTE).ToList();
            }
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/estatus/{Uri.EscapeDataString(estatus)}") ?? [];
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                return await localDataStore.ObtenerTicketsCacheAsync(estatus: estatus);
            }
        }

        public async Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<int>($"api/Tickets/sla/count?esUrgente={esUrgente}");
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tickets = await ObtenerTicketsPorSLAAsync(esUrgente);
                return tickets.Count;
            }
        }

        public async Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<int>($"api/Tickets/estatus/{Uri.EscapeDataString(estatus)}/count");
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tickets = await localDataStore.ObtenerTicketsCacheAsync(estatus: estatus);
                return tickets.Count;
            }
        }

        public async Task<List<HistorialCambiosDto>> ObtenerHistorialPorTicketAsync(int idTicket)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<List<HistorialCambiosDto>>($"api/Tickets/{idTicket}/historial") ?? [];
            }
            catch (Exception ex) when (EsFalloConexion(ex))
            {
                return [];
            }
        }

        public async Task ActualizarTicketAsync(TicketUpdateDto ticketDto)
        {
            var idOperacion = Guid.NewGuid();
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Put, $"api/Tickets/{ticketDto.IdTicket}")
                {
                    Content = JsonContent.Create(ticketDto)
                };
                request.Headers.Add(ConstantesSincronizacion.HeaderIdempotencia, idOperacion.ToString());
                using var response = await httpClient.SendAsync(request);
                await response.EnsureSuccessStatusCodeWithDetailsAsync();

                if (localDataStore != null && !string.IsNullOrWhiteSpace(ticketDto.Estatus))
                {
                    _ = localDataStore.ActualizarEstatusTicketCacheAsync(ticketDto.IdTicket, ticketDto.Estatus);
                }
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var payloadJson = JsonSerializer.Serialize(ticketDto);
                await localDataStore.EncolarOperacionAsync(
                    "ACTUALIZAR_TICKET",
                    $"api/Tickets/{ticketDto.IdTicket}",
                    "PUT",
                    payloadJson,
                    ticketDto.IdTicket.ToString());

                if (!string.IsNullOrWhiteSpace(ticketDto.Estatus))
                {
                    await localDataStore.ActualizarEstatusTicketCacheAsync(ticketDto.IdTicket, ticketDto.Estatus);
                }
            }
        }

        public async Task<List<TicketDto>> ObtenerTicketsPorUsuarioAsync(int idUsuario)
        {
            try
            {
                var tickets = await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/usuario/{idUsuario}") ?? [];
                if (localDataStore != null && tickets.Count > 0)
                {
                    _ = localDataStore.GuardarTicketsCacheAsync(tickets);
                }
                return tickets;
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                return await localDataStore.ObtenerTicketsCacheAsync(idUsuario: idUsuario);
            }
        }

        public async Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)
        {
            try
            {
                var tickets = await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/asignados") ?? [];
                if (localDataStore != null && tickets.Count > 0)
                {
                    _ = localDataStore.GuardarTicketsCacheAsync(tickets);
                }
                return tickets;
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                return await localDataStore.ObtenerTicketsCacheAsync(idTecnico: idTecnico);
            }
        }

        public async Task<List<TicketDto>> ObtenerTicketsCerradosPorTecnicoAsync(int idTecnico)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/cerrados") ?? [];
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tickets = await localDataStore.ObtenerTicketsCacheAsync(idTecnico: idTecnico, estatus: ConstantesEstatus.CERRADO);
                return tickets;
            }
        }

        public async Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync()
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>("api/Tickets/disponibles") ?? [];
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                return await localDataStore.ObtenerTicketsCacheAsync(estatus: ConstantesEstatus.DISPONIBLE);
            }
        }

        public async Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto)
        {
            var idOperacion = Guid.NewGuid();
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, "api/Tickets")
                {
                    Content = JsonContent.Create(ticketDto)
                };
                request.Headers.Add(ConstantesSincronizacion.HeaderIdempotencia, idOperacion.ToString());
                using var response = await httpClient.SendAsync(request);
                var creado = await response.ReadRequiredJsonWithDetailsAsync<TicketDto>("ticket creado");
                if (localDataStore != null)
                {
                    _ = localDataStore.GuardarTicketIndividualCacheAsync(creado);
                }
                return creado;
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tempId = -Math.Abs(idOperacion.GetHashCode());
                var payloadJson = JsonSerializer.Serialize(ticketDto);
                await localDataStore.EncolarOperacionAsync(
                    "CREAR_TICKET",
                    "api/Tickets",
                    "POST",
                    payloadJson,
                    tempId.ToString());

                var ticketLocal = new TicketDto
                {
                    IdTicket = tempId,
                    IdUsuario = ticketDto.IdUsuario,
                    NombreUsuario = "Usuario (Local)",
                    Descripcion = ticketDto.Descripcion,
                    Estatus = ConstantesEstatus.DISPONIBLE,
                    Prioridad = ticketDto.Prioridad ?? ConstantesPrioridad.MEDIA,
                    FechaAlta = DateTime.Now
                };

                await localDataStore.GuardarTicketIndividualCacheAsync(ticketLocal, esOffline: true);
                return ticketLocal;
            }
        }

        public async Task<List<TicketDto>> ObtenerTicketsFiltradosAsync(TicketFilterDto filtros)
        {
            try
            {
                using var response = await httpClient.PostAsJsonAsync("api/Tickets/filtrar/todos", filtros);
                return await response.ReadFromJsonWithDetailsAsync<List<TicketDto>>() ?? [];
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                return await localDataStore.ObtenerTicketsCacheAsync();
            }
        }

        public async Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize)
        {
            try
            {
                using var response = await httpClient.PostAsJsonAsync($"api/Tickets/filtrar?pageNumber={pageNumber}&pageSize={pageSize}", filtros);
                return await response.ReadFromJsonWithDetailsAsync<PaginatedResultDto<TicketDto>>() ?? new PaginatedResultDto<TicketDto>();
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tickets = await localDataStore.ObtenerTicketsCacheAsync();
                return new PaginatedResultDto<TicketDto>
                {
                    Items = tickets.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                    TotalCount = tickets.Count
                };
            }
        }

        public async Task<ReporteKpisDto> ObtenerReporteKpisAsync(DateTime inicio, DateTime fin)
        {
            var inicioStr = Uri.EscapeDataString(inicio.ToString("o"));
            var finStr = Uri.EscapeDataString(fin.ToString("o"));
            return await httpClient.GetFromJsonWithDetailsAsync<ReporteKpisDto>($"api/Tickets/kpis?inicio={inicioStr}&fin={finStr}") ?? new ReporteKpisDto();
        }

        public async Task<bool> RegistrarCalificacionAsync(int idTicket, int calificacion, string? comentario)
        {
            var idOperacion = Guid.NewGuid();
            var requestObj = new { Calificacion = calificacion, Comentario = comentario };
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, $"api/Tickets/{idTicket}/calificar")
                {
                    Content = JsonContent.Create(requestObj)
                };
                request.Headers.Add(ConstantesSincronizacion.HeaderIdempotencia, idOperacion.ToString());
                using var response = await httpClient.SendAsync(request);
                await response.EnsureSuccessStatusCodeWithDetailsAsync();
                return true;
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var payloadJson = JsonSerializer.Serialize(requestObj);
                await localDataStore.EncolarOperacionAsync(
                    "REGISTRAR_CALIFICACION",
                    $"api/Tickets/{idTicket}/calificar",
                    "POST",
                    payloadJson,
                    idTicket.ToString());
                return true;
            }
        }

        public async Task<double> ObtenerPromedioCalificacionTecnicoAsync(int idTecnico)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<double>($"api/Tickets/tecnico/{idTecnico}/promedio-calificacion");
            }
            catch (Exception ex) when (EsFalloConexion(ex))
            {
                return 0.0;
            }
        }

        public async Task<List<TicketDto>> ObtenerFeedbackTecnicoAsync(int idTecnico)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<List<TicketDto>>($"api/Tickets/tecnico/{idTecnico}/feedback") ?? [];
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tickets = await localDataStore.ObtenerTicketsCacheAsync(idTecnico: idTecnico);
                return tickets.Where(t => t.Calificacion.HasValue).ToList();
            }
        }

        public async Task<DashboardResumenDto> ObtenerResumenDashboardAsync(int? idTecnico = null, DateTime? fechaInicio = null, DateTime? fechaFin = null)
        {
            try
            {
                var query = new List<string>();
                if (idTecnico.HasValue) query.Add($"idTecnico={idTecnico.Value}");
                if (fechaInicio.HasValue) query.Add($"fechaInicio={Uri.EscapeDataString(fechaInicio.Value.ToString("o"))}");
                if (fechaFin.HasValue) query.Add($"fechaFin={Uri.EscapeDataString(fechaFin.Value.ToString("o"))}");

                var url = query.Count > 0
                    ? $"api/Tickets/dashboard-resumen?{string.Join("&", query)}"
                    : "api/Tickets/dashboard-resumen";

                return await httpClient.GetFromJsonWithDetailsAsync<DashboardResumenDto>(url) ?? new DashboardResumenDto();
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tickets = await localDataStore.ObtenerTicketsCacheAsync(idTecnico: idTecnico);
                return new DashboardResumenDto
                {
                    TotalNuevos = tickets.Count(t => t.Estatus == ConstantesEstatus.ABIERTO || t.Estatus == ConstantesEstatus.DISPONIBLE),
                    TotalEnProceso = tickets.Count(t => t.Estatus == ConstantesEstatus.EN_PROCESO),
                    TotalCerrados = tickets.Count(t => t.Estatus == ConstantesEstatus.CERRADO),
                    TotalUrgentes = tickets.Count(t => t.Prioridad == ConstantesPrioridad.URGENTE),
                    TotalPendientes = tickets.Count(t => t.Estatus != ConstantesEstatus.CERRADO)
                };
            }
        }

        public async Task<IndicadoresTecnicoDto> ObtenerIndicadoresTecnicoAsync(int idTecnico)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<IndicadoresTecnicoDto>($"api/Tickets/tecnico/{idTecnico}/indicadores")
                       ?? new IndicadoresTecnicoDto(0, 0, 0, 0.0);
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var asignados = await localDataStore.ObtenerTicketsCacheAsync(idTecnico: idTecnico);
                var resueltos = asignados.Count(t => t.Estatus == ConstantesEstatus.CERRADO);
                var pendientes = asignados.Count(t => t.Estatus != ConstantesEstatus.CERRADO);
                return new IndicadoresTecnicoDto(asignados.Count, resueltos, pendientes, 0.0);
            }
        }

        public async Task<ResumenClienteDto> ObtenerResumenClienteAsync(int idUsuario)
        {
            try
            {
                return await httpClient.GetFromJsonWithDetailsAsync<ResumenClienteDto>($"api/Tickets/usuario/{idUsuario}/resumen")
                       ?? new ResumenClienteDto(0, 0, []);
            }
            catch (Exception ex) when (EsFalloConexion(ex) && localDataStore != null)
            {
                var tickets = await localDataStore.ObtenerTicketsCacheAsync(idUsuario: idUsuario);
                var abiertos = tickets.Count(t => t.Estatus != ConstantesEstatus.CERRADO);
                var cerrados = tickets.Count(t => t.Estatus == ConstantesEstatus.CERRADO);
                return new ResumenClienteDto(abiertos, cerrados, tickets.Take(5).ToList());
            }
        }
    }
}
