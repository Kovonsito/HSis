#nullable enable
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.Contracts.Coordinators
{
    public record ElementoOpcionCombo(int? Id, string Nombre);

    /// <summary>
    /// Coordinador de negocio y orquestación de datos para el Dashboard de Administración.
    /// Desacopla el formulario de la invocación múltiple de servicios backend.
    /// </summary>
    public interface IAdminDashboardCoordinator
    {
        /// <summary>
        /// Obtiene y formatea concurrentemente la lista de administradores y técnicos para el combo de asignación/filtro.
        /// </summary>
        Task<List<ElementoOpcionCombo>> ObtenerTecnicosYAdminsAsync();

        /// <summary>
        /// Obtiene el resumen consolidado de KPIs (nuevos, urgentes, en proceso, cerrados, reabiertos, calificación).
        /// </summary>
        Task<DashboardResumenDto> ObtenerResumenKPIsAsync(int idUsuario);

        /// <summary>
        /// Obtiene la calificación promedio de atención como técnico/administrador.
        /// </summary>
        Task<double> ObtenerCalificacionPromedioAsync(int idUsuario);

        /// <summary>
        /// Consulta los tickets filtrados y paginados.
        /// </summary>
        Task<PaginatedResultDto<TicketDto>> FiltrarTicketsPaginadosAsync(TicketFilterDto filtros, int pagina, int tamanoPagina);

        /// <summary>
        /// Obtiene la colección de datos para una pestaña de catálogo específico o inventario.
        /// </summary>
        Task<object?> CargarDatosCatalogoAsync(Type tipoEntidad);
    }

    public class AdminDashboardCoordinator : IAdminDashboardCoordinator
    {
        private readonly ITicketService _ticketService;
        private readonly ICatalogoService _catalogoService;
        private readonly IUsuarioService _usuarioService;
        private readonly IMaterialService _materialService;

        public AdminDashboardCoordinator(
            ITicketService ticketService,
            ICatalogoService catalogoService,
            IUsuarioService usuarioService,
            IMaterialService materialService)
        {
            _ticketService = ticketService;
            _catalogoService = catalogoService;
            _usuarioService = usuarioService;
            _materialService = materialService;
        }

        public async Task<List<ElementoOpcionCombo>> ObtenerTecnicosYAdminsAsync()
        {
            var resultado = new List<ElementoOpcionCombo>
            {
                new(0, "Todos"),
                new(-1, "Sin Asignar")
            };

            var tareaAdmins = _usuarioService.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Administrador);
            var tareaTecnicos = _usuarioService.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Tecnico);

            await Task.WhenAll(tareaAdmins, tareaTecnicos);

            var admins = await tareaAdmins ?? [];
            var tecnicos = await tareaTecnicos ?? [];

            foreach (var a in admins)
            {
                resultado.Add(new(a.IdUsuario, $"Admin - {a.Nombre}"));
            }
            foreach (var t in tecnicos)
            {
                resultado.Add(new(t.IdUsuario, $"Técnico - {t.Nombre}"));
            }

            return resultado;
        }

        public async Task<DashboardResumenDto> ObtenerResumenKPIsAsync(int idUsuario)
        {
            return await _ticketService.ObtenerResumenDashboardAsync(idUsuario) ?? new DashboardResumenDto();
        }

        public async Task<double> ObtenerCalificacionPromedioAsync(int idUsuario)
        {
            return await _ticketService.ObtenerPromedioCalificacionTecnicoAsync(idUsuario);
        }

        public async Task<PaginatedResultDto<TicketDto>> FiltrarTicketsPaginadosAsync(TicketFilterDto filtros, int pagina, int tamanoPagina)
        {
            return await _ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, pagina, tamanoPagina);
        }

        public async Task<object?> CargarDatosCatalogoAsync(Type tipoEntidad)
        {
            if (tipoEntidad == typeof(MaterialDto))
            {
                return await _materialService.ObtenerMaterialesAsync();
            }

            return await _catalogoService.ObtenerTodosPorTipoAsync(tipoEntidad);
        }
    }
}
