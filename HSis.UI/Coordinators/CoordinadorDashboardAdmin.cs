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
    IClienteSignalRNotificaciones notificationClient,
    INotificacionesApiClient notificacionesApiClient,
    IBusEventosNotificaciones eventBus)
    : CoordinadorDashboardBase(
        formulario,
        sidebar,
        topBar,
        vistaTickets,
        contextoSesion,
        sessionCache,
        formFactory,
        notificationClient,
        notificacionesApiClient,
        eventBus)
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
        }, "Error al cargar el dashboard de administración", "dashboard-admin", VistaTickets.Grid);
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

    private Task CargarMaterialesAsync()
        => CargarCatalogoAsync(
            "inventario",
            _vistaMateriales,
            _materialService.ObtenerMaterialesAsync,
            material => new FilaCatalogoAdmin(
                material,
                new object?[]
                {
                    material.IdMaterial,
                    material.Nombre,
                    material.CostoUnitario.ToString("C2"),
                    material.StockActual,
                    material.UnidadMedida
                }),
            "Error al cargar los materiales");

    private async Task NuevoMaterialAsync()
    {
        using var formulario = FormFactory.Crear<MaterialCatalogoForm>();
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarMaterialesAsync();
        }
    }

    private async Task EditarMaterialAsync()
    {
        var material = _vistaMateriales.ObtenerSeleccionado<MaterialDto>();
        if (material is null)
        {
            return;
        }

        using var formulario = FormFactory.Crear<MaterialCatalogoForm>();
        formulario.Editar(material);
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarMaterialesAsync();
        }
    }

    private async Task EliminarMaterialAsync()
    {
        var material = _vistaMateriales.ObtenerSeleccionado<MaterialDto>();
        if (material is null)
        {
            return;
        }

        await EliminarCatalogoAsync(
            _vistaMateriales,
            "material",
            () => _materialService.EliminarMaterialAsync(material.IdMaterial),
            CargarMaterialesAsync);
    }

    private Task CargarUsuariosAsync()
        => CargarCatalogoAsync(
            "usuarios",
            _vistaUsuarios,
            _usuarioService.ObtenerUsuariosAsync,
            usuario => new FilaCatalogoAdmin(
                usuario,
                new object?[]
                {
                    usuario.IdUsuario,
                    usuario.Nombre,
                    usuario.DepartamentoNombre ?? "Sin departamento",
                    usuario.PuestoNombre ?? "Sin puesto",
                    usuario.SucursalNombre ?? "Sin sucursal",
                    usuario.RolNombre ?? "Sin rol"
                }),
            "Error al cargar los usuarios");

    private async Task NuevoUsuarioAsync()
    {
        using var formulario = FormFactory.Crear<UsuarioCatalogoForm>();
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarUsuariosAsync();
        }
    }

    private async Task EditarUsuarioAsync()
    {
        var usuario = _vistaUsuarios.ObtenerSeleccionado<UsuarioDto>();
        if (usuario is null)
        {
            return;
        }

        using var formulario = FormFactory.Crear<UsuarioCatalogoForm>();
        formulario.Editar(usuario);
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarUsuariosAsync();
        }
    }

    private async Task EliminarUsuarioAsync()
    {
        var usuario = _vistaUsuarios.ObtenerSeleccionado<UsuarioDto>();
        if (usuario is null)
        {
            return;
        }

        await EliminarCatalogoAsync(
            _vistaUsuarios,
            "usuario",
            () => _usuarioService.EliminarUsuarioAsync(usuario.IdUsuario),
            CargarUsuariosAsync);
    }

    private Task CargarDepartamentosAsync()
        => CargarCatalogoAsync(
            "departamentos",
            _vistaDepartamentos,
            _departamentoService.ObtenerDepartamentosAsync,
            departamento => new FilaCatalogoAdmin(
                departamento,
                new object?[]
                {
                    departamento.IdDepartamento,
                    departamento.Nombre,
                    departamento.Descripcion ?? string.Empty
                }),
            "Error al cargar los departamentos");

    private async Task NuevoDepartamentoAsync()
    {
        using var formulario = FormFactory.Crear<DepartamentoCatalogoForm>();
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarDepartamentosAsync();
        }
    }

    private async Task EditarDepartamentoAsync()
    {
        var departamento = _vistaDepartamentos.ObtenerSeleccionado<DepartamentoDto>();
        if (departamento is null)
        {
            return;
        }

        using var formulario = FormFactory.Crear<DepartamentoCatalogoForm>();
        formulario.Editar(departamento);
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarDepartamentosAsync();
        }
    }

    private async Task EliminarDepartamentoAsync()
    {
        var departamento = _vistaDepartamentos.ObtenerSeleccionado<DepartamentoDto>();
        if (departamento is null)
        {
            return;
        }

        await EliminarCatalogoAsync(
            _vistaDepartamentos,
            "departamento",
            () => _departamentoService.EliminarDepartamentoAsync(departamento.IdDepartamento),
            CargarDepartamentosAsync);
    }

    private Task CargarSucursalesAsync()
        => CargarCatalogoAsync(
            "sucursales",
            _vistaSucursales,
            _sucursalService.ObtenerSucursalesAsync,
            sucursal => new FilaCatalogoAdmin(
                sucursal,
                new object?[]
                {
                    sucursal.IdSucursal,
                    sucursal.Nombre,
                    sucursal.EmpresaNombre ?? "Sin empresa",
                    sucursal.Calle ?? string.Empty,
                    sucursal.Numero ?? string.Empty,
                    sucursal.Colonia ?? string.Empty,
                    sucursal.Telefono ?? string.Empty
                }),
            "Error al cargar las sucursales");

    private async Task NuevoSucursalAsync()
    {
        using var formulario = FormFactory.Crear<SucursalCatalogoForm>();
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarSucursalesAsync();
        }
    }

    private async Task EditarSucursalAsync()
    {
        var sucursal = _vistaSucursales.ObtenerSeleccionado<SucursalDto>();
        if (sucursal is null)
        {
            return;
        }

        using var formulario = FormFactory.Crear<SucursalCatalogoForm>();
        formulario.Editar(sucursal);
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarSucursalesAsync();
        }
    }

    private async Task EliminarSucursalAsync()
    {
        var sucursal = _vistaSucursales.ObtenerSeleccionado<SucursalDto>();
        if (sucursal is null)
        {
            return;
        }

        await EliminarCatalogoAsync(
            _vistaSucursales,
            "sucursal",
            () => _sucursalService.EliminarSucursalAsync(sucursal.IdSucursal),
            CargarSucursalesAsync);
    }

    private Task CargarEmpresasAsync()
        => CargarCatalogoAsync(
            "empresas",
            _vistaEmpresas,
            _empresaService.ObtenerEmpresasAsync,
            empresa => new FilaCatalogoAdmin(
                empresa,
                new object?[]
                {
                    empresa.IdEmpresa,
                    empresa.Nombre,
                    empresa.Calle ?? string.Empty,
                    empresa.Numero ?? string.Empty,
                    empresa.Colonia ?? string.Empty,
                    empresa.Telefono ?? string.Empty
                }),
            "Error al cargar las empresas");

    private async Task NuevoEmpresaAsync()
    {
        using var formulario = FormFactory.Crear<EmpresaCatalogoForm>();
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarEmpresasAsync();
        }
    }

    private async Task EditarEmpresaAsync()
    {
        var empresa = _vistaEmpresas.ObtenerSeleccionado<EmpresaDto>();
        if (empresa is null)
        {
            return;
        }

        using var formulario = FormFactory.Crear<EmpresaCatalogoForm>();
        formulario.Editar(empresa);
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarEmpresasAsync();
        }
    }

    private async Task EliminarEmpresaAsync()
    {
        var empresa = _vistaEmpresas.ObtenerSeleccionado<EmpresaDto>();
        if (empresa is null)
        {
            return;
        }

        await EliminarCatalogoAsync(
            _vistaEmpresas,
            "empresa",
            () => _empresaService.EliminarEmpresaAsync(empresa.IdEmpresa),
            CargarEmpresasAsync);
    }

    private Task CargarPuestosAsync()
        => CargarCatalogoAsync(
            "puestos",
            _vistaPuestos,
            _puestoService.ObtenerPuestosAsync,
            puesto => new FilaCatalogoAdmin(
                puesto,
                new object?[]
                {
                    puesto.IdPuesto,
                    puesto.Nombre,
                    puesto.Descripcion ?? string.Empty
                }),
            "Error al cargar los puestos");

    private async Task NuevoPuestoAsync()
    {
        using var formulario = FormFactory.Crear<PuestoCatalogoForm>();
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarPuestosAsync();
        }
    }

    private async Task EditarPuestoAsync()
    {
        var puesto = _vistaPuestos.ObtenerSeleccionado<PuestoDto>();
        if (puesto is null)
        {
            return;
        }

        using var formulario = FormFactory.Crear<PuestoCatalogoForm>();
        formulario.Editar(puesto);
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarPuestosAsync();
        }
    }

    private async Task EliminarPuestoAsync()
    {
        var puesto = _vistaPuestos.ObtenerSeleccionado<PuestoDto>();
        if (puesto is null)
        {
            return;
        }

        await EliminarCatalogoAsync(
            _vistaPuestos,
            "puesto",
            () => _puestoService.EliminarPuestoAsync(puesto.IdPuesto),
            CargarPuestosAsync);
    }

    private Task CargarRolesAsync()
        => CargarCatalogoAsync(
            "roles",
            _vistaRoles,
            _rolUsuarioService.ObtenerRolesUsuarioAsync,
            rol => new FilaCatalogoAdmin(
                rol,
                new object?[]
                {
                    rol.IdRol,
                    rol.Descripcion
                }),
            "Error al cargar los roles");

    private async Task NuevoRolAsync()
    {
        using var formulario = FormFactory.Crear<RolUsuarioCatalogoForm>();
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarRolesAsync();
        }
    }

    private async Task EditarRolAsync()
    {
        var rol = _vistaRoles.ObtenerSeleccionado<RolUsuarioDto>();
        if (rol is null)
        {
            return;
        }

        using var formulario = FormFactory.Crear<RolUsuarioCatalogoForm>();
        formulario.Editar(rol);
        if (formulario.ShowDialog(Formulario) == DialogResult.OK)
        {
            await CargarRolesAsync();
        }
    }

    private async Task EliminarRolAsync()
    {
        var rol = _vistaRoles.ObtenerSeleccionado<RolUsuarioDto>();
        if (rol is null)
        {
            return;
        }

        await EliminarCatalogoAsync(
            _vistaRoles,
            "rol",
            () => _rolUsuarioService.EliminarRolUsuarioAsync(rol.IdRol),
            CargarRolesAsync);
    }

    private async Task CargarCatalogoAsync<T>(
        string clave,
        VistaCatalogoAdminControl vista,
        Func<Task<List<T>>> obtener,
        Func<T, FilaCatalogoAdmin> crearFila,
        string mensajeError)
    {
        await vista.EjecutarCargaAsync(async () =>
        {
            await Formulario.EjecutarOperacionAsync(async () =>
            {
                var registros = await obtener();
                vista.CargarFilas(registros.Select(crearFila));
                _catalogosCargados.Add(clave);
            }, mensajeError, $"catalogo-{clave}-carga");
        }, $"catalogo-{clave}");
    }

    private async Task EliminarCatalogoAsync(
        VistaCatalogoAdminControl vista,
        string entidad,
        Func<Task<bool>> eliminar,
        Func<Task> recargar)
    {
        if (!DialogoUIHelper.Confirmar($"¿Desea eliminar el {entidad} seleccionado?"))
        {
            return;
        }

        var eliminado = false;
        await vista.EjecutarCargaAsync(async () =>
        {
            await Formulario.EjecutarOperacionAsync(async () =>
            {
                eliminado = await eliminar();
                if (!eliminado)
                {
                    throw new KeyNotFoundException($"El {entidad} ya no existe.");
                }
            }, $"Error al eliminar el {entidad}", $"catalogo-{entidad}-eliminar");
        }, $"catalogo-{entidad}");

        if (eliminado)
        {
            await recargar();
        }
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

        if (_paginasCatalogos.TryGetValue(clave, out var paginaCatalogo))
        {
            _tabMain.SelectedTab = paginaCatalogo;
            ConfigurarEncabezadoCatalogo(clave);
            _ = CargarCatalogoPorClaveAsync(clave);
            return;
        }

        if (string.Equals(clave, "tickets", StringComparison.OrdinalIgnoreCase))
        {
            _tabMain.SelectedIndex = 0;
        }

        _kpiActivo = null;
        TopBar.Titulo = "Panel de Control";
        TopBar.Subtitulo = "Mesa de Servicio y Gestión Global";
        MostrarPaginaActual();
    }

    private void ConfigurarEncabezadoCatalogo(string clave)
    {
        var encabezado = clave switch
        {
            "inventario" => ("Inventario", "Gestión de materiales y existencias"),
            "usuarios" => ("Usuarios", "Gestión de usuarios y asignación de roles"),
            "departamentos" => ("Departamentos", "Catálogo de departamentos"),
            "sucursales" => ("Sucursales", "Catálogo de sucursales y empresas"),
            "empresas" => ("Empresas", "Catálogo de empresas"),
            "puestos" => ("Puestos", "Catálogo de puestos"),
            "roles" => ("Roles", "Catálogo de roles de usuario"),
            _ => ("Panel de Control", "Mesa de Servicio y Gestión Global")
        };

        TopBar.Titulo = encabezado.Item1;
        TopBar.Subtitulo = encabezado.Item2;
    }

    private Task CargarCatalogoPorClaveAsync(string clave)
        => clave switch
        {
            "inventario" => CargarSiEsNecesarioAsync(clave, CargarMaterialesAsync),
            "usuarios" => CargarSiEsNecesarioAsync(clave, CargarUsuariosAsync),
            "departamentos" => CargarSiEsNecesarioAsync(clave, CargarDepartamentosAsync),
            "sucursales" => CargarSiEsNecesarioAsync(clave, CargarSucursalesAsync),
            "empresas" => CargarSiEsNecesarioAsync(clave, CargarEmpresasAsync),
            "puestos" => CargarSiEsNecesarioAsync(clave, CargarPuestosAsync),
            "roles" => CargarSiEsNecesarioAsync(clave, CargarRolesAsync),
            _ => Task.CompletedTask
        };

    private async Task CargarSiEsNecesarioAsync(string clave, Func<Task> cargar)
    {
        if (_catalogosCargados.Contains(clave))
        {
            return;
        }

        await cargar();
    }

    private void AbrirGeneradorReportes()
    {
        using var formularioReportes = FormFactory.Crear<GeneradorReportesForm>();
        formularioReportes.ShowDialog(Formulario);
    }
}
