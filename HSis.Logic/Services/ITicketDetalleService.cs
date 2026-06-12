using System.Collections.Generic;
using System.Threading.Tasks;
using HSis.Data.Models;

namespace HSis.Logic.Services
{
    public interface ITicketDetalleService
    {
        Task<List<DetTicket>> ObtenerDetallesTicketAsync(int idTicket);
        Task<DetTicket?> ObtenerDetallePorIdAsync(int idTicket, int idMaterial);
        Task AgregarMaterialATicketAsync(DetTicket detTicket);
        Task ActualizarDetalleTicketAsync(DetTicket detTicket);
        Task EliminarMaterialDeTicketAsync(int idTicket, int idMaterial);
        Task<decimal> ObtenerCostoTotalMaterialesTicketAsync(int idTicket);
    }
}
