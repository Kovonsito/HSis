#nullable enable
using System.Data;
using System.Drawing;
using System.Runtime.Versioning;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Forms.Otros;
using HSis.UI.Helpers;
using HSis.UI.Services;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardAdminForm : Form
    {
        private readonly ITicketService _ticketService;
        private readonly ICatalogoService _catalogoService;
        private readonly IUsuarioService _usuarioService;
        private readonly AdministradorUINotificaciones _uiManager;
        private bool _estaCargando = true;
        private PaginacionControl PaginacionControl = null!;

        private IndicadorControl? _ucCalificacion;

        private readonly IFabricaFormularios _fabricaFormularios;
        private readonly ISessionCacheService _sessionCache;

        public DashboardAdminForm(
            ITicketService ticketService,
            ICatalogoService catalogoService,
            IUsuarioService usuarioService,
            AdministradorUINotificaciones uiManager,
            IFabricaFormularios fabricaFormularios,
            ISessionCacheService sessionCache)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _catalogoService = catalogoService;
            _usuarioService = usuarioService;
            _uiManager = uiManager;
            _fabricaFormularios = fabricaFormularios;
            _sessionCache = sessionCache;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            InicializarLayoutDashboard();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            if (tabMain != null)
            {
                _uiManager.Adjuntar(this, tabMain, () => Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync()));
            }

            ConfigurarFechasYFiltros();

            // Cargar los combos de filtros antes del grid
            await InicializarCombosFiltrosAsync();

            // Cargamos KPIs y Grid de tickets en paralelo
            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());

            await ConfigurarTabsCatalogosAsync();
        }

        private void InicializarLayoutDashboard()
        {
            // Instanciar control de paginación reutilizable
            PaginacionControl = new PaginacionControl();
            PaginacionControl.PaginaCambiada += async (s, e) => { if (!_estaCargando) await FiltrarTicketsAsync(); };
            PaginacionControl.Margin = new Padding(12, 0, 12, 0);

            var tblPrincipal = AyudanteDisenoPanel.CrearPanelPrincipal(tabTickets.ClientSize, incluirFiltros: true);
            tblPrincipal.Name = "tblPrincipalTickets";

            _ucCalificacion = new IndicadorControl();
            _ucCalificacion.IndicadorClic += UcCalificacion_Click;
            var tblIndicadores = AyudanteDisenoPanel.CrearPanelIndicadores(
                "tblIndicadoresAdmin",
                6,
                ucNuevos, ucUrgentes, ucEnProceso, ucCerrados, ucReabiertos, _ucCalificacion
            );

            // Configurar otros paneles accesorios
            pnlFiltros.Dock = DockStyle.Fill;
            pnlFiltros.Margin = new Padding(12, 5, 12, 5);

            dgvTickets.Dock = DockStyle.Fill;
            dgvTickets.Margin = new Padding(12, 10, 12, 12);

            // Ensamblar el layout en el grid principal
            tblPrincipal.Controls.Add(lblTitulo, 0, 0);
            tblPrincipal.Controls.Add(tblIndicadores, 0, 1);
            tblPrincipal.Controls.Add(pnlFiltros, 0, 2);
            tblPrincipal.Controls.Add(dgvTickets, 0, 3);
            tblPrincipal.Controls.Add(PaginacionControl, 0, 4);

            // Reubicar controles desde el contenedor original al panel principal
            AyudanteDisenoPanel.ReubicarControles(tabTickets, tblPrincipal, lblTitulo, ucNuevos, ucUrgentes, ucEnProceso, ucCerrados, ucReabiertos, pnlFiltros, dgvTickets);
        }

        private void ConfigurarFechasYFiltros()
        {
            var campos = new List<HSis.UI.Controls.FiltroCampo>
            {
                new() { NombrePropiedad = "Estatus", Etiqueta = "Estatus:", Tipo = HSis.UI.Controls.TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Nuevos", "Urgentes", "Abierto", "En Proceso", "Cerrado", "Reabierto"], Ancho = 130 },
                new() { NombrePropiedad = "Prioridad", Etiqueta = "Prioridad:", Tipo = HSis.UI.Controls.TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Alta", "Media", "Baja", "Urgente"], Ancho = 130 },
                new() { NombrePropiedad = "Tecnico", Etiqueta = "Técnico:", Tipo = HSis.UI.Controls.TipoFiltroControl.ComboSeleccion, Ancho = 160 },
                new() { NombrePropiedad = "Usuario", Etiqueta = "Usuario Emisor:", Tipo = HSis.UI.Controls.TipoFiltroControl.Texto, Ancho = 160 },
                new() { NombrePropiedad = "Temporal", Etiqueta = "Vista Temporal:", Tipo = HSis.UI.Controls.TipoFiltroControl.ComboSeleccion, ValoresCombo = ["Todos", "Día", "Semana", "Mes", "Año"], Ancho = 130 },
                new() { NombrePropiedad = "FechaInicio", Etiqueta = "Desde:", Tipo = HSis.UI.Controls.TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(-30) },
                new() { NombrePropiedad = "FechaFin", Etiqueta = "Hasta:", Tipo = HSis.UI.Controls.TipoFiltroControl.Fecha, Ancho = 130, ValorDefecto = DateTime.Today.AddDays(1).AddTicks(-1) }
            };

            filtroGenerico.InicializarFiltros(campos);
            filtroGenerico.FiltroCambiado += async (s, e) => { if (!_estaCargando) { PaginacionControl.PaginaActual = 1; await FiltrarTicketsAsync(); } };
        }

        public void ucNuevos_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", "Nuevos");
        }

        private void ucUrgentes_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", "Urgentes");
        }

        private void ucEnProceso_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", "En Proceso");
        }

        private void ucCerrados_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", "Cerrado");
        }

        private void ucReabiertos_ucIndicadorEvent(object sender, EventArgs e)
        {
            filtroGenerico.EstablecerValorFiltro("Estatus", "Reabierto");
        }

        private async Task InicializarCombosFiltrosAsync()
        {
            _estaCargando = true;

            try
            {
                var admins = await _usuarioService.ObtenerUsuariosPorRolAsync(1);
                var tecnicos = await _usuarioService.ObtenerUsuariosPorRolAsync(2);

                var listaTecnicos = new List<object> { new { Id = (int?)0, Nombre = "Todos" } };

                foreach (var a in admins)
                {
                    listaTecnicos.Add(new { Id = (int?)a.IdUsuario, Nombre = $"Admin - {a.Nombre}" });
                }
                foreach (var t in tecnicos)
                {
                    listaTecnicos.Add(new { Id = (int?)t.IdUsuario, Nombre = $"Técnico - {t.Nombre}" });
                }

                filtroGenerico.ActualizarCombo("Tecnico", listaTecnicos, "Nombre", "Id");
            }
            catch (Exception)
            {
                filtroGenerico.ActualizarCombo("Tecnico", new List<object> { new { Id = (int?)0, Nombre = "Todos" } }, "Nombre", "Id");
            }

            _estaCargando = false;
        }

        private async Task FiltrarTicketsAsync()
        {
            if (_estaCargando) return;

            var filtros = new TicketFilterDto();
            var vals = filtroGenerico.ObtenerValoresFiltros();

            // 1. Estatus
            if (vals.TryGetValue("Estatus", out var estVal) && estVal != null)
            {
                var est = estVal.ToString();
                if (est != null && est != "Todos")
                {
                    filtros.Estatus = est;
                }
            }

            // 2. Prioridad
            if (vals.TryGetValue("Prioridad", out var priVal) && priVal != null)
            {
                var pri = priVal.ToString();
                if (pri != null && pri != "Todos")
                {
                    filtros.Prioridad = pri;
                }
            }

            // 3. Técnico
            if (vals.TryGetValue("Tecnico", out var tecVal) && tecVal != null)
            {
                if (int.TryParse(tecVal.ToString(), out int idTecnico) && idTecnico > 0)
                {
                    filtros.IdTecnico = idTecnico;
                }
            }

            // 4. Usuario Emisor
            if (vals.TryGetValue("Usuario", out var usrVal) && usrVal != null)
            {
                string emisor = usrVal.ToString()!;
                if (!string.IsNullOrWhiteSpace(emisor))
                {
                    filtros.UsuarioEmisor = emisor;
                }
            }

            // 5. Vista Temporal
            if (vals.TryGetValue("Temporal", out var tempVal) && tempVal != null)
            {
                string tempSel = tempVal.ToString()!;
                if (tempSel != "Todos")
                {
                    filtros.RangoTemporal = tempSel switch
                    {
                        "Día" => VistaTemporal.Dia,
                        "Semana" => VistaTemporal.Semana,
                        "Mes" => VistaTemporal.Mes,
                        "Año" => VistaTemporal.Ano,
                        _ => VistaTemporal.Todos
                    };
                }
                else
                {
                    filtros.RangoTemporal = VistaTemporal.Todos;
                }
            }
            else
            {
                filtros.RangoTemporal = VistaTemporal.Todos;
            }

            // 6. Rango de Fechas
            DateTime fi = DateTime.Today.AddDays(-30);
            if (vals.TryGetValue("FechaInicio", out var fiVal) && fiVal is DateTime)
            {
                fi = (DateTime)fiVal;
            }
            filtros.FechaAltaInicio = fi.Date;

            DateTime ff = DateTime.Today.AddDays(1).AddTicks(-1);
            if (vals.TryGetValue("FechaFin", out var ffVal) && ffVal is DateTime)
            {
                ff = (DateTime)ffVal;
            }
            filtros.FechaAltaFin = ff.Date.AddDays(1).AddTicks(-1);

            var resultadoPaginado = await _ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, PaginacionControl.PaginaActual, PaginacionControl.TamanoPagina);
            PaginacionControl.TotalRegistros = resultadoPaginado.TotalCount;

            ActualizarGrid(resultadoPaginado.Items);
            PaginacionControl.ActualizarInterfaz();
        }

        private async void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            _estaCargando = true;
            var valoresDefecto = new Dictionary<string, object?>
            {
                { "FechaInicio", DateTime.Today.AddDays(-30) },
                { "FechaFin", DateTime.Today.AddDays(1).AddTicks(-1) }
            };
            filtroGenerico.LimpiarFiltros(valoresDefecto);
            PaginacionControl.PaginaActual = 1;
            _estaCargando = false;

            await CargarGridCompletoAsync();
        }

        private void btnAbrirReportes_Click(object sender, EventArgs e)
        {
            var modal = _fabricaFormularios.Crear<GeneradorReportesForm>();
            modal.ShowDialog();
        }

        private async Task CargarKPIsAsync()
        {
            // Iniciamos todas las tareas de conteo en paralelo
            var taskNuevos = _ticketService.ObtenerCountTicketsPorSLAAsync(false);
            var taskUrgentes = _ticketService.ObtenerCountTicketsPorSLAAsync(true);
            var taskEnProceso = _ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.EN_PROCESO);
            var taskCerrados = _ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.CERRADO);
            var taskReabiertos = _ticketService.ObtenerCountTicketsPorEstatusAsync(ConstantesEstatus.REABIERTO);
            var taskCalificacion = _ticketService.ObtenerPromedioCalificacionTecnicoAsync(SesionSistema.IdUsuario);

            await Task.WhenAll(taskNuevos, taskUrgentes, taskEnProceso, taskCerrados, taskReabiertos, taskCalificacion);

            ucNuevos.Titulo = "Nuevos";
            ucNuevos.Cantidad = taskNuevos.Result.ToString();
            ucNuevos.ColorFondo = Color.DodgerBlue;
            ucNuevos.ImagenFondo = Properties.Resources.Nuevo;

            ucUrgentes.Titulo = "Urgentes";
            ucUrgentes.Cantidad = taskUrgentes.Result.ToString();
            ucUrgentes.ColorFondo = Color.Red;
            ucUrgentes.ImagenFondo = Properties.Resources.Urgente;

            ucEnProceso.Titulo = "En proceso";
            ucEnProceso.Cantidad = taskEnProceso.Result.ToString();
            ucEnProceso.ColorFondo = Color.Yellow;
            ucEnProceso.ImagenFondo = Properties.Resources.En_proceso;

            ucCerrados.Titulo = "Cerrados";
            ucCerrados.Cantidad = taskCerrados.Result.ToString();
            ucCerrados.ColorFondo = Color.LawnGreen;
            ucCerrados.ImagenFondo = Properties.Resources.Cerrado;

            ucReabiertos.Titulo = "Reabiertos";
            ucReabiertos.Cantidad = taskReabiertos.Result.ToString();
            ucReabiertos.ColorFondo = Color.Orange;

            if (_ucCalificacion != null)
            {
                var promedio = taskCalificacion.Result;
                _ucCalificacion.Titulo = "Mi Calificación";
                _ucCalificacion.Cantidad = promedio > 0 ? $"⭐ {promedio:F1}" : "⭐ N/A";
                _ucCalificacion.ColorFondo = Color.FromArgb(155, 89, 182); // Púrpura igual al técnico
            }
        }

        private async Task CargarGridCompletoAsync()
        {
            PaginacionControl.PaginaActual = 1;
            await FiltrarTicketsAsync();
        }

        private void ActualizarGrid(List<TicketDto> listaTickets)
        {
            var listaMapeada = listaTickets.Select(t => new TicketGridDto
            {
                Folio = t.IdTicket,
                NombreUsuario = t.NombreUsuario,
                Status = t.Status ?? "N/A",
                Prioridad = t.Prioridad ?? "N/A",
                Alta = t.Alta,
                Atención = t.Atencion,
                Cierre = t.Cierre ?? DateTime.Now,
                AtendidoPor = t.NombreTecnico,
                Descripción = t.Descripcion ?? "N/A",
                Solución = t.Solucion ?? "N/A"
            }).ToList();

            dgvTickets.DataSource = new ListaVinculableOrdenable<TicketGridDto>(listaMapeada);
            dgvTickets.AutoajustarAnchosMinimos();
        }


        private async void btnRecargar_Click(object sender, EventArgs e)
        {
            await CargarGridCompletoAsync();
            await CargarKPIsAsync();
        }

        private async void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            await dgvTickets.ManejarDetalleTicketAsync(e.RowIndex, _fabricaFormularios, () => Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync()), "Folio");
        }

        private async Task ConfigurarTabsCatalogosAsync()
        {
            var catalogos = new (string Nombre, Type Tipo)[] {
                ("Usuarios", typeof(Usuario)),
                ("Departamentos", typeof(Departamento)),
                ("Empresas", typeof(Empresa)),
                ("Materiales", typeof(Material)),
                ("Puestos", typeof(Puesto)),
                ("RolesUsuario", typeof(RolUsuario)),
                ("Sucursales", typeof(Sucursal))
            };

            foreach (var (nombre, tipo) in catalogos)
            {
                try
                {
                    await ConfigurarTabParaCatalogo(nombre, tipo);
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error al configurar o cargar la pestaña de catálogo '{Nombre}'.", nombre);
                }
            }
        }

        private async Task ConfigurarTabParaCatalogo(string nombre, Type tipo)
        {
            TabPage tab = new(nombre);

            DataGridView dgv = new()
            {
                Dock = DockStyle.Fill,
                Name = "dgv" + nombre,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            Panel panelTop = new() { Dock = DockStyle.Top, Height = 50 };
            Button btnCrear = new() { Text = "Crear", Location = new Point(10, 10), Width = 100, Height = 30 };
            Button btnEliminar = new() { Text = "Eliminar", Location = new Point(120, 10), Width = 100, Height = 30 };

            panelTop.Controls.Add(btnCrear);
            panelTop.Controls.Add(btnEliminar);

            if (tipo == typeof(Material))
            {
                AgregarControlesInventario(panelTop, dgv);
            }

            tab.Controls.Add(dgv);
            tab.Controls.Add(panelTop);
            tabMain.TabPages.Add(tab);

            ConfigurarFormateoDeCeldas(dgv, tipo);
            await CargarDatosCatalogo(tipo, dgv);

            btnCrear.Click += async (s, e) => await ManejarCreacionRegistro(tipo, dgv);
            btnEliminar.Click += async (s, e) => await ManejarEliminacionRegistro(tipo, dgv);
            dgv.CellDoubleClick += async (s, e) => await ManejarEdicionRegistro(e, tipo, dgv);
        }

        private void AgregarControlesInventario(Panel panelTop, DataGridView dgv)
        {
            Button btnIngreso = new() { Text = "Nuevo Movimiento", Location = new Point(230, 10), Width = 150, Height = 30 };
            Button btnKardex = new() { Text = "Ver Kardex", Location = new Point(390, 10), Width = 100, Height = 30 };

            btnIngreso.Click += async (s, ev) =>
            {
                var nuevoMovimiento = new MovimientoMaterial
                {
                    IdUsuario = SesionSistema.IdUsuario,
                    FechaMovimiento = DateTime.Now,
                    Cantidad = 1,
                    Motivo = "Ingreso por Compra" // Motivo inicial por defecto
                };
                var frm = _fabricaFormularios.CrearEditorDinamico(nuevoMovimiento, "Nuevo Movimiento de Almacén");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    await _catalogoService.CrearAsync(nuevoMovimiento);
                    MessageBox.Show("Movimiento registrado con éxito.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _ = CargarDatosCatalogo(typeof(Material), dgv);
                }
            };

            btnKardex.Click += (s, ev) =>
            {
                var frmK = _fabricaFormularios.Crear<KardexForm>();
                frmK.ShowDialog();
            };

            panelTop.Controls.Add(btnIngreso);
            panelTop.Controls.Add(btnKardex);
        }

        private static void ConfigurarFormateoDeCeldas(DataGridView dgv, Type tipo)
        {
            dgv.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

                var columnName = dgv.Columns[e.ColumnIndex].Name;
                string idPk = "Id" + (tipo.Name == "RolUsuario" ? "Rol" : tipo.Name);

                if (columnName.StartsWith("Id") && columnName != idPk)
                {
                    var navPropName = columnName + "Navigation";
                    var entidad = dgv.Rows[e.RowIndex].DataBoundItem;
                    if (entidad != null)
                    {
                        var navProp = entidad.GetType().GetProperty(navPropName);
                        var navObj = navProp?.GetValue(entidad);
                        if (navObj != null)
                        {
                            var nombreProp = navObj.GetType().GetProperty("Nombre") ?? navObj.GetType().GetProperty("Descripción");
                            if (nombreProp != null)
                            {
                                e.Value = nombreProp.GetValue(navObj);
                                e.FormattingApplied = true;
                            }
                        }
                    }
                }
            };
        }

        private async Task ManejarCreacionRegistro(Type tipo, DataGridView dgv)
        {
            object nuevaEntidad = Activator.CreateInstance(tipo)!;
            var frm = _fabricaFormularios.CrearEditorDinamico(nuevaEntidad, $"Crear {tipo.Name}");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var miMetodo = typeof(ICatalogoService).GetMethod("CrearAsync")!.MakeGenericMethod(tipo);
                    Task task = (Task)miMetodo.Invoke(_catalogoService, [nuevaEntidad])!;
                    await task;
                    await CargarDatosCatalogo(tipo, dgv);
                }
                catch (Exception ex)
                {
                    var realEx = ex is System.Reflection.TargetInvocationException ? ex.InnerException : ex;
                    if (realEx is FluentValidation.ValidationException vex)
                    {
                        string msg = string.Join("\n", vex.Errors.Select(e => "- " + e.ErrorMessage));
                        MessageBox.Show($"Datos inválidos:\n{msg}", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                    else
                    {
                        MessageBox.Show($"Error al crear el registro: {realEx?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task ManejarEliminacionRegistro(Type tipo, DataGridView dgv)
        {
            if (dgv.SelectedRows.Count > 0)
            {
                var row = dgv.SelectedRows[0];
                string idName = "Id" + (tipo.Name == "RolUsuario" ? "Rol" : tipo.Name);
                var idObj = row.Cells[idName]?.Value;
                if (idObj != null && MessageBox.Show("¿Seguro que deseas eliminar el registro?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        var miMetodo = typeof(ICatalogoService).GetMethod("EliminarAsync")!.MakeGenericMethod(tipo);
                        Task task = (Task)miMetodo.Invoke(_catalogoService, [idObj])!;
                        await task;
                        await CargarDatosCatalogo(tipo, dgv);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("No se pudo eliminar el registro (probablemente esté en uso). " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task ManejarEdicionRegistro(DataGridViewCellEventArgs e, Type tipo, DataGridView dgv)
        {
            if (e.RowIndex >= 0)
            {
                object? entidadExistente = dgv.Rows[e.RowIndex].DataBoundItem;
                if (entidadExistente is null) return;

                string? passwordHashOriginal = null;
                if (tipo == typeof(Usuario))
                {
                    var u = (Usuario)entidadExistente;
                    passwordHashOriginal = u.Contraseña;
                    u.Contraseña = "";
                }

                var frm = _fabricaFormularios.CrearEditorDinamico(entidadExistente, $"Editar {tipo.Name}");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var miMetodo = typeof(ICatalogoService).GetMethod("ActualizarAsync")!.MakeGenericMethod(tipo);
                        Task task = (Task)miMetodo.Invoke(_catalogoService, [entidadExistente])!;
                        await task;
                        await CargarDatosCatalogo(tipo, dgv);
                    }
                    catch (Exception ex)
                    {
                        var realEx = ex is System.Reflection.TargetInvocationException ? ex.InnerException : ex;
                        if (realEx is FluentValidation.ValidationException vex)
                        {
                            string msg = string.Join("\n", vex.Errors.Select(e => "- " + e.ErrorMessage));
                            MessageBox.Show($"Datos inválidos:\n{msg}", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        }
                        else
                        {
                            MessageBox.Show($"Error al actualizar el registro: {realEx?.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        // Restaurar contraseña original si falló la actualización
                        if (tipo == typeof(Usuario)) ((Usuario)entidadExistente).Contraseña = passwordHashOriginal;
                    }
                }
                else
                {
                    if (tipo == typeof(Usuario))
                    {
                        var u = (Usuario)entidadExistente;
                        u.Contraseña = passwordHashOriginal;
                    }
                }
            }
        }

        private async Task CargarDatosCatalogo(Type tipoEntidad, DataGridView dgv)
        {
            var miMetodo = typeof(ICatalogoService).GetMethod("ObtenerTodosAsync")!.MakeGenericMethod(tipoEntidad);
            Task task = (Task)miMetodo.Invoke(_catalogoService, null)!;
            await task;

            var resultProp = task.GetType().GetProperty("Result");
            var resultList = resultProp?.GetValue(task);

            if (resultList != null)
            {
                var bindingListType = typeof(ListaVinculableOrdenable<>).MakeGenericType(tipoEntidad);
                var sortableList = Activator.CreateInstance(bindingListType, resultList);
                dgv.DataSource = sortableList;
            }
            else
            {
                dgv.DataSource = null;
            }

            // Ocultar columnas no deseadas y renombrar cabeceras
            string idPk = "Id" + (tipoEntidad.Name == "RolUsuario" ? "Rol" : tipoEntidad.Name);



            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Name.EndsWith("Navigation") || (col.ValueType?.IsGenericType == true && col.ValueType.GetGenericTypeDefinition() != typeof(Nullable<>)))
                {
                    col.Visible = false; // Ocultar objetos virtuales de navegación y colecciones
                }
                else
                {
                    if (col.Name.StartsWith("Id") && col.Name != idPk)
                    {
                        col.HeaderText = col.Name[2..]; // Ejemplo: "IdDepartamento" se lee como "Departamento"
                    }
                }
            }

            dgv.AutoajustarAnchosMinimos();
        }

        private async void UcCalificacion_Click(object? sender, EventArgs e)
        {
            try
            {
                var promedio = await _ticketService.ObtenerPromedioCalificacionTecnicoAsync(SesionSistema.IdUsuario);
                MessageBox.Show($"Tu calificación promedio como Administrador resolviendo tickets es: {promedio:F1} de 5.0 ⭐", "Mi Calificación", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al obtener la calificación: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }


    }
}
