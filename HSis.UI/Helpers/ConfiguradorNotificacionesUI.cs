#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
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
            Services.Coordinators.IUiSessionCoordinator sesionCoordinator,
            Func<Task>? callbackRecargaDatos = null)
        {
            return formulario.IntegrarNotificacionesModerno(
                topBar,
                sesionCoordinator.FabricaFormularios,
                sesionCoordinator.ContextoSesion,
                sesionCoordinator.NotificationClient,
                sesionCoordinator.NotificacionStorage,
                sesionCoordinator.NotificationEventBus,
                callbackRecargaDatos
            );
        }

        public static NotificacionesControl IntegrarNotificacionesModerno(
            this Form formulario,
            TopBarControl topBar,
            IFabricaFormularios fabricaFormularios,
            IContextoSesion contextoSesion,
            INotificationClientService clienteNotificaciones,
            INotificacionStorageService servicioAlmacenamiento,
            INotificationEventBus? eventBus = null,
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
            topBar.NotificacionesClic += (s, e) => Reposicionar();

            notifControl.VincularTopBar(topBar);
            notifControl.Configurar(fabricaFormularios, contextoSesion, clienteNotificaciones, servicioAlmacenamiento, eventBus, callbackRecargaDatos);
            formulario.Controls.Add(notifControl);
            notifControl.BringToFront();

            ConfigurarOcultarAlHacerClicFuera(formulario, notifControl);
            formulario.FormClosed += (s, e) => notifControl.DesconectarEvents();

            return notifControl;
        }

        public static NotificacionesControl IntegrarNotificaciones(
            this Form formulario,
            IFabricaFormularios fabricaFormularios,
            IContextoSesion contextoSesion,
            INotificationClientService clienteNotificaciones,
            INotificacionStorageService servicioAlmacenamiento,
            INotificationEventBus? eventBus = null,
            Func<Task>? callbackRecargaDatos = null)
        {
            var notifControl = new NotificacionesControl
            {
                Visible = false,
                Width = 330,
                Height = Math.Max(380, formulario.ClientSize.Height - 50),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right
            };

            var menu = formulario.MainMenuStrip;
            if (menu == null)
            {
                menu = formulario.Controls.OfType<MenuStrip>().FirstOrDefault();
                if (menu != null) formulario.MainMenuStrip = menu;
            }

            if (menu != null)
            {
                var itemCampana = new ToolStripMenuItem("🔔 (0)")
                {
                    Alignment = ToolStripItemAlignment.Right
                };
                menu.Items.Add(itemCampana);
                notifControl.VincularItemMenu(itemCampana);

                int topOffset = menu.Height > 0 ? menu.Height : 28;
                notifControl.Location = new Point(formulario.ClientSize.Width - 340, topOffset);
            }
            else
            {
                notifControl.Location = new Point(formulario.ClientSize.Width - 340, 30);
            }

            notifControl.Configurar(fabricaFormularios, contextoSesion, clienteNotificaciones, servicioAlmacenamiento, eventBus, callbackRecargaDatos);
            formulario.Controls.Add(notifControl);
            notifControl.BringToFront();

            ConfigurarOcultarAlHacerClicFuera(formulario, notifControl);

            formulario.FormClosed += (s, e) => notifControl.DesconectarEvents();

            return notifControl;
        }

        private static void ConfigurarOcultarAlHacerClicFuera(Form formulario, NotificacionesControl notifControl)
        {
            void SuscribirRecursivo(Control container)
            {
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
                    if (ctrl == notifControl) continue;
                    SuscribirRecursivo(ctrl);
                }
            }

            SuscribirRecursivo(formulario);
        }
    }
}
