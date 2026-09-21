using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class DashboardAdminPresenter(
        ITicketService ticketService,
        ICatalogoService catalogoService,
        IUsuarioService usuarioService,
        IMaterialService materialService,
        IContextoSesion contextoSesion)
    {
        private IDashboardAdminView? _view;

        public void SetView(IDashboardAdminView view)
        {
            _view = view;
        }

        public async Task CargarKPIsAsync(int? idUsuario = null)
        {
            if (_view == null) return;
            try
            {
                int targetUser = idUsuario ?? contextoSesion.IdUsuario;
                var taskNuevos = ticketService.ObtenerCountTicketsPorSLAAsync(false);
                var taskUrgentes = ticketService.ObtenerCountTicketsPorSLAAsync(true);
                var taskEnProceso = ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.EN_PROCESO);
                var taskCerrados = ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.CERRADO);
                var taskReabiertos = ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.REABIERTO);
                var taskCalificacion = ticketService.ObtenerPromedioCalificacionTecnicoAsync(targetUser);

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
                var admins = await usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Administrador);
                var tecnicos = await usuarioService.ObtenerUsuariosPorRolAsync((int)RolUsuarioEnum.Tecnico);
                _view.CargarCombosFiltros(admins, tecnicos);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar filtros de técnicos y administradores: {ex.Message}");
            }
        }

        public async Task FiltrarTicketsAsync(TicketFilterDto filtro, int pagina, int tamanoPagina)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var resultado = await ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtro, pagina, tamanoPagina);
                _view.MostrarTickets(resultado.Items, resultado.TotalCount);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al filtrar tickets: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task<object?> ObtenerDatosCatalogoAsync(Type tipoEntidad)
        {
            if (tipoEntidad == typeof(MaterialDto))
            {
                return await materialService.ObtenerMaterialesAsync();
            }
            return await catalogoService.ObtenerTodosPorTipoAsync(tipoEntidad);
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

        public async Task CrearMovimientoMaterialAsync(KardexMovimientoDto mov)
        {
            await materialService.RegistrarMovimientoAsync(mov);
        }

        public async Task<double> ObtenerPromedioCalificacionAsync(int? idUsuario = null)
        {
            return await ticketService.ObtenerPromedioCalificacionTecnicoAsync(idUsuario ?? contextoSesion.IdUsuario);
        }
    }
}
