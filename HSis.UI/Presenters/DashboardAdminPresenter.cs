using HSis.Data.Models;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class DashboardAdminPresenter(
        ITicketService ticketService,
        ICatalogoService catalogoService,
        IUsuarioService usuarioService)
    {
        private IDashboardAdminView? _view;

        public void SetView(IDashboardAdminView view)
        {
            _view = view;
        }

        public async Task CargarKPIsAsync(int idUsuario)
        {
            if (_view == null) return;
            try
            {
                var taskNuevos = ticketService.ObtenerCountTicketsPorSLAAsync(false);
                var taskUrgentes = ticketService.ObtenerCountTicketsPorSLAAsync(true);
                var taskEnProceso = ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.EN_PROCESO);
                var taskCerrados = ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.CERRADO);
                var taskReabiertos = ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.REABIERTO);
                var taskCalificacion = ticketService.ObtenerPromedioCalificacionTecnicoAsync(idUsuario);

                await Task.WhenAll(taskNuevos, taskUrgentes, taskEnProceso, taskCerrados, taskReabiertos, taskCalificacion);

                _view.MostrarKPIs(
                    taskNuevos.Result,
                    taskUrgentes.Result,
                    taskEnProceso.Result,
                    taskCerrados.Result,
                    taskReabiertos.Result,
                    taskCalificacion.Result
                );
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar KPIs de administración: {ex.Message}");
            }
        }

        public async Task CargarCombosFiltrosAsync()
        {
            if (_view == null) return;
            try
            {
                var admins = await usuarioService.ObtenerUsuariosPorRolAsync(1);
                var tecnicos = await usuarioService.ObtenerUsuariosPorRolAsync(2);
                _view.CargarCombosFiltros(admins, tecnicos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar filtros de técnicos y administradores: {ex.Message}");
                _view.CargarCombosFiltros([], []);
            }
        }

        public async Task FiltrarTicketsAsync(TicketFilterDto filtros, int pagina, int tamanoPagina)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var resultado = await ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, pagina, tamanoPagina);
                _view.MostrarTickets(resultado.Items, resultado.TotalCount);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al obtener listado de tickets: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task<object?> ObtenerDatosCatalogoAsync(Type tipoEntidad)
        {
            var miMetodo = typeof(ICatalogoService).GetMethod(nameof(ICatalogoService.ObtenerTodosAsync))!.MakeGenericMethod(tipoEntidad);
            Task task = (Task)miMetodo.Invoke(catalogoService, null)!;
            await task;
            var resultProp = task.GetType().GetProperty("Result");
            return resultProp?.GetValue(task);
        }

        public async Task CrearEntidadCatalogoAsync(Type tipo, object entidad)
        {
            var miMetodo = typeof(ICatalogoService).GetMethod(nameof(ICatalogoService.CrearAsync))!.MakeGenericMethod(tipo);
            Task task = (Task)miMetodo.Invoke(catalogoService, [entidad])!;
            await task;
        }

        public async Task ActualizarEntidadCatalogoAsync(Type tipo, object entidad)
        {
            var miMetodo = typeof(ICatalogoService).GetMethod(nameof(ICatalogoService.ActualizarAsync))!.MakeGenericMethod(tipo);
            Task task = (Task)miMetodo.Invoke(catalogoService, [entidad])!;
            await task;
        }

        public async Task EliminarEntidadCatalogoAsync(Type tipo, object id)
        {
            var miMetodo = typeof(ICatalogoService).GetMethod(nameof(ICatalogoService.EliminarAsync))!.MakeGenericMethod(tipo);
            Task task = (Task)miMetodo.Invoke(catalogoService, [id])!;
            await task;
        }

        public async Task CrearMovimientoMaterialAsync(MovimientoMaterial mov)
        {
            await catalogoService.CrearAsync(mov);
        }

        public async Task<double> ObtenerPromedioCalificacionAsync(int idUsuario)
        {
            return await ticketService.ObtenerPromedioCalificacionTecnicoAsync(idUsuario);
        }
    }
}

