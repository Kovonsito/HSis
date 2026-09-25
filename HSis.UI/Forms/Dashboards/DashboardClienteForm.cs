#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.UI.Coordinators;
using HSis.UI.Factories;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardClienteForm : Form
    {
        private readonly CoordinadorDashboardCliente _coordinador;

        public DashboardClienteForm(
            ITicketService ticketService,
            IAdministradorSesionUsuario contextoSesion,
            IAlmacenamientoCredencialesLocal sessionCache,
            IFabricaFormularios formFactory,
            IClienteSignalRNotificaciones notificationClient)
        {
            InitializeComponent();
            sidebarCliente.Colapsado = true;
            topBarCliente.MostrarHamburguesa = false;
            _coordinador = new CoordinadorDashboardCliente(
                this,
                sidebarCliente,
                topBarCliente,
                vistaTickets,
                btnNuevoReporte,
                ticketService,
                contextoSesion,
                sessionCache,
                formFactory,
                notificationClient
            );
        }

        private async void frmDashboardCliente_Load(object? sender, EventArgs e)
        {
            await _coordinador.IniciarAsync();
        }

        private void topBarCliente_Load(object sender, EventArgs e)
        {

        }
    }
}
