#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Catalogos;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;

namespace HSis.UI.Coordinators;

[SupportedOSPlatform("windows")]
public class CoordinadorDashboardAdmin(
    Form formulario,
    SidebarControl sidebar,
    TopBarControl topBar,
    VistaTicketsDashboardControl vistaTickets,
    TabControl tabMain,
    Control btnNuevoTicket,
    Control btnAbrirReportes,
    ITicketService ticketService,
    IUsuarioService usuarioService,
    IMaterialService materialService,
    IDepartamentoService departamentoService,
    ISucursalService sucursalService,
    IEmpresaService empresaService,
    IPuestoService puestoService,
    IRolUsuarioService rolUsuarioService,
    IAdministradorSesionUsuario contextoSesion,
    IAlmacenamientoCredencialesLocal sessionCache,
    IFabricaFormularios formFactory,
    IClienteSignalRNotificaciones notificationClient)
    : CoordinadorDashboardBase(
        formulario,
        sidebar,
        topBar,
        vistaTickets,
        contextoSesion,
        sessionCache,
        formFactory,
        notificationClient)
{
    private readonly ITicketService _ticketService = ticketService;
    private readonly IUsuarioService _usuarioService = usuarioService;
    private readonly IMaterialService _materialService = materialService;
    private readonly IDepartamentoService _departamentoService = departamentoService;
    private readonly ISucursalService _sucursalService = sucursalService;
    private readonly IEmpresaService _empresaService = empresaService;
    private readonly IPuestoService _puestoService = puestoService;
    private readonly IRolUsuarioService _rolUsuarioService = rolUsuarioService;
    private readonly TabControl _tabMain = tabMain;
    private readonly VistaCatalogoAdminControl _vistaMateriales = new("Inventario");
    private readonly VistaCatalogoAdminControl _vistaUsuarios = new("Usuarios");
    private readonly VistaCatalogoAdminControl _vistaDepartamentos = new("Departamentos");
    private readonly VistaCatalogoAdminControl _vistaSucursales = new("Sucursales");
    private readonly VistaCatalogoAdminControl _vistaEmpresas = new("Empresas");
    private readonly VistaCatalogoAdminControl _vistaPuestos = new("Puestos");
    private readonly VistaCatalogoAdminControl _vistaRoles = new("Roles");
    private readonly Dictionary<string, TabPage> _paginasCatalogos = [];
    private readonly HashSet<string> _catalogosCargados = [];
    private List<TicketDto> _tickets = [];
    private TipoKpiDashboard? _kpiActivo;
    private int _numeroRecarga;

    protected override void ConfigurarNavegacion()
    {
        ConfigurarPestanasCatalogos();
        ConfiguradorSidebarDashboard.Configurar(
            Sidebar,
            TopBar,
            SessionCache,
            ContextoSesion,
            FabricaMenusSidebar.ParaAdmin(),
            "tickets",
            SeleccionarVista);

    }

    private void ConfigurarPestanasCatalogos()
    {
        if (_paginasCatalogos.Count > 0)
        {
            return;
        }

        _vistaMateriales.ConfigurarColumnas(
            ("IdMaterial", "Id", 70),
            ("Nombre", "Nombre", 180),
            ("Costo", "Costo", 100),
            ("Inventario", "Existencias", 100),
            ("UnidadMedida", "Unidad de medida", 140));
        _vistaUsuarios.ConfigurarColumnas(
            ("IdUsuario", "Id", 70),
            ("Nombre", "Nombre", 170),
            ("Departamento", "Departamento", 130),
            ("Puesto", "Puesto", 130),
            ("Sucursal", "Sucursal", 130),
            ("Rol", "Rol", 80));
        _vistaDepartamentos.ConfigurarColumnas(
            ("IdDepartamento", "Id", 70),
            ("Nombre", "Nombre", 180),
            ("Descripcion", "Descripción", 260));
        _vistaSucursales.ConfigurarColumnas(
            ("IdSucursal", "Id", 70),
            ("Nombre", "Nombre", 150),
            ("Empresa", "Empresa", 140),
            ("Calle", "Calle", 140),
            ("Numero", "Número", 80),
            ("Colonia", "Colonia", 130),
            ("Telefono", "Teléfono", 120));
        _vistaEmpresas.ConfigurarColumnas(
            ("IdEmpresa", "Id", 70),
            ("Nombre", "Nombre", 170),
            ("Calle", "Calle", 140),
            ("Numero", "Número", 80),
            ("Colonia", "Colonia", 130),
            ("Telefono", "Teléfono", 120));
        _vistaPuestos.ConfigurarColumnas(
            ("IdPuesto", "Id", 70),
            ("Nombre", "Nombre", 180),
            ("Descripcion", "Descripción", 260));
        _vistaRoles.ConfigurarColumnas(
            ("IdRol", "Id", 70),
            ("Descripcion", "Descripción", 260));

        AgregarPaginaCatalogo("inventario", "Inventario", _vistaMateriales);
        AgregarPaginaCatalogo("usuarios", "Usuarios", _vistaUsuarios);
        AgregarPaginaCatalogo("departamentos", "Departamentos", _vistaDepartamentos);
        AgregarPaginaCatalogo("sucursales", "Sucursales", _vistaSucursales);
        AgregarPaginaCatalogo("empresas", "Empresas", _vistaEmpresas);
        AgregarPaginaCatalogo("puestos", "Puestos", _vistaPuestos);
        AgregarPaginaCatalogo("roles", "Roles", _vistaRoles);

        _vistaMateriales.ActualizarClic += async (_, _) => await CargarMaterialesAsync();
        _vistaMateriales.NuevoClic += async (_, _) => await NuevoMaterialAsync();
        _vistaMateriales.EditarClic += async (_, _) => await EditarMaterialAsync();
        _vistaMateriales.EliminarClic += async (_, _) => await EliminarMaterialAsync();

        _vistaUsuarios.ActualizarClic += async (_, _) => await CargarUsuariosAsync();
        _vistaUsuarios.NuevoClic += async (_, _) => await NuevoUsuarioAsync();
        _vistaUsuarios.EditarClic += async (_, _) => await EditarUsuarioAsync();
        _vistaUsuarios.EliminarClic += async (_, _) => await EliminarUsuarioAsync();

        _vistaDepartamentos.ActualizarClic += async (_, _) => await CargarDepartamentosAsync();
        _vistaDepartamentos.NuevoClic += async (_, _) => await NuevoDepartamentoAsync();
        _vistaDepartamentos.EditarClic += async (_, _) => await EditarDepartamentoAsync();
        _vistaDepartamentos.EliminarClic += async (_, _) => await EliminarDepartamentoAsync();

        _vistaSucursales.ActualizarClic += async (_, _) => await CargarSucursalesAsync();
        _vistaSucursales.NuevoClic += async (_, _) => await NuevoSucursalAsync();
        _vistaSucursales.EditarClic += async (_, _) => await EditarSucursalAsync();
        _vistaSucursales.EliminarClic += async (_, _) => await EliminarSucursalAsync();

        _vistaEmpresas.ActualizarClic += async (_, _) => await CargarEmpresasAsync();
        _vistaEmpresas.NuevoClic += async (_, _) => await NuevoEmpresaAsync();
        _vistaEmpresas.EditarClic += async (_, _) => await EditarEmpresaAsync();
        _vistaEmpresas.EliminarClic += async (_, _) => await EliminarEmpresaAsync();

        _vistaPuestos.ActualizarClic += async (_, _) => await CargarPuestosAsync();
        _vistaPuestos.NuevoClic += async (_, _) => await NuevoPuestoAsync();
        _vistaPuestos.EditarClic += async (_, _) => await EditarPuestoAsync();
        _vistaPuestos.EliminarClic += async (_, _) => await EliminarPuestoAsync();

        _vistaRoles.ActualizarClic += async (_, _) => await CargarRolesAsync();
        _vistaRoles.NuevoClic += async (_, _) => await NuevoRolAsync();
        _vistaRoles.EditarClic += async (_, _) => await EditarRolAsync();
        _vistaRoles.EliminarClic += async (_, _) => await EliminarRolAsync();
    }

    private void AgregarPaginaCatalogo(string clave, string titulo, VistaCatalogoAdminControl vista)
    {
        var pagina = new TabPage(titulo)
        {
            Name = $"tab{titulo.Replace(" ", string.Empty)}",
            BackColor = TemaVisual.FondoApp,
            Padding = new Padding(3)
        };
        pagina.Controls.Add(vista);
        _tabMain.Controls.Add(pagina);
        _paginasCatalogos[clave] = pagina;
    }

    protected override void ConfigurarVistaTickets()
    {
        VistaTickets.ConfigurarKpis(
            [
                TipoKpiDashboard.Disponibles,
                TipoKpiDashboard.Urgentes,
                TipoKpiDashboard.EnProceso,
                TipoKpiDashboard.Cerrados,
                TipoKpiDashboard.Reabiertos,
                TipoKpiDashboard.Calificacion
            ],
            [btnNuevoTicket, btnAbrirReportes]);

        VistaTickets.Filtro.InicializarFiltros(ConfiguracionFiltrosTickets.ObtenerCamposAdmin());
        VistaTickets.ControladorPaginacion.Vincular((Action)MostrarPaginaActual);
        VistaTickets.KpiClic += MostrarVistaKpi;
        VistaTickets.Grid.CellDoubleClick += VistaTickets_DoubleClick;

        btnNuevoTicket.Click += (_, _) =>
        {
            using var formularioNuevo = FormFactory.Crear<NuevoTicketForm>();
            if (formularioNuevo.ShowDialog(Formulario) == DialogResult.OK)
            {
                _ = RecargarDatosAsync();
            }
        };

        btnAbrirReportes.Click += (_, _) => AbrirGeneradorReportes();
    }

    protected override async Task PrepararFiltrosAsync()
    {
        var tecnicos = await _usuarioService.ObtenerUsuariosPorRolAsync(RolUsuarioEnum.Tecnico);
        var opciones = new List<object> { "Todos" };
        opciones.AddRange(tecnicos
            .OrderBy(t => t.Nombre)
            .Select(t => (object)new ElementoCombo<int>(t.Nombre ?? $"Técnico #{t.IdUsuario}", t.IdUsuario)));

        VistaTickets.Filtro.CargarOpcionesCombo("Tecnico", opciones);
    }

    public override async Task RecargarDatosAsync()
    {
        var numeroRecarga = Interlocked.Increment(ref _numeroRecarga);

        await Formulario.EjecutarOperacionAsync(async () =>
        {
            var valores = VistaTickets.Filtro.ObtenerValoresFiltros();
            var filtros = ConfiguracionFiltrosTickets.MapearFiltrosAdmin(valores);
            var tickets = await _ticketService.ObtenerTicketsFiltradosAsync(filtros);

            // El usuario puede cambiar otro filtro mientras la petición estaba pendiente.
            // En ese caso, solo la petición más reciente puede actualizar la pantalla.
            if (numeroRecarga != Volatile.Read(ref _numeroRecarga))
            {
                return;
            }

            _tickets = tickets;
            ActualizarKpis(_tickets);
            VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        }, "Error al cargar el dashboard de administración");
    }

    private void ActualizarKpis(IEnumerable<TicketDto> tickets)
    {
        var lista = tickets.ToList();
        var limiteSla = DateTime.Now.AddHours(-48);

        VistaTickets.ActualizarValorKpi(
            TipoKpiDashboard.Disponibles,
            lista.Count(t => t.Estatus == ConstantesEstatus.ABIERTO &&
                             t.FechaAlta.HasValue && t.FechaAlta.Value >= limiteSla));
        VistaTickets.ActualizarValorKpi(
            TipoKpiDashboard.Urgentes,
            lista.Count(t => t.Estatus == ConstantesEstatus.ABIERTO &&
                             t.FechaAlta.HasValue && t.FechaAlta.Value < limiteSla));
        VistaTickets.ActualizarValorKpi(
            TipoKpiDashboard.EnProceso,
            lista.Count(t => t.Estatus == ConstantesEstatus.EN_PROCESO));
        VistaTickets.ActualizarValorKpi(
            TipoKpiDashboard.Cerrados,
            lista.Count(t => t.Estatus == ConstantesEstatus.CERRADO));
        VistaTickets.ActualizarValorKpi(
            TipoKpiDashboard.Reabiertos,
            lista.Count(t => t.Estatus == ConstantesEstatus.REABIERTO));

        var calificaciones = lista
            .Where(t => t.Calificacion.HasValue)
            .Select(t => t.Calificacion!.Value)
            .ToList();
        var promedio = calificaciones.Count == 0 ? 0D : calificaciones.Average();
        VistaTickets.ActualizarValorKpi(TipoKpiDashboard.Calificacion, promedio, true);
    }

    private void MostrarPaginaActual()
    {
        IEnumerable<TicketDto> consulta = _tickets;
        var valores = VistaTickets.Filtro.ObtenerValoresFiltros();
        var (_, fechaInicio, fechaFin, prioridad, usuario) = valores.ExtraerFiltrosComunes();

        if (_kpiActivo == TipoKpiDashboard.Calificacion)
        {
            consulta = consulta.Where(t => t.Calificacion.HasValue);
        }
        else if (_kpiActivo.HasValue)
        {
            var limiteSla = DateTime.Now.AddHours(-48);
            consulta = _kpiActivo.Value switch
            {
                TipoKpiDashboard.Disponibles => consulta.Where(t => t.Estatus == ConstantesEstatus.ABIERTO && t.FechaAlta.HasValue && t.FechaAlta.Value >= limiteSla),
                TipoKpiDashboard.Urgentes => consulta.Where(t => t.Estatus == ConstantesEstatus.ABIERTO && t.FechaAlta.HasValue && t.FechaAlta.Value < limiteSla),
                TipoKpiDashboard.EnProceso => consulta.Where(t => t.Estatus == ConstantesEstatus.EN_PROCESO),
                TipoKpiDashboard.Cerrados => consulta.Where(t => t.Estatus == ConstantesEstatus.CERRADO),
                TipoKpiDashboard.Reabiertos => consulta.Where(t => t.Estatus == ConstantesEstatus.REABIERTO),
                _ => consulta
            };
        }

        if (!string.IsNullOrWhiteSpace(prioridad))
        {
            consulta = consulta.Where(t => string.Equals(t.Prioridad, prioridad, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(usuario))
        {
            consulta = consulta.Where(t => (t.NombreUsuario ?? string.Empty).Contains(usuario, StringComparison.OrdinalIgnoreCase));
        }

        if (fechaInicio.HasValue)
        {
            consulta = consulta.Where(t => t.FechaAlta.HasValue && t.FechaAlta.Value >= fechaInicio.Value);
        }

        if (fechaFin.HasValue)
        {
            consulta = consulta.Where(t => t.FechaAlta.HasValue && t.FechaAlta.Value <= fechaFin.Value);
        }

        var filtrados = consulta.ToList();
        var pagina = VistaTickets.ControladorPaginacion.ObtenerPagina(filtrados).ToList();
        VistaTickets.Grid.DataSource = new ListaVinculableOrdenable<TicketDto>(pagina);

        if (_kpiActivo == TipoKpiDashboard.Calificacion)
        {
            ConfiguracionColumnasDashboard.AplicarPerfilCalificaciones(VistaTickets.Grid);
        }
        else
        {
            ConfiguracionColumnasDashboard.AplicarPerfilAdmin(VistaTickets.Grid);
        }

        VistaTickets.ControladorPaginacion.Actualizar(filtrados.Count);
    }

    private void MostrarVistaKpi(TipoKpiDashboard tipo)
    {
        _kpiActivo = _kpiActivo == tipo ? null : tipo;

        if (_kpiActivo == TipoKpiDashboard.Calificacion)
        {
            TopBar.Titulo = "Calificaciones";
            TopBar.Subtitulo = "Retroalimentación registrada por los usuarios";
        }
        else
        {
            TopBar.Titulo = "Panel de Control";
            TopBar.Subtitulo = "Mesa de Servicio y Gestión Global";
        }

        VistaTickets.ControladorPaginacion.ReiniciarAPrimeraPagina();
        MostrarPaginaActual();
    }

    private async void VistaTickets_DoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        await VistaTickets.Grid.ManejarDetalleTicketAsync(
            e.RowIndex,
            FormFactory,
            RecargarDatosAsync,
            "IdTicket",
            abrirEnRetroalimentacion: _kpiActivo == TipoKpiDashboard.Calificacion);
    }

    private void SeleccionarVista(string clave)
    {
        if (string.Equals(clave, "reportes", StringComparison.OrdinalIgnoreCase))
        {
            AbrirGeneradorReportes();
            return;
        }

        _kpiActivo = null;
        TopBar.Titulo = "Panel de Control";
        TopBar.Subtitulo = "Mesa de Servicio y Gestión Global";
        MostrarPaginaActual();
    }

    private void AbrirGeneradorReportes()
    {
        using var formularioReportes = FormFactory.Crear<GeneradorReportesForm>();
        formularioReportes.ShowDialog(Formulario);
    }
}
