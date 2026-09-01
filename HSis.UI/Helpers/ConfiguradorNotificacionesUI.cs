#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.UI.Controls;
using HSis.UI.Factories;
using HSis.UI.Presenters;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public static class ConfiguradorNotificacionesUI
    {
        public static NotificacionesControl IntegrarNotificaciones(
            this Form formulario,
            NotificacionesPresenter presenter,
            IFabricaFormularios fabricaFormularios,
            IContextoSesion contextoSesion,
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

            notifControl.Configurar(presenter, fabricaFormularios, contextoSesion, callbackRecargaDatos);
            formulario.Controls.Add(notifControl);
            notifControl.BringToFront();

            ConfigurarOcultarAlHacerClicFuera(formulario, notifControl);

            formulario.FormClosed += (s, e) => presenter.DesconectarEvents();

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


