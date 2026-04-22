using HSis.Data.Models;
using HSis.Logic.DTOs;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    public class TicketService
    {
        private readonly IDbContextFactory<HSisDbContext> _dbContextFactory;
        private readonly AutoMapper.IMapper _mapper;
        private readonly FluentValidation.IValidator<HSis.Logic.DTOs.TicketCreateDto> _createValidator;
        private readonly FluentValidation.IValidator<HSis.Logic.DTOs.TicketUpdateDto> _updateValidator;

        public TicketService(
            IDbContextFactory<HSisDbContext> dbContextFactory, 
            AutoMapper.IMapper mapper,
            FluentValidation.IValidator<HSis.Logic.DTOs.TicketCreateDto> createValidator,
            FluentValidation.IValidator<HSis.Logic.DTOs.TicketUpdateDto> updateValidator)
        {
            _dbContextFactory = dbContextFactory;
            _mapper = mapper;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        // Obtener todos los tickets - Async
        public async Task<List<TicketDto>> ObtenerTicketsAsync()
        {
            using var db = _dbContextFactory.CreateDbContext();
            var tickets = await db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync();
            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // Obtener un ticket por su id - Async
        public async Task<TicketDto?> ObtenerTicketPorIdAsync(int id)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var ticket = await db.Tickets
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .FirstOrDefaultAsync(t => t.IdTicket == id);
            return _mapper.Map<TicketDto>(ticket);
        }

        // Helper method para centralizar la configuración del SLA (DRY)
        private DateTime ObtenerLimiteSLA()
        {
            return DateTime.Now.AddHours(-48);
        }

        // Obtener tickets filtrados por SLA (urgentes/no urgentes) - Async
        public async Task<List<TicketDto>> ObtenerTicketsPorSLAAsync(bool esUrgente)
        {
            using var db = _dbContextFactory.CreateDbContext();
            DateTime limite = ObtenerLimiteSLA();

            var query = esUrgente
                ? db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta < limite)
                : db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta >= limite);

            var tickets = await query
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync();
            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // Obtener tickets por estatus - Async
        public async Task<List<TicketDto>> ObtenerTicketsPorEstatusAsync(string estatus)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var tickets = await db.Tickets
                .Where(t => t.Status == estatus)
                .Include(t => t.IdUsuarioNavigation)
                .Include(t => t.IdTecnicoNavigation)
                .ToListAsync();
            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // Contar tickets por SLA - Async
        public async Task<int> ObtenerCountTicketsPorSLAAsync(bool esUrgente)
        {
            using var db = _dbContextFactory.CreateDbContext();
            DateTime limite = ObtenerLimiteSLA();

            var query = esUrgente
                ? db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta < limite)
                : db.Tickets.Where(t => t.Status == ConstantesEstatus.ABIERTO && t.Alta >= limite);

            return await query.CountAsync();
        }

        // Contar tickets por estatus - Async
        public async Task<int> ObtenerCountTicketsPorEstatusAsync(string estatus)
        {
            using var db = _dbContextFactory.CreateDbContext();
            return await db.Tickets.CountAsync(t => t.Status == estatus);
        }

        public async Task<List<object>> ObtenerHistorialPorTicketAsync(int idTicket)
        {
            using var db = _dbContextFactory.CreateDbContext();

            // Filtramos por el ticket actual y ordenamos del más reciente al más antiguo
            var historial = await db.HistorialCambiosTickets
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

            return historial;
        }

        // Actualizar ticket
        public async Task ActualizarTicketAsync(TicketUpdateDto ticketDto)
        {
            var result = await _updateValidator.ValidateAsync(ticketDto);
            if (!result.IsValid) throw new FluentValidation.ValidationException(result.Errors);

            using var db = _dbContextFactory.CreateDbContext();

            var ticketTracked = await db.Tickets.FindAsync(ticketDto.IdTicket);
            if (ticketTracked != null)
            {
                // Mapeamos los cambios del DTO a la entidad trackeada
                _mapper.Map(ticketDto, ticketTracked);
                await db.SaveChangesAsync();
            }
            else
            {
                throw new Exception("El ticket no existe o ya fue eliminado.");
            }
        }

        // Obtener tickets por usuario - Async
        public async Task<List<TicketDto>> ObtenerTicketsPorUsuarioAsync(int idUsuario)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var tickets = await db.Tickets
                .Where(t => t.IdUsuario == idUsuario)
                .Include(t => t.IdTecnicoNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync();
            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // Obtener tickets asignados a un técnico (no cerrados) - Async
        public async Task<List<TicketDto>> ObtenerTicketsAsignadosATecnicoAsync(int idTecnico)
        {
            using var db = _dbContextFactory.CreateDbContext();
            var tickets = await db.Tickets
                .Where(t => t.IdTecnico == idTecnico && t.Status != ConstantesEstatus.CERRADO)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync();
            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // Obtener tickets disponibles (abiertos sin técnico asignado) - Async
        public async Task<List<TicketDto>> ObtenerTicketsDisponiblesAsync()
        {
            using var db = _dbContextFactory.CreateDbContext();
            var tickets = await db.Tickets
                .Where(t => t.Status == ConstantesEstatus.ABIERTO && t.IdTecnico == null)
                .Include(t => t.IdUsuarioNavigation)
                .OrderByDescending(t => t.Alta)
                .ToListAsync();
            return _mapper.Map<List<TicketDto>>(tickets);
        }

        // Crear un nuevo ticket - Async
        public async Task<TicketDto> CrearTicketAsync(TicketCreateDto ticketDto)
        {
            var result = await _createValidator.ValidateAsync(ticketDto);
            if (!result.IsValid) throw new FluentValidation.ValidationException(result.Errors);

            using var db = _dbContextFactory.CreateDbContext();
            
            var ticket = _mapper.Map<Ticket>(ticketDto);
            ticket.Alta = DateTime.Now;
            ticket.Status = ConstantesEstatus.ABIERTO;

            db.Tickets.Add(ticket);
            await db.SaveChangesAsync();
            
            return _mapper.Map<TicketDto>(ticket);
        }



        // Lógica de dominio: Transiciones de estatus permitidas (SRP)
        public List<string> ObtenerEstatusPermitidos(int idRolUsuario, string estatusActual)
        {
            var estatusPermitidos = new List<string>();
            bool esAdmin = idRolUsuario == 1;

            if (esAdmin)
            {
                estatusPermitidos.Add(ConstantesEstatus.ABIERTO);
                estatusPermitidos.Add(ConstantesEstatus.EN_PROCESO);
                estatusPermitidos.Add(ConstantesEstatus.CERRADO);
                estatusPermitidos.Add(ConstantesEstatus.REABIERTO);
                return estatusPermitidos;
            }

            if (estatusActual == ConstantesEstatus.ABIERTO)
            {
                estatusPermitidos.Add(ConstantesEstatus.ABIERTO);
                estatusPermitidos.Add(ConstantesEstatus.EN_PROCESO);
            }
            else if (estatusActual == ConstantesEstatus.EN_PROCESO)
            {
                estatusPermitidos.Add(ConstantesEstatus.EN_PROCESO);
                estatusPermitidos.Add(ConstantesEstatus.CERRADO);
            }
            else if (estatusActual == ConstantesEstatus.CERRADO)
            {
                estatusPermitidos.Add(ConstantesEstatus.CERRADO);
            }
            else if (estatusActual == ConstantesEstatus.REABIERTO)
            {
                estatusPermitidos.Add(ConstantesEstatus.REABIERTO);
                estatusPermitidos.Add(ConstantesEstatus.EN_PROCESO);
            }
            else
            {
                // Fallback por si el estatus no existe en la regla
                estatusPermitidos.Add(estatusActual);
            }

            return estatusPermitidos;
        }
    }
}
