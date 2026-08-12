using System.ComponentModel;
using System.Runtime.Versioning;
using HSis.Logic.Constants;
using HSis.Logic.DTOs;
using HSis.UI.Factories;
using HSis.UI.Forms.Dashboards;
using HSis.UI.Helpers;
using HSis.UI.Presenters;

namespace HSis.UI.Forms.Auth
{
    [SupportedOSPlatform("windows")]
    public partial class IniciarSesionForm : Form, IIniciarSesionView
    {
        private readonly IniciarSesionPresenter _presenter;
        private readonly IFabricaFormularios _fabricaFormularios;

        public IniciarSesionForm(IniciarSesionPresenter presenter, IFabricaFormularios fabricaFormularios)
        {
            InitializeComponent();
            _presenter = presenter;
            _fabricaFormularios = fabricaFormularios;
            _presenter.SetView(this);
            InicializarLayoutLogin();
        }

        #region Propiedades de IIniciarSesionView
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string NombreUsuario
        {
            get => txtUsuario.Text;
            set => txtUsuario.Text = value;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Contraseña
        {
            get => txtContraseña.Text;
            set => txtContraseña.Text = value;
        }

        public void LimpiarCredenciales()
        {
            txtUsuario.Clear();
            txtContraseña.Clear();
        }

        public void CargarCredencialesGuardadas(string usuario, string contraseña)
        {
            txtUsuario.Text = usuario;
            txtContraseña.Text = contraseña;
        }

        public void NavegarADashboard(UsuarioDto usuario, string rolNombre)
        {
            Form dashboardForm = (RolUsuarioEnum)(usuario.IdRol ?? (int)RolUsuarioEnum.Administrador) switch
            {
                RolUsuarioEnum.Administrador => (Form)_fabricaFormularios.Crear<DashboardAdminForm>(),
                RolUsuarioEnum.Tecnico => (Form)_fabricaFormularios.Crear<DashboardTecnicoForm>(),
                RolUsuarioEnum.Cliente => (Form)_fabricaFormularios.Crear<DashboardClienteForm>(),
                _ => (Form)_fabricaFormularios.Crear<DashboardAdminForm>()
            };

            dashboardForm.FormClosed += (s, closedArgs) => Application.Exit();
            this.Hide();
            dashboardForm.Show();
        }

        public void MostrarError(string mensaje)
        {
            MessageBox.Show(mensaje, "Error al iniciar sesión", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public void MostrarCargando(bool cargando)
        {
            btnIniciarSesion.Enabled = !cargando;
            this.UseWaitCursor = cargando;
        }
        #endregion

        #region Form Events
        private async void btnIniciarSesion_Click(object? sender, EventArgs e)
        {
            await _presenter.IniciarSesionAsync();
        }

        private void frmIniciarSesion_Load(object? sender, EventArgs e)
        {
            _presenter.CargarCredencialesEnCache();
        }


        #endregion
    }
}
