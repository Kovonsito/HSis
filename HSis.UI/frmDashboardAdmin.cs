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
        private bool _estaCargando = true;
        private int _paginaActual = 1;
        private int _tamanhoPagina = 10;
        private int _totalRegistros = 0;

        public frmDashboardAdmin(TicketService ticketService, CatalogoService catalogoService, UsuarioService usuarioService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _catalogoService = catalogoService;
            _usuarioService = usuarioService;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            SesionSistema.ConfigurarMenuSesion(this);

            ConfigurarPaginacionYFechas();

            // Cargar los combos de filtros antes del grid
            await InicializarCombosFiltrosAsync();

            // Cargamos KPIs y Grid de tickets en paralelo
            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());

            ConfigurarTabsCatalogos();
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

            await Task.WhenAll(taskNuevos, taskUrgentes, taskEnProceso, taskCerrados, taskReabiertos);

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
            //ucReabiertos.ImagenFondo = Properties.Resources.Reabierto;
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
    }
}
