using HSis.Data.Models;
using HSis.Logic.Constants;
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
        INotificadorTicket? notifier = null,
        INotificationClientService? notificationClient = null,
        IServerNotificationDispatcher? notificationDispatcher = null) : ITicketService
    {
        private readonly IDbContextFactory<HSisDbContext> dbContextFactory = dbContextFactory;
        private readonly IMapper mapper = mapper;
        private readonly FluentValidation.IValidator<TicketCreateDto> createValidator = createValidator;
        private readonly FluentValidation.IValidator<TicketUpdateDto> updateValidator = updateValidator;
        private readonly INotificadorTicket? notifier = notifier ?? notificationDispatcher as INotificadorTicket ?? notificationClient;
        private readonly INotificationClientService? notificationClient = notificationClient;
        private readonly IServerNotificationDispatcher? notificationDispatcher = notificationDispatcher;

        private static DateTime ObtenerLimiteSLA()
        {
            return DateTime.Now.AddHours(-48);
        }

        // Obtener todos los tickets - Async

        public async Task<List<TicketDto>> ObtenerTicketsAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Include(t => t.Usuario)
                .Include(t => t.Tecnico)
                .ToListAsync());
        }

        // Obtener un ticket por su id - Async
        public async Task<TicketDto?> ObtenerTicketPorIdAsync(int id)
        {
            using var db = dbContextFactory.CreateDbContext();
            var ticket = await db.Tickets
                .Include(t => t.Usuario)
                .ThenInclude(u => u.Departamento)
                .Include(t => t.Tecnico)
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
                .Include(t => t.Usuario)
                .Include(t => t.Tecnico)
                .ToListAsync());
        }

        // Obtener tickets por estatus - Async
        public async Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.Estatus == estatus)
                .Include(t => t.Usuario)
                .Include(t => t.Tecnico)
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
            return await db.Tickets.CountAsync(t => t.Estatus == estatus);
        }

        // Obtener historial de cambios de un ticket - Async
        public async Task<List<HistorialCambiosDto>> ObtenerHistorialPorTicketAsync(int idTicket)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.HistorialCambiosTickets
                .Include(h => h.UsuarioCambio)
                .Where(h => h.IdTicket == idTicket)
                .OrderByDescending(h => h.FechaMovimiento)
                .Select(h => new HistorialCambiosDto
                {
                    IdTicket = h.IdTicket,
                    UsuarioCambio = h.UsuarioCambio.Nombre ?? "-",
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

            var estatusAnterior = ticketTracked.Estatus;

            mapper.Map(ticketDto, ticketTracked);
            await db.SaveChangesAsync();

            // Guardar notificación en base de datos si el estatus cambió
            if (estatusAnterior != ticketTracked.Estatus)
            {
                var notificacion = new Notificacion
                {
                    UsuarioDestinoId = ticketTracked.IdUsuario,
                    Mensaje = $"El ticket TK-{ticketTracked.IdTicket:d6} ha cambiado al estatus: {ticketTracked.Estatus}.",
                    Tipo = "EstadoTicket",
                    FechaCreacion = DateTime.Now,
                    Leido = false
                };
                db.Notificaciones.Add(notificacion);
                await db.SaveChangesAsync();

                if (notificationDispatcher != null)
                {
                    _ = notificationDispatcher.NotifyTicketStatusChangedAsync(
                        ticketTracked.IdUsuario,
                        ticketTracked.IdTicket,
                        ticketTracked.IdTicket.ToString("d6"),
                        ticketTracked.Estatus ?? string.Empty
                    );
                }
                else if (notificationClient != null)
                {
                    _ = notificationClient.NotificarCambioEstatusTicketAsync(
                        ticketTracked.IdUsuario,
                        ticketTracked.IdTicket,
                        ticketTracked.IdTicket.ToString("d6"),
                        ticketTracked.Estatus ?? string.Empty
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
                .Include(t => t.Tecnico)
                .OrderByDescending(t => t.FechaAlta)
                .ToListAsync());
        }

        // Obtener tickets asignados a un técnico (no cerrados) - Async
        public async Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Estatus != ConstantesEstatus.CERRADO)
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.FechaAlta)
                .ToListAsync());
        }

        // Obtener tickets cerrados por un técnico - Async
        public async Task<List<TicketDto>> ObtenerTicketsCerradosPorTecnicoAsync(int idTecnico)
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Estatus == ConstantesEstatus.CERRADO)
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.FechaCierre)
                .ToListAsync());
        }

        // Obtener tickets disponibles (abiertos sin técnico asignado) - Async
        public async Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync()
        {
            using var db = dbContextFactory.CreateDbContext();
            return mapper.Map<List<TicketDto>>(await db.Tickets
                .Where(t => t.Estatus == ConstantesEstatus.ABIERTO && t.IdTecnico == null)
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.FechaAlta)
                .ToListAsync());
        }

        // Crear un nuevo ticket - Async
        public async Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto)
        {
            var validacionResult = await createValidator.ValidateAsync(ticketDto);
            if (!validacionResult.IsValid) throw new FluentValidation.ValidationException(validacionResult.Errors);

            using var db = dbContextFactory.CreateDbContext();

            var nuevoTicket = mapper.Map<Ticket>(ticketDto);
            nuevoTicket.FechaAlta = DateTime.Now;
            if (nuevoTicket.IdTecnico.HasValue && nuevoTicket.IdTecnico.Value > 0)
            {
                nuevoTicket.Estatus = ConstantesEstatus.EN_PROCESO;
                nuevoTicket.FechaAtencion = DateTime.Now;
            }
            else
            {
                nuevoTicket.Estatus = ConstantesEstatus.ABIERTO;
            }

            db.Tickets.Add(nuevoTicket);
            await db.SaveChangesAsync();

            if (notifier != null)
            {
                _ = notifier.NotificarTicketCreadoAsync(
                    nuevoTicket.IdTicket,
                    nuevoTicket.IdTicket.ToString("d6"),
                    nuevoTicket.Descripcion ?? string.Empty
                );
            }

            return mapper.Map<TicketDto>(nuevoTicket);
        }

        // Lógica de dominio: Transiciones de estatus permitidas (SRP)
        public static List<string> ObtenerEstatusPermitidos(int idRolUsuario, string estatusActual)
        {
            if (idRolUsuario == (int)RolUsuarioEnum.Administrador)
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
                .Include(t => t.Usuario)
                .Include(t => t.Tecnico)
                .ApplyFilters(filtros);

            return mapper.Map<List<TicketDto>>(await query.OrderByDescending(t => t.FechaAlta).ToListAsync());
        }

        // Obtener tickets filtrados y paginados dinámicamente - Async
        public async Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize)
        {
            using var db = dbContextFactory.CreateDbContext();
            var query = db.Tickets
                .Include(t => t.Usuario)
                .Include(t => t.Tecnico)
                .ApplyFilters(filtros);

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(t => t.FechaAlta)
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
                .Include(t => t.Usuario)
                    .ThenInclude(u => u.Departamento)
                .Include(t => t.Tecnico)
                .Where(t => t.FechaAlta >= inicioDelDia && t.FechaAlta <= finDelDia)
                .ToListAsync();

            var totalCreados = tickets.Count;
            var totalResueltos = tickets.Count(t => t.Estatus == ConstantesEstatus.CERRADO);
            var tasaCierre = totalCreados > 0 ? (double)totalResueltos / totalCreados * 100 : 0;

            var attendedTickets = tickets.Where(t => t.FechaAtencion.HasValue && t.FechaAlta.HasValue).ToList();
            double tiempoPromedio = attendedTickets.Count > 0
                ? attendedTickets.Average(t => (t.FechaAtencion!.Value - t.FechaAlta!.Value).TotalHours)
                : 0;

            // Productividad Técnica
            var productividad = tickets
                .GroupBy(t => t.IdTecnico)
                .Select(g =>
                {
                    var tecnicoNombre = g.First().Tecnico?.Nombre ?? "Sin Asignar";
                    var asignados = g.Count();
                    var resueltos = g.Count(t => t.Estatus == ConstantesEstatus.CERRADO);
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
                    var usuarioNombre = g.First().Usuario?.Nombre ?? "Desconocido";
                    var creados = g.Count();
                    var resueltos = g.Count(t => t.Estatus == ConstantesEstatus.CERRADO);
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
                .GroupBy(t => t.Usuario?.Departamento?.Nombre ?? "Sin Departamento")
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
                ? tickets.Where(t => t.FechaAlta.HasValue).GroupBy(t => t.FechaAlta!.Value.ToString("dd/MM/yyyy"))
                : tickets.Where(t => t.FechaAlta.HasValue).GroupBy(t => t.FechaAlta!.Value.ToString("yyyy-MM (MMMM)"));

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

            if (ticket.Estatus != ConstantesEstatus.CERRADO)
                throw new InvalidOperationException("Solo se pueden calificar tickets que estén cerrados.");

            ticket.Calificacion = calificacion;
            ticket.ComentarioEvaluacion = comentario;
            ticket.FechaEvaluacion = DateTime.Now;

            // Notificar la calificación vía SignalR (a técnico si existe, o 0 para administradores)
            int idTecnicoNotif = ticket.IdTecnico ?? 0;
            var stars = new string('⭐', calificacion);
            var notificacion = new Notificacion
            {
                UsuarioDestinoId = idTecnicoNotif > 0 ? idTecnicoNotif : 1, // Admin si no hay técnico
                Mensaje = $"El cliente calificó el ticket TK-{ticket.IdTicket:d6} con {stars} ({calificacion}/5). Comentario: \"{comentario ?? string.Empty}\"",
                Tipo = "Calificacion",
                FechaCreacion = DateTime.Now,
                Leido = false
            };
            db.Notificaciones.Add(notificacion);

            if (notificationDispatcher != null)
            {
                _ = notificationDispatcher.NotifyTicketRatedAsync(
                    idTecnicoNotif,
                    ticket.IdTicket,
                    ticket.IdTicket.ToString("d6"),
                    calificacion,
                    comentario ?? string.Empty
                );
            }
            else if (notificationClient != null)
            {
                _ = notificationClient.NotificarCalificacionTicketAsync(
                    idTecnicoNotif,
                    ticket.IdTicket,
                    ticket.IdTicket.ToString("d6"),
                    calificacion,
                    comentario ?? string.Empty
                );
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
                .Include(t => t.Usuario)
                .OrderByDescending(t => t.FechaEvaluacion)
                .ToListAsync());
        }
    }
}

