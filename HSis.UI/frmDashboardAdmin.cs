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
        private readonly UsuarioService _usuarioService;
        private readonly CatalogoService _catalogoService;

        public frmDashboardAdmin(TicketService ticketService, UsuarioService usuarioService, CatalogoService catalogoService)
        {
            InitializeComponent();
            _ticketService = ticketService;
            _usuarioService = usuarioService;
            _catalogoService = catalogoService;
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            SesionSistema.ConfigurarMenuSesion(this);
            // Cargamos KPIs y Grid de tickets en paralelo
            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync());

            ConfigurarTabsCatalogos();
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
            var todos = await _ticketService.ObtenerTicketsAsync();
            ActualizarGrid(todos);
        }

        private void ActualizarGrid(List<TicketDto> listaTickets)
        {
            var listaMapeada = listaTickets.Select(t => new TicketGridDto
            {
                Folio = t.IdTicket,
                NombreUsuario = t.NombreUsuario,
                Status = t.Status ?? "N/A",
                Alta = t.Alta,
                Atención = t.Atencion,
                Cierre = t.Cierre ?? DateTime.Now,
                AtendidoPor = t.NombreTecnico,
                Descripción = t.Descripcion ?? "N/A",
                Solución = t.Solucion ?? "N/A"
            }).ToList();

            dgvTickets.DataSource = new SortableBindingList<TicketGridDto>(listaMapeada);
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
            TabPage tab = new TabPage(nombre);

            DataGridView dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                Name = "dgv" + nombre,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                MultiSelect = false
            };

            Panel panelTop = new Panel { Dock = DockStyle.Top, Height = 40 };
            Button btnCrear = new Button { Text = "Crear", Location = new Point(10, 10), Width = 100 };
            Button btnEliminar = new Button { Text = "Eliminar", Location = new Point(120, 10), Width = 100 };

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
            Button btnIngreso = new Button { Text = "Nuevo Ingreso", Location = new Point(230, 10), Width = 120 };
            Button btnKardex = new Button { Text = "Ver Kardex", Location = new Point(360, 10), Width = 100 };

            btnIngreso.Click += async (s, ev) =>
            {
                var nuevoIngreso = new IngresosMaterial
                {
                    IdUsuario = SesionSistema.IdUsuario,
                    FechaIngreso = DateTime.Now,
                    Cantidad = 1
                };
                var frm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmEditorDinamico>(Program.ServiceProvider, nuevoIngreso, "Nuevo Ingreso de Almacén");
                if (frm.ShowDialog() == DialogResult.OK)
                {
                    await _catalogoService.CrearAsync<IngresosMaterial>(nuevoIngreso);
                    MessageBox.Show("Ingreso registrado con éxito.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        private void ConfigurarFormateoDeCeldas(DataGridView dgv, Type tipo)
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
                    Task task = (Task)miMetodo.Invoke(_catalogoService, new object[] { nuevaEntidad })!;
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
                        Task task = (Task)miMetodo.Invoke(_catalogoService, new object[] { idObj })!;
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
            foreach (DataGridViewColumn col in dgv.Columns)
            {
                if (col.Name.EndsWith("Navigation") || col.ValueType?.IsGenericType == true)
                {
                    col.Visible = false; // Ocultar objetos virtuales de navegación y colecciones
                }
                else if (col.Name.StartsWith("Id") && col.Name != idPk)
                {
                    col.HeaderText = col.Name.Substring(2); // Ejemplo: "IdDepartamento" se lee como "Departamento"
                }
            }
        }
    }
}

