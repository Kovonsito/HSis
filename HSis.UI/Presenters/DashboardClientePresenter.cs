using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class DashboardClientePresenter(ITicketQueryService queryService)
    {
        private IDashboardClienteView? _view;

        public void SetView(IDashboardClienteView view)
        {
            _view = view;
        }


        public async Task CargarTicketsClienteAsync(int idUsuario)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var tickets = await queryService.ObtenerTicketsPorUsuarioAsync(idUsuario);

                int activos = tickets.Count(t => t.Estatus != ConstantesEstatus.CERRADO);
                int cerrados = tickets.Count(t => t.Estatus == ConstantesEstatus.CERRADO);
                _view.MostrarIndicadores(activos, cerrados);

                var dtos = tickets.Select(t => new TicketClienteDto
                {
                    IdTicket = t.IdTicket,
                    FechaAlta = t.FechaAlta,
                    Status = t.Estatus,
                    TecnicoAsignado = t.NombreTecnico,
                    Descripcion = t.Descripcion,
                    Feedback = t.ComentarioEvaluacion
                }).ToList();

                _view.MostrarTickets(dtos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar tickets del cliente: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}
