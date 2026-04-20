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

        public frmDashboardAdmin()
        {
            InitializeComponent();
            _ticketService = new TicketService();
            _usuarioService = new UsuarioService();
            _catalogoService = new CatalogoService();
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            SesionSistema.ConfigurarMenuSesion(this);
            // Cargamos KPIs, Grid de tickets y Grid de usuarios en paralelo
            await Task.WhenAll(CargarKPIsAsync(), CargarGridCompletoAsync(), CargarUsuariosAsync());
            
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
                    // Pasamos el ID al constructor del formulario
                    frmTicketDetalle formulario = new frmTicketDetalle(idSeleccionado);
                    formulario.ShowDialog();

                    // Al cerrar el detalle, recargamos el Dashboard por si hubo cambios
                    _ = CargarKPIsAsync();
                    _ = CargarGridCompletoAsync();
                }
            }
        }

        private async Task CargarUsuariosAsync()
        {
            var usuarios = await _usuarioService.ObtenerUsuariosAsync();
            var listaMapeada = usuarios.Select(u => new
            {
                IdUsuario = u.IdUsuario,
                Nombre = u.Nombre,
                Departamento = u.IdDepartamentoNavigation?.Nombre ?? "N/A",
                Puesto = u.IdPuestoNavigation?.Nombre ?? "N/A",
                Sucursal = u.IdSucursalNavigation?.Nombre ?? "N/A",
                Rol = u.IdRolNavigation?.Descripción ?? "N/A"
            }).ToList();

            dgvUsuarios.DataSource = listaMapeada;
        }

        private async void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            var frm = new frmCrearUsuario(this);
            if (frm.ShowDialog() == DialogResult.OK)
            {
                await CargarUsuariosAsync();
            }
        }

        private async void dgvUsuarios_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (int.TryParse(dgvUsuarios.Rows[e.RowIndex].Cells["IdUsuario"].Value?.ToString(), out int idSeleccionado))
                {
                    var frm = new frmCrearUsuario(this, idSeleccionado);
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
                        await CargarUsuariosAsync();
                    }
                }
            }
        }

        private async void ConfigurarTabsCatalogos()
        {
            var catalogos = new (string Nombre, Type Tipo)[] { 
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
                
                tab.Controls.Add(dgv);
                tab.Controls.Add(panelTop);
                tabMain.TabPages.Add(tab);

                // Cargar datos inicialmente
                await CargarDatosCatalogo(cat.Tipo, dgv);

                // Eventos
                btnCrear.Click += async (s, e) => {
                    object nuevaEntidad = Activator.CreateInstance(cat.Tipo)!;
                    var frm = new frmEditorDinamico(nuevaEntidad, $"Crear {cat.Nombre}");
                    if (frm.ShowDialog() == DialogResult.OK)
                    {
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
                        var frm = new frmEditorDinamico(entidadExistente, $"Editar {cat.Nombre}");
                        if (frm.ShowDialog() == DialogResult.OK)
                        {
                            var miMetodo = typeof(CatalogoService).GetMethod("ActualizarAsync")!.MakeGenericMethod(cat.Tipo);
                            Task task = (Task)miMetodo.Invoke(_catalogoService, new object[] { entidadExistente })!;
                            await task;
                            await CargarDatosCatalogo(cat.Tipo, dgv);
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
            
            // Ocultar columnas de navegación si se generaron
            foreach(DataGridViewColumn col in dgv.Columns)
            {
                if (col.Name.EndsWith("Navigation") || col.Name == "Usuarios" || col.Name == "Sucursals" || col.Name == "DetTickets")
                {
                    col.Visible = false;
                }
            }
        }
    }
}

