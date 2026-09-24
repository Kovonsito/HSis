#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.UI.Coordinators;
using HSis.UI.Factories;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardAdminForm : Form
    {
        private readonly CoordinadorDashboardAdmin _coordinador;

        public DashboardAdminForm(
            ITicketService ticketService,
            IUsuarioService usuarioService,
            ICatalogoService catalogoService,
            IAdministradorSesionUsuario contextoSesion,
            IAlmacenamientoCredencialesLocal sessionCache,
            IFabricaFormularios fabricaFormularios,
            IClienteSignalRNotificaciones notificationClient)
        {
            InitializeComponent();
            _coordinador = new CoordinadorDashboardAdmin(
                this,
                sidebarAdmin,
                topBarAdmin,
                vistaTickets,
                tabMain,
                btnNuevoTicket,
                btnAbrirReportes,
                ticketService,
                usuarioService,
                catalogoService,
                contextoSesion,
                sessionCache,
                fabricaFormularios,
                notificationClient
            );
        }

        private async void DashboardAdmin_Load(object sender, EventArgs e)
        {
            await _coordinador.IniciarAsync();
        }
    }
}
