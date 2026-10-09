#nullable enable
using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public static class ConfiguradorNotificacionesUI
    {
        public static NotificacionesControl IntegrarNotificacionesModerno(
            this Form formulario,
            TopBarControl topBar,
            IFabricaFormularios fabricaFormularios,
            IAdministradorSesionUsuario contextoSesion,
            IClienteSignalRNotificaciones clienteNotificaciones,
            INotificacionesApiClient notificacionesApiClient,
            IBusEventosNotificaciones? eventBus = null,
            Func<Task>? callbackRecargaDatos = null)
        {
            int ancho = 360;
            int alto = Math.Min(480, Math.Max(320, formulario.ClientSize.Height - topBar.Height - 40));

            var notifControl = new NotificacionesControl
            {
                Visible = false,
                Width = ancho,
                Height = alto,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Location = new Point(Math.Max(10, formulario.ClientSize.Width - ancho - 20), topBar.Height + 6)
            };

            void Reposicionar()
            {
                notifControl.Location = new Point(Math.Max(10, formulario.ClientSize.Width - notifControl.Width - 20), topBar.Height + 6);
                notifControl.BringToFront();
            }

            formulario.Resize += (s, e) => Reposicionar();
            EventHandler reposicionarHandler = (_, _) => Reposicionar();
            topBar.NotificacionesClic += reposicionarHandler;

            EventHandler<EstadoConexionEventArgs>? estadoConexionHandler = null;
            if (eventBus != null)
            {
                estadoConexionHandler = (_, args) =>
                    topBar.ActualizarConexion(args.Conectado, args.MensajeEstado);
                eventBus.OnEstadoConexionCambiado += estadoConexionHandler;
            }

            notifControl.VincularTopBar(topBar);
            notifControl.Configurar(
                fabricaFormularios,
                contextoSesion,
                clienteNotificaciones,
                notificacionesApiClient,
                eventBus,
                callbackRecargaDatos);
            formulario.Controls.Add(notifControl);
            notifControl.BringToFront();

            var filtroClicFuera = ConfigurarOcultarAlHacerClicFuera(formulario, notifControl, topBar);
            formulario.FormClosed += (_, _) =>
            {
                Application.RemoveMessageFilter(filtroClicFuera);
                topBar.NotificacionesClic -= reposicionarHandler;
                if (estadoConexionHandler != null && eventBus != null)
                {
                    eventBus.OnEstadoConexionCambiado -= estadoConexionHandler;
                }
                notifControl.DesconectarEvents();
            };

            return notifControl;
        }

        private static IMessageFilter ConfigurarOcultarAlHacerClicFuera(
            Form formulario,
            NotificacionesControl notifControl,
            TopBarControl topBar)
        {
            var filtro = new FiltroClicFuera(formulario, notifControl, topBar);
            Application.AddMessageFilter(filtro);
            return filtro;
        }

        // Escucha el clic a nivel de aplicacion: suscribir Click por control no cubre la barra superior ni los hijos que consumen el evento.
        private sealed class FiltroClicFuera(Form formulario, NotificacionesControl notifControl, TopBarControl topBar) : IMessageFilter
        {
            private const int WM_LBUTTONDOWN = 0x0201;
            private const int WM_RBUTTONDOWN = 0x0204;

            public bool PreFilterMessage(ref Message m)
            {
                if ((m.Msg != WM_LBUTTONDOWN && m.Msg != WM_RBUTTONDOWN) || !notifControl.Visible)
                {
                    return false;
                }

                var origen = Control.FromHandle(m.HWnd);
                if (origen is null || origen.FindForm() != formulario)
                {
                    return false;
                }

                var puntoPantalla = Cursor.Position;
                var rectPanel = notifControl.RectangleToScreen(notifControl.ClientRectangle);
                if (rectPanel.Contains(puntoPantalla) || topBar.EsPuntoDeCampana(puntoPantalla))
                {
                    return false;
                }

                notifControl.Visible = false;
                return false;
            }
        }    }
}
