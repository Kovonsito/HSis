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

        public frmIniciarSesion()
        {
            InitializeComponent();
            _usuarioService = new UsuarioService();
        }

        private async void btnIniciarSesion_Click(object sender, EventArgs e)
        {
            var usuario = await _usuarioService.AutenticarAsync(txtUsuario.Text, txtContraseña.Text);
            if (usuario != null)
            {
                SesionSistema.UsuarioActual = usuario;

                Form dashboardForm = SesionSistema.IdRolUsuario switch
                {
                    1 => new frmDashboardAdmin(),
                    2 => new frmDashboardTecnico(),
                    3 => new frmDashboardCliente(),
                    _ => new frmDashboardAdmin()
                };

                // Suscribirse al evento FormClosed para cerrar la aplicación correctamente
                dashboardForm.FormClosed += (s, closedArgs) => Application.Exit();

                this.Hide();
                dashboardForm.Show();
            }
            else
            {
                MessageBox.Show("Usuario o contraseña incorrectos", "Error al iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtUsuario.Clear();
                txtContraseña.Clear();
            }
        }

        private async void frmIniciarSesion_Load(object sender, EventArgs e)
        {
            //await _usuarioService.RehashearContraseñasAsync();
        }
    }
}
