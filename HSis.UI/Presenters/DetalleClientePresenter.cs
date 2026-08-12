using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class DetalleClientePresenter(ITicketService ticketService)
    {
        private IDetalleClienteView? _view;

        public void SetView(IDetalleClienteView view)
        {
            _view = view;
        }

        public async Task CargarTicketAsync(int idTicket)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var ticket = await ticketService.ObtenerTicketPorIdAsync(idTicket);
                if (ticket == null)
                {
                    _view.MostrarError("Ticket no encontrado.");
                    _view.CerrarFormulario();
                    return;
                }

                _view.MostrarTicket(ticket);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar ticket: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task RegistrarCalificacionAsync(int idTicket, int calificacion, string? comentario)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                bool exito = await ticketService.RegistrarCalificacionAsync(idTicket, calificacion, comentario);
                if (exito)
                {
                    _view.MostrarExito("¡Gracias por tu retroalimentación! La calificación fue registrada.");
                    await CargarTicketAsync(idTicket);
                }
                else
                {
                    _view.MostrarError("No se pudo registrar la calificación.");
                }
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al registrar calificación: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}
