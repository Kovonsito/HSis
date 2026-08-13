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
            var notifControl = new NotificacionesControl();
            notifControl.Configurar(presenter, fabricaFormularios, contextoSesion, callbackRecargaDatos);
            formulario.Controls.Add(notifControl);
            notifControl.BringToFront();
            return notifControl;
        }
    }
}

