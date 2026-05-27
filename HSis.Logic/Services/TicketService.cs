using Mapster;
using MapsterMapper;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    public class TicketService(
        IDbContextFactory<HSisDbContext> dbContextFactory,
        IMapper mapper,
        FluentValidation.IValidator<TicketCreateDto> createValidator,
        FluentValidation.IValidator<TicketUpdateDto> updateValidator)
    {
        private DateTime ObtenerLimiteSLA() => DateTime.Now.AddHours(-48);

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

            var slaFilterQuery = esUrgente
                ? db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta < fechaLimite)
                : db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta >= fechaLimite);

            return mapper.Map<List<TicketDto>>(await slaFilterQuery
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

            var slaFilterQuery = esUrgente
                ? db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta < fechaLimite)
                : db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta >= fechaLimite);

            return await slaFilterQuery.CountAsync();
        }

        // Contar tickets por estatus - Async
        public async Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus)
        {
            using var db = dbContextFactory.CreateDbContext();
            return await db.Tickets.CountAsync(t => t.Status == estatus);
        }

        // Obtener historial de cambios de un ticket - Async
        public async Task<List<object>> ObtenerHistorialPorTicketAsync(int idTicket)
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
                .ToListAsync<object>();
        }

        // Actualizar ticket
        public async Task ActualizarTicketAsync(TicketUpdateDto ticketDto)
        {
            var validacionResult = await updateValidator.ValidateAsync(ticketDto);
            if (!validacionResult.IsValid) throw new FluentValidation.ValidationException(validacionResult.Errors);

            using var db = dbContextFactory.CreateDbContext();

            var ticketTracked = await db.Tickets.FindAsync(ticketDto.IdTicket)
                ?? throw new KeyNotFoundException("El ticket no existe o ya fue eliminado.");

            mapper.Map(ticketDto, ticketTracked);
            await db.SaveChangesAsync();
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
            nuevoTicket.Status = ConstantesEstatus.ABIERTO;

            db.Tickets.Add(nuevoTicket);
            await db.SaveChangesAsync();

            return mapper.Map<TicketDto>(nuevoTicket);
        }

        // Lógica de dominio: Transiciones de estatus permitidas (SRP)
        public List<string> ObtenerEstatusPermitidos(int idRolUsuario, string estatusActual)
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
            IQueryable<Ticket> query = db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation);

            // 1. Filtros de Texto / Identificadores
            if (!string.IsNullOrWhiteSpace(filtros.UsuarioEmisor))
            {
                query = query.Where(t => t.IdUsuarioNavigation.Nombre != null && t.IdUsuarioNavigation.Nombre.Contains(filtros.UsuarioEmisor));
            }
            if (!string.IsNullOrWhiteSpace(filtros.Estatus))
            {
                query = query.Where(t => t.Status == filtros.Estatus);
            }
            if (filtros.IdTecnico.HasValue)
            {
                query = query.Where(t => t.IdTecnico == filtros.IdTecnico.Value);
            }
            if (!string.IsNullOrWhiteSpace(filtros.Prioridad))
            {
                query = query.Where(t => t.Prioridad == filtros.Prioridad);
            }

            // 2. Rangos de Fechas Explícitos
            if (filtros.FechaAltaInicio.HasValue)
                query = query.Where(t => t.Alta >= filtros.FechaAltaInicio.Value);
            if (filtros.FechaAltaFin.HasValue)
                query = query.Where(t => t.Alta <= filtros.FechaAltaFin.Value);

            // 3. Vistas Temporales Rápidas (Día, Semana, Mes, Año)
            if (filtros.RangoTemporal.HasValue)
            {
                var hoy = DateTime.Today;
                DateTime inicio = hoy;
                DateTime fin = hoy.AddDays(1).AddTicks(-1);

                switch (filtros.RangoTemporal.Value)
                {
                    case VistaTemporal.Dia:
                        inicio = hoy;
                        break;
                    case VistaTemporal.Semana:
                        int diasAlLunes = (int)hoy.DayOfWeek - (int)DayOfWeek.Monday;
                        if (diasAlLunes < 0) diasAlLunes += 7;
                        inicio = hoy.AddDays(-diasAlLunes);
                        break;
                    case VistaTemporal.Mes:
                        inicio = new DateTime(hoy.Year, hoy.Month, 1);
                        break;
                    case VistaTemporal.Ano:
                        inicio = new DateTime(hoy.Year, 1, 1);
                        break;
                }

                if (filtros.RangoTemporal.Value != VistaTemporal.Todos)
                {
                    query = query.Where(t => t.Alta >= inicio && t.Alta <= fin);
                }
            }

            return mapper.Map<List<TicketDto>>(await query.OrderByDescending(t => t.Alta).ToListAsync());
        }

        // Obtener tickets filtrados y paginados dinámicamente - Async
        public async Task<PaginatedResultDto<TicketDto>> ObtenerTicketsFiltradosPaginadosAsync(TicketFilterDto filtros, int pageNumber, int pageSize)
        {
            using var db = dbContextFactory.CreateDbContext();
            IQueryable<Ticket> query = db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation);

            // 1. Filtros de Texto / Identificadores
            if (!string.IsNullOrWhiteSpace(filtros.UsuarioEmisor))
            {
                query = query.Where(t => t.IdUsuarioNavigation.Nombre != null && t.IdUsuarioNavigation.Nombre.Contains(filtros.UsuarioEmisor));
            }
            if (!string.IsNullOrWhiteSpace(filtros.Estatus))
            {
                query = query.Where(t => t.Status == filtros.Estatus);
            }
            if (filtros.IdTecnico.HasValue)
            {
                query = query.Where(t => t.IdTecnico == filtros.IdTecnico.Value);
            }
            if (!string.IsNullOrWhiteSpace(filtros.Prioridad))
            {
                query = query.Where(t => t.Prioridad == filtros.Prioridad);
            }

            // 2. Rangos de Fechas Explícitos
            if (filtros.FechaAltaInicio.HasValue)
                query = query.Where(t => t.Alta >= filtros.FechaAltaInicio.Value);
            if (filtros.FechaAltaFin.HasValue)
                query = query.Where(t => t.Alta <= filtros.FechaAltaFin.Value);

            // 3. Vistas Temporales Rápidas (Día, Semana, Mes, Año)
            if (filtros.RangoTemporal.HasValue)
            {
                var hoy = DateTime.Today;
                DateTime inicio = hoy;
                DateTime fin = hoy.AddDays(1).AddTicks(-1);

                switch (filtros.RangoTemporal.Value)
                {
                    case VistaTemporal.Dia:
                        inicio = hoy;
                        break;
                    case VistaTemporal.Semana:
                        int diasAlLunes = (int)hoy.DayOfWeek - (int)DayOfWeek.Monday;
                        if (diasAlLunes < 0) diasAlLunes += 7;
                        inicio = hoy.AddDays(-diasAlLunes);
                        break;
                    case VistaTemporal.Mes:
                        inicio = new DateTime(hoy.Year, hoy.Month, 1);
                        break;
                    case VistaTemporal.Ano:
                        inicio = new DateTime(hoy.Year, 1, 1);
                        break;
                }

                if (filtros.RangoTemporal.Value != VistaTemporal.Todos)
                {
                    query = query.Where(t => t.Alta >= inicio && t.Alta <= fin);
                }
            }

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
            double tiempoPromedio = attendedTickets.Any()
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
    }
}
