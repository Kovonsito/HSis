using HSis.Data.Models;
using HSis.Logic.DTOs;
using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    public class TicketService(
        IDbContextFactory<HSisDbContext> dbContextFactory,
        IMapper mapper,
        FluentValidation.IValidator<TicketCreateDto> createValidator,
        FluentValidation.IValidator<TicketUpdateDto> updateValidator,
        INotificationClientService? notificationClient = null) : ITicketService
    {
        private static DateTime ObtenerLimiteSLA() => DateTime.Now.AddHours(-48);

        // Obtener todos los tickets - Async
        public async Task<List<TicketDto>> ObtenerTicketsAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync());
        }

        // Obtener un ticket por su id - Async
        public async Task<TicketDto?> ObtenerTicketPorIdAsync(int id)
        {
            using var db = dbContextFactory.CreateDbContext();
            var ticket = await db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .ThenInclude(u => u.IdDepartamentoNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .FirstOrDefaultAsync(t => t.IdTicket == id);
            return ticket is null ? null : mapper.Map<TicketDto>(ticket);
        }

        // Obtener tickets filtrados por SLA (urgentes/no urgentes) - Async
        public async Task<List<TicketDto>> ObtenerTicketsPorSLAAsync(bool esUrgente)
        {
            using var db = dbContextFactory.CreateDbContext();
            DateTime fechaLimite = ObtenerLimiteSLA();

            var query = db.Tickets.ApplySlaFilter(esUrgente, fechaLimite);

            return mapper.Map<List<TicketDto>>(await query
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync());
        }

        // Obtener tickets por estatus - Async
        public async Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.Status == estatus)
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync());
        }

        // Contar tickets por SLA - Async
        public async Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente)
        {
            using var db = dbContextFactory.CreateDbContext();
            DateTime fechaLimite = ObtenerLimiteSLA();

            return await db.Tickets.ApplySlaFilter(esUrgente, fechaLimite).CountAsync();
        }

        // Contar tickets por estatus - Async
        public async Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.Tickets.CountAsync(t => t.Status == estatus);
        }

        // Obtener historial de cambios de un ticket - Async
        public async Task<List<HistorialCambiosDto>> ObtenerHistorialPorTicketAsync(int idTicket)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.HistorialCambiosTickets
                .Include(h => h.IdUsuarioCambioNavigation)
                .Where(h => h.IdTicket == idTicket)
                .OrderByDescending(h => h.FechaMovimiento)
                .Select(h => new HistorialCambiosDto
                {
                    IdTicket = h.IdTicket,
                    UsuarioCambio = h.IdUsuarioCambioNavigation.Nombre ?? "-",
                    FechaMovimiento = h.FechaMovimiento,
                    CampoModificado = h.CampoModificado,
                    ValorAnterior = h.ValorAnterior ?? "-",
                    ValorNuevo = h.ValorNuevo ?? "-"
                })
                .ToListAsync();
        }

        // Actualizar ticket
        public async Task ActualizarTicketAsync(TicketUpdateDto ticketDto)
        {
            var validacionResult = await updateValidator.ValidateAsync(ticketDto);
            if (!validacionResult.IsValid) throw new FluentValidation.ValidationException(validacionResult.Errors);

            using var db = dbContextFactory.CreateDbContext();

            var ticketTracked = await db.Tickets.FindAsync(ticketDto.IdTicket)
                ?? throw new KeyNotFoundException("El ticket no existe o ya fue eliminado.");

            var estatusAnterior = ticketTracked.Status;

            mapper.Map(ticketDto, ticketTracked);
            await db.SaveChangesAsync();

            // Guardar notificación en base de datos si el estatus cambió
            if (estatusAnterior != ticketTracked.Status)
            {
                var notificacion = new Notificacion
                {
                    UsuarioDestinoId = ticketTracked.IdUsuario,
                    Mensaje = $"El ticket TK-{ticketTracked.IdTicket:d6} ha cambiado al estatus: {ticketTracked.Status}.",
                    Tipo = "EstadoTicket",
                    FechaCreacion = DateTime.Now,
                    Leido = false
                };
                db.Notificaciones.Add(notificacion);
                await db.SaveChangesAsync();

                if (notificationClient != null)
                {
                    _ = notificationClient.NotifyTicketStatusChangedAsync(
                        ticketTracked.IdUsuario,
                        ticketTracked.IdTicket,
                        ticketTracked.IdTicket.ToString("d6"),
                        ticketTracked.Status ?? string.Empty
                    );
                }
            }
        }

        // Obtener tickets por usuario - Async
        public async Task<List<TicketDto>> ObtenerTicketsPorUsuarioAsync(int idUsuario)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.IdUsuario == idUsuario)
                .Include(t => t.IdTecnicoNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync());
        }

        // Obtener tickets asignados a un técnico (no cerrados) - Async
        public async Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Status != ConstantesEstatus.CERRADO)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync());
        }

        // Obtener tickets cerrados por un técnico - Async
        public async Task<List<TicketDto>> ObtenerTicketsCerradosPorTecnicoAsync(int idTecnico)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Status == ConstantesEstatus.CERRADO)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.Cierre)
                .ToListAsync());
        }

        // Obtener tickets disponibles (abiertos sin técnico asignado) - Async
        public async Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.Status == ConstantesEstatus.ABIERTO && t.IdTecnico == null)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync());
        }

        // Crear un nuevo ticket - Async
        public async Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto)
        {
            var validacionResult = await createValidator.ValidateAsync(ticketDto);
            if (!validacionResult.IsValid) throw new FluentValidation.ValidationException(validacionResult.Errors);

            using var db = dbContextFactory.CreateDbContext();

            var nuevoTicket = mapper.Map<Ticket>(ticketDto);
            nuevoTicket.Alta = DateTime.Now;
            if (nuevoTicket.IdTecnico.HasValue && nuevoTicket.IdTecnico.Value > 0)
            {
                nuevoTicket.Status = ConstantesEstatus.EN_PROCESO;
                nuevoTicket.Atención = DateTime.Now;
            }
            else
            {
                nuevoTicket.Status = ConstantesEstatus.ABIERTO;
            }

            db.Tickets.Add(nuevoTicket);
            await db.SaveChangesAsync();

            return mapper.Map<TicketDto>(nuevoTicket);
        }

        // Lógica de dominio: Transiciones de estatus permitidas (SRP)
        public static List<string> ObtenerEstatusPermitidos(int idRolUsuario, string estatusActual)
        {
            if (idRolUsuario == 1) // Admin
            {
                return [ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO, ConstantesEstatus.REABIERTO];
            }

            return estatusActual switch
            {
                ConstantesEstatus.ABIERTO => [ConstantesEstatus.ABIERTO, ConstantesEstatus.EN_PROCESO],
                ConstantesEstatus.EN_PROCESO => [ConstantesEstatus.EN_PROCESO, ConstantesEstatus.CERRADO],
                ConstantesEstatus.CERRADO => [ConstantesEstatus.CERRADO],
                ConstantesEstatus.REABIERTO => [ConstantesEstatus.REABIERTO, ConstantesEstatus.EN_PROCESO],
                _ => [estatusActual]
            };
        }

        // Obtener tickets filtrados dinámicamente - Async
        public async Task<List<TicketDto>> ObtenerTicketsFiltradosAsync(TicketFilterDto filtros)
        {
            using var db = dbContextFactory.CreateDbContext();
            var query = db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ApplyFilters(filtros);

            return mapper.Map<List<TicketDto>>(await query.OrderByDescending(t => t.Alta).ToListAsync());
        }

        // Obtener tickets filtrados y paginados dinámicamente - Async
        public async Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize)
        {
            using var db = dbContextFactory.CreateDbContext();
            var query = db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ApplyFilters(filtros);

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(t => t.Alta)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PaginatedResultDto<TicketDto>
            {
                Items = mapper.Map<List<TicketDto>>(items),
                TotalCount = totalCount
            };
        }

        // Obtener DTO de KPIs y Analítica de Reportes - Async
        public async Task<ReporteKpisDto> ObtenerReporteKpisAsync(DateTime inicio, DateTime fin)
        {
            using var db = dbContextFactory.CreateDbContext();

            // Forzamos fin a las 23:59:59 del día seleccionado para incluir todos sus registros
            var finDelDia = fin.Date.AddDays(1).AddTicks(-1);
            var inicioDelDia = inicio.Date;

            var tickets = await db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                    .ThenInclude(u => u.IdDepartamentoNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .Where(t => t.Alta >= inicioDelDia && t.Alta <= finDelDia)
                .ToListAsync();

            var totalCreados = tickets.Count;
            var totalResueltos = tickets.Count(t => t.Status == ConstantesEstatus.CERRADO);
            var tasaCierre = totalCreados > 0 ? (double)totalResueltos / totalCreados * 100 : 0;

            var attendedTickets = tickets.Where(t => t.Atención.HasValue && t.Alta.HasValue).ToList();
            double tiempoPromedio = attendedTickets.Count > 0
                ? attendedTickets.Average(t => (t.Atención!.Value - t.Alta!.Value).TotalHours)
                : 0;

            // Productividad Técnica
            var productividad = tickets
                .GroupBy(t => t.IdTecnico)
                .Select(g =>
                {
                    var tecnicoNombre = g.First().IdTecnicoNavigation?.Nombre ?? "Sin Asignar";
                    var asignados = g.Count();
                    var resueltos = g.Count(t => t.Status == ConstantesEstatus.CERRADO);
                    return new PersonalProductividadDto
                    {
                        Tecnico = tecnicoNombre,
                        TicketsAsignados = asignados,
                        TicketsResueltos = resueltos,
                        TasaCierre = asignados > 0 ? (double)resueltos / asignados * 100 : 0
                    };
                })
                .OrderByDescending(p => p.TicketsResueltos)
                .ToList();

            // Demanda por Usuario (Top 10)
            var demandaUsuarios = tickets
                .GroupBy(t => t.IdUsuario)
                .Select(g =>
                {
                    var usuarioNombre = g.First().IdUsuarioNavigation?.Nombre ?? "Desconocido";
                    var creados = g.Count();
                    var resueltos = g.Count(t => t.Status == ConstantesEstatus.CERRADO);
                    return new UsuarioDemandaDto
                    {
                        Usuario = usuarioNombre,
                        TicketsCreados = creados,
                        TicketsResueltos = resueltos,
                        TicketsPendientes = creados - resueltos
                    };
                })
                .OrderByDescending(d => d.TicketsCreados)
                .Take(10)
                .ToList();

            // Demanda por Departamento
            var demandaDepartamentos = tickets
                .GroupBy(t => t.IdUsuarioNavigation?.IdDepartamentoNavigation?.Nombre ?? "Sin Departamento")
                .Select(g =>
                {
                    var dptoNombre = g.Key;
                    var total = g.Count();
                    var altaPrioridad = g.Count(t => t.Prioridad == "Alta" || t.Prioridad == "Urgente");
                    return new DepartamentoMetricaDto
                    {
                        Departamento = dptoNombre,
                        TotalTickets = total,
                        AltaPrioridad = altaPrioridad,
                        PorcentajeDelTotal = totalCreados > 0 ? (double)total / totalCreados * 100 : 0
                    };
                })
                .OrderByDescending(d => d.TotalTickets)
                .ToList();

            // Análisis Temporal
            var totalDias = (finDelDia - inicioDelDia).TotalDays;
            var groupedTemporal = totalDias <= 31
                ? tickets.Where(t => t.Alta.HasValue).GroupBy(t => t.Alta!.Value.ToString("dd/MM/yyyy"))
                : tickets.Where(t => t.Alta.HasValue).GroupBy(t => t.Alta!.Value.ToString("yyyy-MM (MMMM)"));

            var analisisTemporal = groupedTemporal
                .Select(g => new AnalisisTemporalDto
                {
                    Periodo = g.Key,
                    Cantidad = g.Count()
                })
                .OrderBy(a => a.Periodo)
                .ToList();

            return new ReporteKpisDto
            {
                TotalCreados = totalCreados,
                TotalResueltos = totalResueltos,
                TasaCierre = Math.Round(tasaCierre, 2),
                TiempoPromedioAtencionHoras = Math.Round(tiempoPromedio, 2),
                ProductividadTecnica = productividad,
                DemandaUsuarios = demandaUsuarios,
                DemandaDepartamentos = demandaDepartamentos,
                AnalisisTemporal = analisisTemporal
            };
        }

        // Registrar calificación del cliente para un ticket
        public async Task<bool> RegistrarCalificacionAsync(int idTicket, int calificacion, string? comentario)
        {
            if (calificacion < 1 || calificacion > 5)
                throw new ArgumentException("La calificación debe estar entre 1 y 5.");

            using var db = dbContextFactory.CreateDbContext();
            var ticket = await db.Tickets.FindAsync(idTicket);
            if (ticket == null) return false;

            if (ticket.Status != ConstantesEstatus.CERRADO)
                throw new InvalidOperationException("Solo se pueden calificar tickets que estén cerrados.");

            ticket.Calificacion = calificacion;
            ticket.ComentarioFeedback = comentario;
            ticket.FechaFeedback = DateTime.Now;

            // Guardar notificación si hay técnico asignado
            if (ticket.IdTecnico.HasValue)
            {
                var stars = new string('⭐', calificacion);
                var notificacion = new Notificacion
                {
                    UsuarioDestinoId = ticket.IdTecnico.Value,
                    Mensaje = $"El cliente calificó el ticket TK-{ticket.IdTicket:d6} con {stars} ({calificacion}/5). Comentario: \"{comentario ?? string.Empty}\"",
                    Tipo = "Calificacion",
                    FechaCreacion = DateTime.Now,
                    Leido = false
                };
                db.Notificaciones.Add(notificacion);

                if (notificationClient != null)
                {
                    _ = notificationClient.NotifyTicketRatedAsync(
                        ticket.IdTecnico.Value,
                        ticket.IdTicket,
                        ticket.IdTicket.ToString("d6"),
                        calificacion,
                        comentario ?? string.Empty
                    );
                }
            }

            await db.SaveChangesAsync();
            return true;
        }

        // Obtener promedio de calificación de un técnico
        public async Task<double> ObtenerPromedioCalificacionTecnicoAsync(int idTecnico)
        {
            using var db = dbContextFactory.CreateDbContext();
            var calificaciones = await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Calificacion.HasValue)
                .Select(t => t.Calificacion!.Value)
                .ToListAsync();

            if (calificaciones.Count == 0) return 0.0;
            return calificaciones.Average();
        }

        // Obtener la lista de calificaciones y comentarios recibidos por un técnico
        public async Task<List<TicketDto>> ObtenerFeedbackTecnicoAsync(int idTecnico)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Calificacion.HasValue)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.FechaFeedback)
                .ToListAsync());
        }
    }
}
