using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.UI.Coordinators;
using HSis.UI.Factories;

namespace HSis.UI.Forms.Dashboards
{
    [SupportedOSPlatform("windows")]
    public partial class DashboardTecnicoForm : Form
    {
        private readonly CoordinadorDashboardTecnico _coordinador;

        public DashboardTecnicoForm(
            ITicketService ticketService,
            IAdministradorSesionUsuario contextoSesion,
            IAlmacenamientoCredencialesLocal sessionCache,
            IFabricaFormularios formFactory,
            IClienteSignalRNotificaciones notificationClient,
            INotificacionesApiClient notificacionesApiClient,
            IBusEventosNotificaciones eventBus)
        {
            InitializeComponent();
            sidebarTecnico.Colapsado = true;
            _coordinador = new CoordinadorDashboardTecnico(
                this,
                sidebarTecnico,
                topBarTecnico,
                vistaTickets,
                btnNuevoTicket,
                ticketService,
                contextoSesion,
                sessionCache,
                formFactory,
                notificationClient,
                notificacionesApiClient,
                eventBus
            );
        }

        private async void frmDashboardTecnico_Load(object? sender, EventArgs e)
        {
            await _coordinador.IniciarAsync();
        }

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x02000000; // WS_EX_COMPOSITED: composición nativa sin parpadeos
                return cp;
            }
        }
    }
}
