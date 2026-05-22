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
    }
}
