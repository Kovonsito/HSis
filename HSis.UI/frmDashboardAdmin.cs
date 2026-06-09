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
        private readonly TicketService _ticketService;
        private readonly CatalogoService _catalogoService;
        private readonly UsuarioService _usuarioService;
        private readonly NotificationClientService _notificationClient;
        private bool _estaCargando = true;
        private int _paginaActual = 1;
        private int _tamanhoPagina = 10;
        private int _totalRegistros = 0;

        private Panel? _pnlResilienceBanner;
        private Label? _lblResilienceBanner;
        private ucIndicador? _ucCalificacion;

        private readonly NotificacionStorageService _storageService;
        private Panel? pnlNotificacionesHistorial;
        private FlowLayoutPanel? flpNotificaciones;
        private ToolStripMenuItem? menuCampanaItem;
        private int _notificacionesNoLeidas = 0;

        public frmDashboardAdmin(TicketService ticketService, CatalogoService catalogoService, UsuarioService usuarioService, NotificationClientService notificationClient, NotificacionStorageService storageService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _catalogoService = catalogoService;
            _usuarioService = usuarioService;
            _notificationClient = notificationClient;
            _storageService = storageService;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            InicializarBannerResiliencia();
            InicializarLayoutTicketsAdmin();
            SesionSistema.ConfigurarMenuSesion(this);
            AjustarZOrderControles();

            // Configurar campana en el menú
            var menu = this.MainMenuStrip;
            if (menu != null)
            {
                menuCampanaItem = new ToolStripMenuItem("🔔 (0)");
                menuCampanaItem.Alignment = ToolStripItemAlignment.Right;
                menuCampanaItem.Click += BtnCampana_Click;
                menu.Items.Add(menuCampanaItem);
            }

            // Suscribirse a los eventos de SignalR
            _notificationClient.OnNotificationReceived += OnNotificationReceived;
            _notificationClient.OnReconnecting += OnReconnecting;
            _notificationClient.OnConnected += OnConnected;
            _notificationClient.OnDisconnected += OnDisconnected;

            // Limpieza al cerrar formulario
            this.FormClosed += (s, args) =>
            {
                _notificationClient.OnNotificationReceived -= OnNotificationReceived;
                _notificationClient.OnReconnecting -= OnReconnecting;
                _notificationClient.OnConnected -= OnConnected;
                _notificationClient.OnDisconnected -= OnDisconnected;
            };

            // Establecer estado inicial según la conexión
            ActualizarEstadoConexion(_notificationClient.IsConnected, "⚠️ Conectando al servidor de notificaciones...", Color.FromArgb(230, 126, 34));

            ConfigurarPaginacionYFechas();

            // Cargar los combos de filtros antes del grid
            await InicializarCombosFiltrosAsync();

            // Cargamos KPIs y Grid de tickets en paralelo
            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());

            ConfigurarTabsCatalogos();
            await CargarHistorialNotificacionesAsync();
        }

        private void InicializarLayoutTicketsAdmin()
        {
            // 1. Crear el TableLayoutPanel principal que ocupará la TabPage de Tickets
            var tblPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblPrincipalTickets",
                RowCount = 5,
                ColumnCount = 1,
                Size = tabTickets.ClientSize
            };

            // Definir filas del grid principal
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 110F)); // Fila 0: 6 Indicadores
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // Fila 1: Panel de Filtros
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 35F));  // Fila 2: Botón de Recargar
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));  // Fila 3: Grid (dgvTickets)
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));  // Fila 4: Panel de Paginación

            // 2. Crear el TableLayoutPanel para los 6 indicadores (6 columnas, 16.6% cada una)
            var tblIndicadores = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                Name = "tblIndicadoresAdmin",
                RowCount = 1,
                ColumnCount = 6,
                Margin = new Padding(0)
            };
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));
            tblIndicadores.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16.66F));

            // Instanciar el control de calificación
            _ucCalificacion = new ucIndicador();
            _ucCalificacion.Dock = DockStyle.Fill;
            _ucCalificacion.Margin = new Padding(5);
            _ucCalificacion.ucIndicadorEvent += UcCalificacion_Click;

            // Configurar los indicadores para que ocupen su celda
            ucNuevos.Dock = DockStyle.Fill;
            ucUrgentes.Dock = DockStyle.Fill;
            ucEnProceso.Dock = DockStyle.Fill;
            ucCerrados.Dock = DockStyle.Fill;
            ucReabiertos.Dock = DockStyle.Fill;

            ucNuevos.Margin = new Padding(5);
            ucUrgentes.Margin = new Padding(5);
            ucEnProceso.Margin = new Padding(5);
            ucCerrados.Margin = new Padding(5);
            ucReabiertos.Margin = new Padding(5);

            tblIndicadores.Controls.Add(ucNuevos, 0, 0);
            tblIndicadores.Controls.Add(ucUrgentes, 1, 0);
            tblIndicadores.Controls.Add(ucEnProceso, 2, 0);
            tblIndicadores.Controls.Add(ucCerrados, 3, 0);
            tblIndicadores.Controls.Add(ucReabiertos, 4, 0);
            tblIndicadores.Controls.Add(_ucCalificacion, 5, 0);

            // 3. Panel de filtros
            pnlFiltros.Dock = DockStyle.Fill;
            pnlFiltros.Margin = new Padding(5);

            // 4. Botón de Recargar alineado a la derecha
            var pnlRecargar = new Panel
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            btnRecargar.Anchor = AnchorStyles.Right;
            btnRecargar.Location = new Point(pnlRecargar.Width - btnRecargar.Width - 12, (pnlRecargar.Height - btnRecargar.Height) / 2);
            pnlRecargar.Controls.Add(btnRecargar);

            pnlRecargar.SizeChanged += (s, e) =>
            {
                btnRecargar.Location = new Point(pnlRecargar.Width - btnRecargar.Width - 12, (pnlRecargar.Height - btnRecargar.Height) / 2);
            };

            // 5. Grid principal
            dgvTickets.Dock = DockStyle.Fill;
            dgvTickets.Margin = new Padding(5);

            // 6. Panel de paginación
            pnlPaginacion.Dock = DockStyle.Fill;
            pnlPaginacion.Margin = new Padding(5, 0, 5, 0);

            // Agregar componentes al TableLayoutPanel principal
            tblPrincipal.Controls.Add(tblIndicadores, 0, 0);
            tblPrincipal.Controls.Add(pnlFiltros, 0, 1);
            tblPrincipal.Controls.Add(pnlRecargar, 0, 2);
            tblPrincipal.Controls.Add(dgvTickets, 0, 3);
            tblPrincipal.Controls.Add(pnlPaginacion, 0, 4);

            // Remover de la tabTickets original para agregarlos al grid principal
            tabTickets.Controls.Remove(ucNuevos);
            tabTickets.Controls.Remove(ucUrgentes);
            tabTickets.Controls.Remove(ucEnProceso);
            tabTickets.Controls.Remove(ucCerrados);
            tabTickets.Controls.Remove(ucReabiertos);
            tabTickets.Controls.Remove(pnlFiltros);
            tabTickets.Controls.Remove(btnRecargar);
            tabTickets.Controls.Remove(dgvTickets);
            tabTickets.Controls.Remove(pnlPaginacion);

            // Agregar el grid principal a la pestaña
            tabTickets.Controls.Add(tblPrincipal);
        }

        private void ConfigurarPaginacionYFechas()
        {
            // Inicializar Page Size ComboBox
            cmbPageSize.Items.Clear();
            cmbPageSize.Items.AddRange(["10", "20", "50", "100"]);
            cmbPageSize.SelectedIndex = 0; // Default: 10
            _tamanhoPagina = 10;

            // Configurar fechas iniciales (desde hace 30 días hasta hoy)
            dtpFechaInicio.Value = DateTime.Today.AddDays(-30);
            dtpFechaFin.Value = DateTime.Today.AddDays(1).AddTicks(-1);

            // Suscribir eventos de filtros de fecha
            dtpFechaInicio.ValueChanged += async (s, e) => { if (!_estaCargando) { _paginaActual = 1; await FiltrarTicketsAsync(); } };
            dtpFechaFin.ValueChanged += async (s, e) => { if (!_estaCargando) { _paginaActual = 1; await FiltrarTicketsAsync(); } };

            cmbPageSize.SelectedIndexChanged += async (s, e) =>
            {
                if (int.TryParse(cmbPageSize.SelectedItem?.ToString(), out int size))
                {
                    _tamanhoPagina = size;
                    _paginaActual = 1;
                    await FiltrarTicketsAsync();
                }
            };

            btnFirstPage.Click += async (s, e) => { _paginaActual = 1; await FiltrarTicketsAsync(); };
            btnPrevPage.Click += async (s, e) => { if (_paginaActual > 1) { _paginaActual--; await FiltrarTicketsAsync(); } };
            btnNextPage.Click += async (s, e) => { if (_paginaActual < ObtenerTotalPaginas()) { _paginaActual++; await FiltrarTicketsAsync(); } };
            btnLastPage.Click += async (s, e) => { _paginaActual = ObtenerTotalPaginas(); await FiltrarTicketsAsync(); };
        }

        private int ObtenerTotalPaginas()
        {
            if (_totalRegistros <= 0) return 1;
            return (int)Math.Ceiling((double)_totalRegistros / _tamanhoPagina);
        }

        private void ActualizarControlesPaginacion()
        {
            int totalPaginas = ObtenerTotalPaginas();
            if (_paginaActual > totalPaginas) _paginaActual = totalPaginas;
            if (_paginaActual < 1) _paginaActual = 1;

            lblPageInfo.Text = $"Página {_paginaActual} de {totalPaginas}";
            lblTotalTickets.Text = $"Total: {_totalRegistros} tickets";

            btnFirstPage.Enabled = _paginaActual > 1;
            btnPrevPage.Enabled = _paginaActual > 1;
            btnNextPage.Enabled = _paginaActual < totalPaginas;
            btnLastPage.Enabled = _paginaActual < totalPaginas;
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

            var resultadoPaginado = await _ticketService.ObtenerTicketsFiltradosPaginadosAsync(filtros, _paginaActual, _tamanhoPagina);
            _totalRegistros = resultadoPaginado.TotalCount;

            ActualizarGrid(resultadoPaginado.Items);
            ActualizarControlesPaginacion();
        }

        private async void cmbFiltroEstatus_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { _paginaActual = 1; await FiltrarTicketsAsync(); } }
        private async void cmbFiltroPrioridad_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { _paginaActual = 1; await FiltrarTicketsAsync(); } }
        private async void cmbFiltroTecnico_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { _paginaActual = 1; await FiltrarTicketsAsync(); } }
        private async void txtFiltroUsuario_TextChanged(object sender, EventArgs e) { if (!_estaCargando) { _paginaActual = 1; await FiltrarTicketsAsync(); } }
        private async void cmbFiltroTemporal_SelectedIndexChanged(object sender, EventArgs e) { if (!_estaCargando) { _paginaActual = 1; await FiltrarTicketsAsync(); } }
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
            _paginaActual = 1;
            _estaCargando = false;

            await CargarGridCompletoAsync();
        }

        private void btnAbrirReportes_Click(object sender, EventArgs e)
        {
            var modal = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmGeneradorReportes>(Program.ServiceProvider);
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
            _paginaActual = 1;
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

        private void dgvTickets_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                // Obtenemos el valor de la celda "Folio" de manera segura
                if (int.TryParse(dgvTickets.Rows[e.RowIndex].Cells["Folio"].Value?.ToString(), out int idSeleccionado))
                {
                    // Pasamos el ID al constructor del formulario a través de Inyección de Dependencias
                    frmTicketDetalle formulario = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmTicketDetalle>(Program.ServiceProvider, idSeleccionado);
                    formulario.ShowDialog();

                    // Al cerrar el detalle, recargamos el Dashboard por si hubo cambios
                    _ = CargarKPIsAsync();
                    _ = CargarGridCompletoAsync();
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

            foreach (var cat in catalogos)
            {
                await ConfigurarTabParaCatalogo(cat.Nombre, cat.Tipo);
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
                var frm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmEditorDinamico>(Program.ServiceProvider, nuevoMovimiento, "Nuevo Movimiento de Almacén");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    await _catalogoService.CrearAsync(nuevoMovimiento);
                    MessageBox.Show("Movimiento registrado con éxito.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    _ = CargarDatosCatalogo(typeof(Material), dgv);
                }
            };

            btnKardex.Click += (s, ev) =>
            {
                var frmK = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmKardex>(Program.ServiceProvider);
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
            var frm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmEditorDinamico>(Program.ServiceProvider, nuevaEntidad, $"Crear {tipo.Name}");
            if (frm.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    if (tipo == typeof(Usuario))
                    {
                        var u = (Usuario)nuevaEntidad;
                        if (!string.IsNullOrEmpty(u.Contraseña))
                        {
                            u.Contraseña = UsuarioService.HashPassword(u.Contraseña);
                        }
                    }
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

                if (Program.ServiceProvider is null) return;

                var frm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmEditorDinamico>(Program.ServiceProvider, entidadExistente, $"Editar {tipo.Name}");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        if (tipo == typeof(Usuario))
                        {
                            var u = (Usuario)entidadExistente;
                            if (string.IsNullOrWhiteSpace(u.Contraseña))
                            {
                                u.Contraseña = passwordHashOriginal;
                            }
                            else
                            {
                                u.Contraseña = UsuarioService.HashPassword(u.Contraseña);
                            }
                        }
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

            // --- DIAGNÓSTICO TEMPORAL ---
            if (tipoEntidad.Name == "Usuario")
            {
                var debugLines = new List<string>();
                debugLines.Add($"=== Columnas para Usuario ===");
                foreach (DataGridViewColumn col in dgv.Columns)
                {
                    bool isGeneric = col.ValueType?.IsGenericType == true;
                    string isNullable = col.ValueType != null && col.ValueType.IsGenericType && col.ValueType.GetGenericTypeDefinition() == typeof(Nullable<>) ? "Sí" : "No";
                    debugLines.Add($"Columna: {col.Name} | Tipo: {col.ValueType?.Name ?? "null"} | Genérico: {isGeneric} | Nullable: {isNullable} | Visible original: {col.Visible}");
                }
                try
                {
                    System.IO.File.WriteAllLines(@"c:\HSis\debug_columns.txt", debugLines);
                }
                catch { }
            }
            // -----------------------------

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
                        col.HeaderText = col.Name.Substring(2); // Ejemplo: "IdDepartamento" se lee como "Departamento"
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

        private void InicializarBannerResiliencia()
        {
            _pnlResilienceBanner = new Panel
            {
                Dock = DockStyle.Top,
                Height = 35,
                BackColor = Color.FromArgb(231, 76, 60), // Rojo
                Visible = false
            };

            _lblResilienceBanner = new Label
            {
                Dock = DockStyle.Fill,
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleCenter,
                Text = "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar..."
            };

            _pnlResilienceBanner.Controls.Add(_lblResilienceBanner);
            this.Controls.Add(_pnlResilienceBanner);
            _pnlResilienceBanner.BringToFront();

            // Crear Panel de Notificaciones (Flotante)
            pnlNotificacionesHistorial = new Panel
            {
                Width = 300,
                Height = this.ClientSize.Height - 35,
                Location = new Point(this.ClientSize.Width - 300, 35),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right,
                BackColor = Color.FromArgb(245, 247, 250),
                Visible = false,
                Padding = new Padding(10)
            };

            var pnlNotifHeader = new Panel
            {
                Dock = DockStyle.Top,
                Height = 40,
                Padding = new Padding(0, 0, 0, 5)
            };
            var lblNotifTitle = new Label
            {
                Text = "Notificaciones",
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                Dock = DockStyle.Left,
                AutoSize = true
            };
            var btnLimpiarNotif = new Button
            {
                Text = "Limpiar todo",
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Dock = DockStyle.Right,
                Width = 95,
                BackColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            btnLimpiarNotif.FlatAppearance.BorderSize = 1;
            btnLimpiarNotif.FlatAppearance.BorderColor = Color.LightGray;
            btnLimpiarNotif.Click += BtnLimpiarNotif_Click;

            pnlNotifHeader.Controls.Add(lblNotifTitle);
            pnlNotifHeader.Controls.Add(btnLimpiarNotif);
            pnlNotificacionesHistorial.Controls.Add(pnlNotifHeader);

            flpNotificaciones = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                Padding = new Padding(0, 5, 0, 0)
            };
            flpNotificaciones.SizeChanged += (s, e) =>
            {
                foreach (Control ctrl in flpNotificaciones.Controls)
                {
                    ctrl.Width = flpNotificaciones.ClientSize.Width - 10;
                }
            };
            pnlNotificacionesHistorial.Controls.Add(flpNotificaciones);

            pnlNotifHeader.SendToBack();
            flpNotificaciones.BringToFront();

            this.Controls.Add(pnlNotificacionesHistorial);
        }

        private void AjustarZOrderControles()
        {
            if (pnlNotificacionesHistorial != null)
            {
                this.Controls.SetChildIndex(pnlNotificacionesHistorial, 0); // Al frente de todo (flotante)
            }
            var tabMain = this.Controls["tabMain"];
            if (tabMain != null)
            {
                this.Controls.SetChildIndex(tabMain, 1); // Debajo de pnlNotificacionesHistorial
            }
            if (_pnlResilienceBanner != null)
            {
                this.Controls.SetChildIndex(_pnlResilienceBanner, 2); // Debajo de tabMain
            }
            foreach (Control ctrl in this.Controls)
            {
                if (ctrl is MenuStrip)
                {
                    this.Controls.SetChildIndex(ctrl, 3); // Al fondo en lógica de layout (se queda hasta arriba)
                    break;
                }
            }
        }

        private async void OnNotificationReceived(string tipo, int ticketId, string mensaje)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => OnNotificationReceived(tipo, ticketId, mensaje)));
                return;
            }

            // Guardar persistentemente
            await _storageService.GuardarNotificacionAsync(SesionSistema.IdUsuario, ticketId, mensaje);

            MessageBox.Show(mensaje, "Notificación de HSis", MessageBoxButtons.OK, MessageBoxIcon.Information);
            
            _ = Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());
            _ = CargarHistorialNotificacionesAsync();
        }

        private async Task CargarHistorialNotificacionesAsync()
        {
            if (flpNotificaciones == null) return;

            flpNotificaciones.Controls.Clear();
            var list = await _storageService.ObtenerNotificacionesAsync(SesionSistema.IdUsuario);

            _notificacionesNoLeidas = list.Count(n => !n.Leido);
            ActualizarCampanaBadge();

            foreach (var notif in list)
            {
                AgregarNotificacionAUI(notif);
            }
        }

        private void ActualizarCampanaBadge()
        {
            if (menuCampanaItem != null)
            {
                menuCampanaItem.Text = _notificacionesNoLeidas > 0 ? $"🔔 ({_notificacionesNoLeidas}) 🔴" : $"🔔 ({_notificacionesNoLeidas})";
                menuCampanaItem.ForeColor = _notificacionesNoLeidas > 0 ? Color.Red : Color.Black;
                menuCampanaItem.Font = new Font("Segoe UI", 10F, _notificacionesNoLeidas > 0 ? FontStyle.Bold : FontStyle.Regular);
            }
        }

        private void AgregarNotificacionAUI(NotificacionLocal notif)
        {
            if (flpNotificaciones == null) return;

            var pnlItem = new Panel
            {
                Width = flpNotificaciones.ClientSize.Width - 10,
                Height = 85,
                BackColor = notif.Leido ? Color.White : Color.FromArgb(235, 245, 251),
                Padding = new Padding(8),
                Margin = new Padding(0, 0, 0, 8),
                Cursor = Cursors.Hand
            };

            pnlItem.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlItem.ClientRectangle, Color.FromArgb(220, 224, 230), ButtonBorderStyle.Solid);
                if (!notif.Leido)
                {
                    using var brush = new SolidBrush(Color.FromArgb(52, 152, 219));
                    e.Graphics.FillEllipse(brush, pnlItem.Width - 15, 8, 8, 8);
                }
            };

            var lblMsg = new Label
            {
                Text = notif.Mensaje,
                Font = new Font("Segoe UI", 9.5F, notif.Leido ? FontStyle.Regular : FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(8, 8),
                Size = new Size(pnlItem.Width - 25, 50),
                AutoEllipsis = true
            };

            var lblFecha = new Label
            {
                Text = notif.Fecha.ToString("g"),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(8, 60),
                Size = new Size(pnlItem.Width - 20, 18)
            };

            lblMsg.Click += (s, e) => AbrirDetalleYMarcarLeido(notif, pnlItem);
            lblFecha.Click += (s, e) => AbrirDetalleYMarcarLeido(notif, pnlItem);
            pnlItem.Click += (s, e) => AbrirDetalleYMarcarLeido(notif, pnlItem);

            pnlItem.Controls.Add(lblMsg);
            pnlItem.Controls.Add(lblFecha);

            flpNotificaciones.Controls.Add(pnlItem);
        }

        private async void AbrirDetalleYMarcarLeido(NotificacionLocal notif, Panel pnlItem)
        {
            if (!notif.Leido)
            {
                await _storageService.MarcarComoLeidaAsync(SesionSistema.IdUsuario, notif.Id);
                notif.Leido = true;
                pnlItem.BackColor = Color.White;
                foreach (Control ctrl in pnlItem.Controls)
                {
                    if (ctrl is Label lbl && lbl.Text == notif.Mensaje)
                    {
                        lbl.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
                    }
                }
                _notificacionesNoLeidas = Math.Max(0, _notificacionesNoLeidas - 1);
                ActualizarCampanaBadge();
                pnlItem.Invalidate();
            }

            using var frmTicket = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmTicketDetalle>(Program.ServiceProvider, notif.TicketId);
            frmTicket.ShowDialog();
            _ = Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());
        }

        private async void BtnLimpiarNotif_Click(object? sender, EventArgs e)
        {
            await _storageService.LimpiarTodasAsync(SesionSistema.IdUsuario);
            _notificacionesNoLeidas = 0;
            ActualizarCampanaBadge();
            flpNotificaciones?.Controls.Clear();
        }

        private void BtnCampana_Click(object? sender, EventArgs e)
        {
            if (pnlNotificacionesHistorial != null)
            {
                pnlNotificacionesHistorial.Visible = !pnlNotificacionesHistorial.Visible;
                if (pnlNotificacionesHistorial.Visible)
                {
                    pnlNotificacionesHistorial.BringToFront();
                }
            }
        }

        private void OnReconnecting()
        {
            ActualizarEstadoConexion(false, "⚠️ Intentando reconectar con el servidor de notificaciones...", Color.FromArgb(230, 126, 34));
        }

        private void OnConnected()
        {
            ActualizarEstadoConexion(true, string.Empty, Color.Empty);
            _ = Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());
        }

        private void OnDisconnected()
        {
            ActualizarEstadoConexion(false, "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar...", Color.FromArgb(231, 76, 60));
        }

        private void ActualizarEstadoConexion(bool conectado, string mensaje, Color colorFondo)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ActualizarEstadoConexion(conectado, mensaje, colorFondo)));
                return;
            }

            if (_pnlResilienceBanner != null && _lblResilienceBanner != null)
            {
                _pnlResilienceBanner.Visible = !conectado;
                _lblResilienceBanner.Text = mensaje;
                _pnlResilienceBanner.BackColor = colorFondo;
            }
        }
    }
}
