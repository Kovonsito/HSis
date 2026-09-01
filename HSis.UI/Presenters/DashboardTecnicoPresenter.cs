using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class DashboardTecnicoPresenter(ITicketService ticketService)
    {
        private IDashboardTecnicoView? _view;

        public void SetView(IDashboardTecnicoView view)
        {
            _view = view;
        }

        public async Task CargarIndicadoresAsync(int idTecnico)
        {
            if (_view == null) return;
            try
            {
                var taskAsignados = ticketService.ObtenerTicketsAsignadosATecnicoAsync(idTecnico);
                var taskDisponibles = ticketService.ObtenerTicketsDisponiblesAsync();
                var taskCerrados = ticketService.ObtenerTicketsCerradosPorTecnicoAsync(idTecnico);
                var taskPromedio = ticketService.ObtenerPromedioCalificacionTecnicoAsync(idTecnico);

                await Task.WhenAll(taskAsignados, taskDisponibles, taskCerrados, taskPromedio);

                _view.MostrarIndicadores(
                    taskAsignados.Result.Count,
                    taskDisponibles.Result.Count,
                    taskCerrados.Result.Count,
                    taskPromedio.Result
                );
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar indicadores técnicos: {ex.Message}");
            }
        }

        public async Task CargarTicketsAsignadosAsync(int idTecnico)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var list = await ticketService.ObtenerTicketsAsignadosATecnicoAsync(idTecnico);
                var operativos = MapearAOperativos(list);
                _view.MostrarTickets(operativos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar tickets asignados: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task CargarTicketsDisponiblesAsync()
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var list = await ticketService.ObtenerTicketsDisponiblesAsync();
                var operativos = MapearAOperativos(list);
                _view.MostrarTickets(operativos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar tickets disponibles: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task CargarTicketsCerradosAsync(int idTecnico)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var list = await ticketService.ObtenerTicketsCerradosPorTecnicoAsync(idTecnico);
                var operativos = MapearAOperativos(list);
                _view.MostrarTickets(operativos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar tickets cerrados: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task CargarFeedbacksAsync(int idTecnico)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var feedbacks = await ticketService.ObtenerFeedbackTecnicoAsync(idTecnico);
                var dtos = feedbacks.Select(f => new FeedbackTecnicoDto
                {
                    IdTicket = f.IdTicket,
                    Calificacion = (f.Calificacion ?? 0).ToString(),
                    Comentario = f.ComentarioEvaluacion,
                    Fecha = f.FechaEvaluacion
                }).ToList();

                _view.MostrarFeedbacks(dtos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar feedbacks: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task<double> ObtenerPromedioCalificacionAsync(int idTecnico)
        {
            try
            {
                return await ticketService.ObtenerPromedioCalificacionTecnicoAsync(idTecnico);
            }
            catch (Exception ex)
            {
                _view?.MostrarError($"Error al obtener promedio de calificación: {ex.Message}");
                return 0;
            }
        }

        private static List<TicketOperativoDto> MapearAOperativos(List<TicketDto> list)
        {
            return list.Select(t => new TicketOperativoDto
            {
                IdTicket = t.IdTicket,
                Usuario = t.NombreUsuario,
                FechaAlta = t.FechaAlta,
                Status = t.Estatus,
                Descripcion = t.Descripcion,
                Prioridad = t.Prioridad
            }).ToList();
        }
    }
}

