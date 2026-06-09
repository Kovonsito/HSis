#nullable enable
using System.Data;
using System.Runtime.Versioning;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    public partial class frmDashboardAdmin : Form
    {
        private readonly ITicketService _ticketService;
        private readonly ICatalogoService _catalogoService;
        private readonly IUsuarioService _usuarioService;
        private readonly NotificationUIManager _uiManager;
        private bool _estaCargando = true;
        private ucPaginacion ucPaginacion = null!;

        private ucIndicador? _ucCalificacion;

        private readonly IFormFactory _formFactory;
        private readonly ISessionCacheService _sessionCache;

        public frmDashboardAdmin(
            ITicketService ticketService,
            ICatalogoService catalogoService,
            IUsuarioService usuarioService,
            NotificationUIManager uiManager,
            IFormFactory formFactory,
            ISessionCacheService sessionCache)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _catalogoService = catalogoService;
            _usuarioService = usuarioService;
            _uiManager = uiManager;
            _formFactory = formFactory;
            _sessionCache = sessionCache;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            InicializarLayoutTicketsAdmin();
            SesionSistema.ConfigurarMenuSesion(this, _sessionCache);

            if (tabMain != null)
            {
                _uiManager.Attach(this, tabMain, () => Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync()));
            }

            ConfigurarFechasYFiltros();

            // Cargar los combos de filtros antes del grid
            await InicializarCombosFiltrosAsync();

            // Cargamos KPIs y Grid de tickets en paralelo
            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());

            ConfigurarTabsCatalogos();
        }

        private void InicializarLayoutTicketsAdmin()
        {
            var tblPrincipal = CrearPanelPrincipal();
            var tblIndicadores = CrearPanelIndicadores();
            var pnlRecargar = ConfigurarBotonRecargar();

            // Instanciar control de paginación reutilizable
            ucPaginacion = new ucPaginacion();
            ucPaginacion.PageChanged += async (s, e) => { if (!_estaCargando) await FiltrarTicketsAsync(); };

            // Configurar otros paneles accesorios
            pnlFiltros.Dock = DockStyle.Fill;
            pnlFiltros.Margin = new Padding(5);

            dgvTickets.Dock = DockStyle.Fill;
            dgvTickets.Margin = new Padding(5);

            // Ensamblar el layout en el grid principal
            tblPrincipal.Controls.Add(tblIndicadores, 0, 0);
            tblPrincipal.Controls.Add(pnlFiltros, 0, 1);
            tblPrincipal.Controls.Add(pnlRecargar, 0, 2);
            tblPrincipal.Controls.Add(dgvTickets, 0, 3);
            tblPrincipal.Controls.Add(ucPaginacion, 0, 4);

            // Reubicar controles desde el contenedor original al panel principal
            ReubicarControlesAlPrincipal(tblPrincipal);
        }

        private TableLayoutPanel CrearPanelPrincipal()
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblPrincipalTickets",
                RowCount = 5,
                ColumnCount = 1,
                Size = tabTickets.ClientSize
            };

            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F)); // Fila 0: 6 Indicadores
            tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Fila 1: Panel de Filtros
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));  // Fila 2: Botón de Recargar
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Fila 3: Grid (dgvTickets)
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // Fila 4: Panel de Paginación

            return tbl;
        }

        private TableLayoutPanel CrearPanelIndicadores()
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblIndicadoresAdmin",
                RowCount = 1,
                ColumnCount = 6,
                Margin = new Padding(0)
            };

            for (int i = 0; i < 6; i++)
            {
                tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            }

            _ucCalificacion = new ucIndicador
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(5)
            };
            _ucCalificacion.ucIndicadorEvent += UcCalificacion_Click;

            // Configurar docks y margins
            ucNuevos.Dock = DockStyle.Fill; ucNuevos.Margin = new Padding(5);
            ucUrgentes.Dock = DockStyle.Fill; ucUrgentes.Margin = new Padding(5);
            ucEnProceso.Dock = DockStyle.Fill; ucEnProceso.Margin = new Padding(5);
            ucCerrados.Dock = DockStyle.Fill; ucCerrados.Margin = new Padding(5);
            ucReabiertos.Dock = DockStyle.Fill; ucReabiertos.Margin = new Padding(5);

            tbl.Controls.Add(ucNuevos, 0, 0);
            tbl.Controls.Add(ucUrgentes, 1, 0);
            tbl.Controls.Add(ucEnProceso, 2, 0);
            tbl.Controls.Add(ucCerrados, 3, 0);
            tbl.Controls.Add(ucReabiertos, 4, 0);
            tbl.Controls.Add(_ucCalificacion, 5, 0);

            return tbl;
        }

        private Panel ConfigurarBotonRecargar()
        {
            var pnl = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };

            btnRecargar.Anchor = AnchorStyles.Right;
            btnRecargar.Location = new Point(pnl.Width - btnRecargar.Width - 12, (pnl.Height - btnRecargar.Height) / 2);
            pnl.Controls.Add(btnRecargar);

            pnl.SizeChanged += (s, e) =>
            {
                btnRecargar.Location = new Point(pnl.Width - btnRecargar.Width - 12, (pnl.Height - btnRecargar.Height) / 2);
            };

            return pnl;
        }

        private void ReubicarControlesAlPrincipal(TableLayoutPanel tblPrincipal)
        {
            tabTickets.Controls.Remove(ucNuevos);
            tabTickets.Controls.Remove(ucUrgentes);
            tabTickets.Controls.Remove(ucEnProceso);
            tabTickets.Controls.Remove(ucCerrados);
            tabTickets.Controls.Remove(ucReabiertos);
            tabTickets.Controls.Remove(pnlFiltros);
            tabTickets.Controls.Remove(btnRecargar);
            tabTickets.Controls.Remove(dgvTickets);

            tabTickets.Controls.Add(tblPrincipal);
        }

        private void ConfigurarFechasYFiltros()
        {
            // Configurar fechas iniciales (desde hace 30 días hasta hoy)
            dtpFechaInicio.Value = DateTime.Today.AddDays(-30);
            dtpFechaFin.Value = DateTime.Today.AddDays(1).AddTicks(-1);

            // Suscribir eventos de filtros de fecha
            dtpFechaInicio.ValueChanged += async (s, e) => { if (!_estaCargando) { ucPaginacion.CurrentPage = 1; await FiltrarTicketsAsync(); } };
            dtpFechaFin.ValueChanged += async (s, e) => { if (!_estaCargando) { ucPaginacion.CurrentPage = 1; await FiltrarTicketsAsync(); } };
        }

        private async Task InicializarCombosFiltrosAsync()
        {
            _estaCargando = true;

            // 1. Estatus
            cmbFiltroEstatus.Items.Clear();
            cmbFiltroEstatus.Items.AddRange(["Todos", "Abierto", "En Proceso", "Cerrado", "Reabierto"]);
            cmbFiltroEstatus.SelectedIndex = 0;

            // 2. Prioridad
            cmbFiltroPrioridad.Items.Clear();
            cmbFiltroPrioridad.Items.AddRange(["Todos", "Alta", "Media", "Baja", "Urgente"]);
            cmbFiltroPrioridad.SelectedIndex = 0;

            // 3. Vista Temporal
            cmbFiltroTemporal.Items.Clear();
            cmbFiltroTemporal.Items.AddRange(["Todos", "Día", "Semana", "Mes", "Año"]);
            cmbFiltroTemporal.SelectedIndex = 0;

            // 4. Técnicos y Admins
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

                cmbFiltroTecnico.DisplayMember = "Nombre";
                cmbFiltroTecnico.ValueMember = "Id";
                cmbFiltroTecnico.DataSource = listaTecnicos;
                cmbFiltroTecnico.SelectedIndex = 0;
            }
            catch (Exception)
            {
                cmbFiltroTecnico.Items.Clear();
                cmbFiltroTecnico.Items.Add("Todos");
                cmbFiltroTecnico.SelectedIndex = 0;
            }

            _estaCargando = false;
        }

        private async Task FiltrarTicketsAsync()
        {
            if (_estaCargando) return;

            var filtros = new TicketFilterDto();

            // 1. Estatus
            var estatusSel = cmbFiltroEstatus.SelectedItem?.ToString();
            if (estatusSel != "Todos")
            {
                filtros.Estatus = estatusSel;
            }

            // 2. Prioridad
            var prioridadSel = cmbFiltroPrioridad.SelectedItem?.ToString();
            if (prioridadSel != "Todos")
            {
                filtros.Prioridad = prioridadSel;
            }

            // 3. Técnico
            if (cmbFiltroTecnico.SelectedValue is int idTecnico && idTecnico > 0)
            {
                filtros.IdTecnico = idTecnico;
            }

            // 4. Usuario Emisor
            var emisor = txtFiltroUsuario.Text.Trim();
            if (!string.IsNullOrWhiteSpace(emisor))
            {
                filtros.UsuarioEmisor = emisor;
            }

            // 5. Vista Temporal
            var tempSel = cmbFiltroTemporal.SelectedItem?.ToString();
            if (tempSel != "Todos" && tempSel != null)
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

            // 6. Rango de Fechas
            filtros.FechaAltaInicio = dtpFechaInicio.Value.Date;
            filtros.FechaAltaFin = dtpFechaFin.Value.Date.AddDays(1).AddTicks(-1);

            var resultadoPaginado = await _ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, ucPaginacion.CurrentPage, ucPaginacion.PageSize);
            ucPaginacion.TotalRecords = resultadoPaginado.TotalCount;

            ActualizarGrid(resultadoPaginado.Items);
            ucPaginacion.ActualizarInterfaz();
        }

        private async void cmbFiltroEstatus_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { ucPaginacion.CurrentPage = 1; await FiltrarTicketsAsync(); } }
        private async void cmbFiltroPrioridad_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { ucPaginacion.CurrentPage = 1; await FiltrarTicketsAsync(); } }
        private async void cmbFiltroTecnico_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { ucPaginacion.CurrentPage = 1; await FiltrarTicketsAsync(); } }
        private async void txtFiltroUsuario_TextChanged(object sender, EventArgs e) { if (!_estaCargando) { ucPaginacion.CurrentPage = 1; await FiltrarTicketsAsync(); } }
        private async void cmbFiltroTemporal_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { ucPaginacion.CurrentPage = 1; await FiltrarTicketsAsync(); } }
        private async void btnLimpiarFiltros_Click(object sender, EventArgs e)
        {
            _estaCargando = true;
            cmbFiltroEstatus.SelectedIndex = 0;
            cmbFiltroPrioridad.SelectedIndex = 0;
            cmbFiltroTecnico.SelectedIndex = 0;
            txtFiltroUsuario.Clear();
            cmbFiltroTemporal.SelectedIndex = 0;
            dtpFechaInicio.Value = DateTime.Today.AddDays(-30);
            dtpFechaFin.Value = DateTime.Today.AddDays(1).AddTicks(-1);
            ucPaginacion.CurrentPage = 1;
            _estaCargando = false;

            await CargarGridCompletoAsync();
        }

        private void btnAbrirReportes_Click(object sender, EventArgs e)
        {
            var modal = _formFactory.Create<frmGeneradorReportes>();
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
            ucPaginacion.CurrentPage = 1;
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

            dgvTickets.DataSource = new SortableBindingList<TicketGridDto>(listaMapeada);

            // Asegurar que ninguna columna se aplaste a un ancho menor al de su título
            foreach (DataGridViewColumn col in dgvTickets.Columns)
            {
                if (col.Visible)
                {
                    col.MinimumWidth = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true);
                }
            }
        }

        public async void ucNuevos_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorSLAAsync(false);
            ActualizarGrid(filtrados);
        }

        private async void ucUrgentes_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorSLAAsync(true);
            ActualizarGrid(filtrados);
        }

        private async void ucEnProceso_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorEstatusAsync(ConstantesEstatus.EN_PROCESO);
            ActualizarGrid(filtrados);
        }

        private async void ucCerrados_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorEstatusAsync(ConstantesEstatus.CERRADO);
            ActualizarGrid(filtrados);
        }

        private async void ucReabiertos_ucIndicadorEvent(object sender, EventArgs e)
        {
            var filtrados = await _ticketService.ObtenerTicketsPorEstatusAsync(ConstantesEstatus.REABIERTO);
            ActualizarGrid(filtrados);
        }

        private async void btnRecargar_Click(object sender, EventArgs e)
        {
            await CargarGridCompletoAsync();
            await CargarKPIsAsync();
        }

        private async void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (int.TryParse(dgvTickets.Rows[e.RowIndex].Cells["Folio"].Value?.ToString(), out int idSeleccionado))
                {
                    using var formulario = _formFactory.CreateTicketDetalle(idSeleccionado);
                    formulario.ShowDialog();

                    // Al cerrar el detalle, recargamos el Dashboard por si hubo cambios
                    await CargarKPIsAsync();
                    await CargarGridCompletoAsync();
                }
            }
        }

        private async void ConfigurarTabsCatalogos()
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
                await ConfigurarTabParaCatalogo(nombre, tipo);
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
                var frm = _formFactory.CreateEditorDinamico(nuevoMovimiento, "Nuevo Movimiento de Almacén");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    await _catalogoService.CrearAsync(nuevoMovimiento);
                    MessageBox.Show("Movimiento registrado con éxito.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _ = CargarDatosCatalogo(typeof(Material), dgv);
                }
            };

            btnKardex.Click += (s, ev) =>
            {
                var frmK = _formFactory.Create<frmKardex>();
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
            var frm = _formFactory.CreateEditorDinamico(nuevaEntidad, $"Crear {tipo.Name}");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var miMetodo = typeof(CatalogoService).GetMethod("CrearAsync")!.MakeGenericMethod(tipo);
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
                        var miMetodo = typeof(CatalogoService).GetMethod("EliminarAsync")!.MakeGenericMethod(tipo);
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

                var frm = _formFactory.CreateEditorDinamico(entidadExistente, $"Editar {tipo.Name}");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        var miMetodo = typeof(CatalogoService).GetMethod("ActualizarAsync")!.MakeGenericMethod(tipo);
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
            var miMetodo = typeof(CatalogoService).GetMethod("ObtenerTodosAsync")!.MakeGenericMethod(tipoEntidad);
            Task task = (Task)miMetodo.Invoke(_catalogoService, null)!;
            await task;

            var resultProp = task.GetType().GetProperty("Result");
            var resultList = resultProp?.GetValue(task);

            if (resultList != null)
            {
                var bindingListType = typeof(SortableBindingList<>).MakeGenericType(tipoEntidad);
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

                    // Asegurar que ninguna columna se aplaste a un ancho menor al de su título
                    col.MinimumWidth = col.GetPreferredWidth(DataGridViewAutoSizeColumnMode.ColumnHeader, true);
                }
            }
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
