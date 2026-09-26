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

            ConfigurarOcultarAlHacerClicFuera(formulario, notifControl, topBar);
            formulario.FormClosed += (_, _) =>
            {
                topBar.NotificacionesClic -= reposicionarHandler;
                if (estadoConexionHandler != null && eventBus != null)
                {
                    eventBus.OnEstadoConexionCambiado -= estadoConexionHandler;
                }
                notifControl.DesconectarEvents();
            };

            return notifControl;
        }

        private static void ConfigurarOcultarAlHacerClicFuera(
            Form formulario,
            NotificacionesControl notifControl,
            TopBarControl topBar)
        {
            void SuscribirRecursivo(Control container)
            {
                if (container == notifControl || container == topBar)
                {
                    return;
                }

                container.Click += (s, e) =>
                {
                    if (notifControl.Visible)
                    {
                        Point mousePos = formulario.PointToClient(Cursor.Position);
                        if (!notifControl.Bounds.Contains(mousePos))
                        {
                            notifControl.Visible = false;
                        }
                    }
                };

                foreach (Control ctrl in container.Controls)
                {
                    SuscribirRecursivo(ctrl);
                }
            }

            SuscribirRecursivo(formulario);
        }
    }
}
