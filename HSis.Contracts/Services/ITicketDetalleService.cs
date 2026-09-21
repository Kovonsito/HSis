using HSis.Logic.DTOs;

namespace HSis.Logic.Services
{
    public interface ITicketDetalleService
    {
        Task<List<TicketDetalleDto>> ObtenerDetallesTicketAsync(int idTicket);
        Task<TicketDetalleDto?> ObtenerDetallePorIdAsync(int idTicket, int idMaterial);
        Task AgregarMaterialATicketAsync(TicketDetalleDto detalleDto);
        Task ActualizarDetalleTicketAsync(TicketDetalleDto detalleDto);
        Task EliminarMaterialDeTicketAsync(int idTicket, int idMaterial);
        Task<decimal> ObtenerCostoTotalMaterialesTicketAsync(int idTicket);
    }
}

