using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using HSis.Logic.Services;

namespace HSis.UI
{
    public partial class frmIniciarSesion : Form
    {
        private readonly UsuarioService _usuarioService;
        private frmCrearUsuario _frmCrearUsuario;

        public frmIniciarSesion()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
            _frmCrearUsuario = new frmCrearUsuario(this);
        }

        private async void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            var usuario = await _usuarioService.AutenticarAsync(txtUsuario.Text, txtContraseña.Text);
            if (usuario != null)
            {
                SesionSistema.NombreUsuario = usuario.Nombre ?? string.Empty;
                SesionSistema.IdUsuario = usuario.IdUsuario;
                SesionSistema.IdRolUsuario = usuario.IdRol;

                MessageBox.Show("Inicio de sesión exitoso", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);

                Form dashboardForm = SesionSistema.IdRolUsuario switch
                {
                    1 => new frmDashboardAdmin(),
                    2 => new frmDashboardTecnico(),
                    3 => new frmDashboardCliente(),
                    _ => new frmDashboardAdmin()
                };

                // Suscribirse al evento FormClosed para cerrar la aplicación correctamente
                dashboardForm.FormClosed += (s, closedArgs) => Application.Exit();

                dashboardForm.Show();
                this.Hide();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error al iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Clear();
                txtContraseña.Clear();
            }
        }

        private void btnCrearUsuario_Click(object sender, EventArgs e)
        {
            this.Hide();
            _frmCrearUsuario.ShowDialog();
        }

        private async void frmIniciarSesion_Load(object sender, EventArgs e)
        {
            //await _usuarioService.RehashearContraseñasAsync();
        }
    }
}
