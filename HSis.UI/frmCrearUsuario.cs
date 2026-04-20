using System;
using System.Linq;
using System.Windows.Forms;
using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmCrearUsuario : Form
    {
        private readonly UsuarioService _service = new UsuarioService();
        private readonly Form? _parentForm;
        private readonly int? _idUsuario;
        private Usuario? _usuarioActual;

        public frmCrearUsuario(Form? parentForm = null, int? idUsuario = null)
        {
            _parentForm = parentForm;
            _idUsuario = idUsuario;
            InitializeComponent();
        }

        private async void frmCrearUsuario_Load(object sender, EventArgs e)
        {
            // Cargar comboboxes desde el servicio de lógica - Async
            var departamentos = await _service.ObtenerDepartamentosAsync();
            cmbDepartamento.DisplayMember = "Nombre";
            cmbDepartamento.ValueMember = "IdDepartamento";
            cmbDepartamento.DataSource = departamentos;

            var puestos = await _service.ObtenerPuestosAsync();
            cmbPuesto.DisplayMember = "Nombre";
            cmbPuesto.ValueMember = "IdPuesto";
            cmbPuesto.DataSource = puestos;

            var sucursales = await _service.ObtenerSucursalesAsync();
            cmbSucursal.DisplayMember = "Nombre";
            cmbSucursal.ValueMember = "IdSucursal";
            cmbSucursal.DataSource = sucursales;

            if (_idUsuario.HasValue)
            {
                this.Text = "Editar Usuario";
                _usuarioActual = await _service.ObtenerUsuarioPorIdAsync(_idUsuario.Value);
                if (_usuarioActual != null)
                {
                    txtNombre.Text = _usuarioActual.Nombre;
                    cmbDepartamento.SelectedValue = _usuarioActual.IdDepartamento ?? -1;
                    cmbPuesto.SelectedValue = _usuarioActual.IdPuesto ?? -1;
                    cmbSucursal.SelectedValue = _usuarioActual.IdSucursal ?? -1;
                }
            }
        }

        private async void btnGuardar_Click(object sender, EventArgs e)
        {
            var nombre = txtNombre.Text?.Trim();
            var contraseña = txtContraseña.Text ?? string.Empty;

            if (string.IsNullOrWhiteSpace(nombre))
            {
                MessageBox.Show("El nombre es requerido.", "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtNombre.Focus();
                return;
            }

            try
            {
                if (_idUsuario.HasValue && _usuarioActual != null)
                {
                    _usuarioActual.Nombre = nombre!;
                    _usuarioActual.IdDepartamento = cmbDepartamento.SelectedValue != null ? (int?)Convert.ToInt32(cmbDepartamento.SelectedValue) : null;
                    _usuarioActual.IdPuesto = cmbPuesto.SelectedValue != null ? (int?)Convert.ToInt32(cmbPuesto.SelectedValue) : null;
                    _usuarioActual.IdSucursal = cmbSucursal.SelectedValue != null ? (int?)Convert.ToInt32(cmbSucursal.SelectedValue) : null;
                    
                    if (!string.IsNullOrWhiteSpace(contraseña))
                    {
                        _usuarioActual.Contraseña = UsuarioService.HashPassword(contraseña);
                    }

                    await _service.ActualizarUsuarioAsync(_usuarioActual);
                    MessageBox.Show("Usuario actualizado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else
                {
                    var usuario = new Usuario
                    {
                        Nombre = nombre!, // validado no nulo/whitespace arriba
                        IdDepartamento = cmbDepartamento.SelectedValue != null ? (int?)Convert.ToInt32(cmbDepartamento.SelectedValue) : null,
                        IdPuesto = cmbPuesto.SelectedValue != null ? (int?)Convert.ToInt32(cmbPuesto.SelectedValue) : null,
                        IdSucursal = cmbSucursal.SelectedValue != null ? (int?)Convert.ToInt32(cmbSucursal.SelectedValue) : null,
                        IdRol = 3, // Asignar rol de usuario por defecto (3 = Usuario)
                        Contraseña = contraseña,
                    };

                    await _service.CrearUsuarioAsync(usuario);
                    MessageBox.Show("Usuario creado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                this.DialogResult = DialogResult.OK;
                this.Close();
                _parentForm?.Show();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar el usuario: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
            _parentForm?.Show();
        }
    }
}
