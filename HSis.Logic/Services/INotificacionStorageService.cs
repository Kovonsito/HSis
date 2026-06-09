using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HSis.Logic.Services
{
    public interface INotificacionStorageService
    {
        Task<List<NotificacionLocal>> ObtenerNotificacionesAsync(int userId);
        Task GuardarNotificacionAsync(int userId, int ticketId, string mensaje);
        Task MarcarComoLeidaAsync(int userId, Guid id);
        Task LimpiarTodasAsync(int userId);
    }
}
