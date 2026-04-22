using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.Xml.Serialization;
using HSis.Data.Models;
using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI
{
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

        private void ActualizarGrid(List<Ticket> listaTickets)
        {
            var listaMapeada = listaTickets.Select(t => new TicketGridDto
            {
                Folio = t.IdTicket,
                NombreUsuario = t.IdUsuarioNavigation?.Nombre ?? "N/A",
                Status = t.Status ?? "N/A",
                Alta = t.Alta,
                Atención = t.Atención,
                Cierre = t.Cierre ?? DateTime.Now,
                AtendidoPor = t.IdTecnicoNavigation != null ? t.IdTecnicoNavigation.Nombre : "N/A",
                Descripción = t.Descripción ?? "N/A",
                Solución = t.Solución ?? "N/A"
            }).ToList();

            dgvTickets.DataSource = listaMapeada;
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
                TabPage tab = new TabPage(cat.Nombre);
                
                DataGridView dgv = new DataGridView { 
                    Dock = DockStyle.Fill, 
                    Name = "dgv" + cat.Nombre,
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

                if (cat.Tipo == typeof(Material))
                {
                    Button btnIngreso = new Button { Text = "Nuevo Ingreso", Location = new Point(230, 10), Width = 120 };
                    Button btnKardex = new Button { Text = "Ver Kardex", Location = new Point(360, 10), Width = 100 };
                    
                    btnIngreso.Click += async (s, ev) => {
                        var nuevoIngreso = new IngresosMaterial { 
                            IdUsuario = SesionSistema.IdUsuario, 
                            FechaIngreso = DateTime.Now,
                            Cantidad = 1 // Valor inicial para evitar división por cero en el editor
                        };
                        var frm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmEditorDinamico>(Program.ServiceProvider, nuevoIngreso, "Nuevo Ingreso de Almacén");
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            await _catalogoService.CrearAsync<IngresosMaterial>(nuevoIngreso);
                            MessageBox.Show("Ingreso registrado con éxito.", "Inventario", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            _ = CargarDatosCatalogo(cat.Tipo, dgv);
                        }
                    };

                    btnKardex.Click += (s, ev) => {
                        var frmK = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmKardex>(Program.ServiceProvider);
                        frmK.ShowDialog();
                    };

                    panelTop.Controls.Add(btnIngreso);
                    panelTop.Controls.Add(btnKardex);
                }
                
                tab.Controls.Add(dgv);
                tab.Controls.Add(panelTop);
                tabMain.TabPages.Add(tab);

                // Formateo de celdas para reemplazar el ID por el Nombre/Descripción de la clase navegada
                dgv.CellFormatting += (s, e) => {
                    if (e.RowIndex >= 0 && e.ColumnIndex >= 0)
                    {
                        var columnName = dgv.Columns[e.ColumnIndex].Name;
                        string idPk = "Id" + (cat.Tipo.Name == "RolUsuario" ? "Rol" : cat.Tipo.Name);
                        
                        if (columnName.StartsWith("Id") && columnName != idPk) // Es un foreign key
                        {
                            var navPropName = columnName + "Navigation";
                            var entidad = dgv.Rows[e.RowIndex].DataBoundItem;
                            if (entidad != null)
                            {
                                var navProp = entidad.GetType().GetProperty(navPropName);
                                if (navProp != null)
                                {
                                    var navObj = navProp.GetValue(entidad);
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
                        }
                    }
                };

                // Cargar datos inicialmente
                await CargarDatosCatalogo(cat.Tipo, dgv);

                // Eventos
                btnCrear.Click += async (s, e) => {
                    object nuevaEntidad = Activator.CreateInstance(cat.Tipo)!;
                    var frm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmEditorDinamico>(Program.ServiceProvider, nuevaEntidad, $"Crear {cat.Nombre}");
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        if (cat.Tipo == typeof(Usuario))
                        {
                            var u = (Usuario)nuevaEntidad;
                            if (!string.IsNullOrEmpty(u.Contraseña))
                            {
                                u.Contraseña = UsuarioService.HashPassword(u.Contraseña);
                            }
                        }
                        var miMetodo = typeof(CatalogoService).GetMethod("CrearAsync")!.MakeGenericMethod(cat.Tipo);
                        Task task = (Task)miMetodo.Invoke(_catalogoService, new object[] { nuevaEntidad })!;
                        await task;
                        await CargarDatosCatalogo(cat.Tipo, dgv);
                    }
                };

                btnEliminar.Click += async (s, e) => {
                    if (dgv.SelectedRows.Count > 0)
                    {
                        var row = dgv.SelectedRows[0];
                        string idName = "Id" + (cat.Tipo.Name == "RolUsuario" ? "Rol" : cat.Tipo.Name);
                        var idObj = row.Cells[idName]?.Value;
                        if (idObj != null && MessageBox.Show("¿Seguro que deseas eliminar el registro?", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                        {
                            try
                            {
                                var miMetodo = typeof(CatalogoService).GetMethod("EliminarAsync")!.MakeGenericMethod(cat.Tipo);
                                Task task = (Task)miMetodo.Invoke(_catalogoService, new object[] { idObj })!;
                                await task;
                                await CargarDatosCatalogo(cat.Tipo, dgv);
                            }
                            catch(Exception ex)
                            {
                                MessageBox.Show("No se pudo eliminar el registro (probablemente esté en uso). " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                };

                dgv.CellDoubleClick += async (s, e) => {
                    if (e.RowIndex >= 0)
                    {
                        object entidadExistente = dgv.Rows[e.RowIndex].DataBoundItem;
                        string passwordHashOriginal = null;
                        if (cat.Tipo == typeof(Usuario))
                        {
                            var u = (Usuario)entidadExistente;
                            passwordHashOriginal = u.Contraseña;
                            u.Contraseña = "";
                        }

                        var frm = Microsoft.Extensions.DependencyInjection.ActivatorUtilities.CreateInstance<frmEditorDinamico>(Program.ServiceProvider, entidadExistente, $"Editar {cat.Nombre}");
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            if (cat.Tipo == typeof(Usuario))
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
                            var miMetodo = typeof(CatalogoService).GetMethod("ActualizarAsync")!.MakeGenericMethod(cat.Tipo);
                            Task task = (Task)miMetodo.Invoke(_catalogoService, new object[] { entidadExistente })!;
                            await task;
                            await CargarDatosCatalogo(cat.Tipo, dgv);
                        }
                        else
                        {
                            if (cat.Tipo == typeof(Usuario))
                            {
                                var u = (Usuario)entidadExistente;
                                u.Contraseña = passwordHashOriginal;
                            }
                        }
                    }
                };
            }
        }

        private async Task CargarDatosCatalogo(Type tipoEntidad, DataGridView dgv)
        {
            var miMetodo = typeof(CatalogoService).GetMethod("ObtenerTodosAsync")!.MakeGenericMethod(tipoEntidad);
            Task task = (Task)miMetodo.Invoke(_catalogoService, null)!;
            await task;
            
            var resultProp = task.GetType().GetProperty("Result");
            var resultList = resultProp?.GetValue(task);
            dgv.DataSource = resultList;
            
            // Ocultar columnas no deseadas y renombrar cabeceras
            string idPk = "Id" + (tipoEntidad.Name == "RolUsuario" ? "Rol" : tipoEntidad.Name);
            foreach(DataGridViewColumn col in dgv.Columns)
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

