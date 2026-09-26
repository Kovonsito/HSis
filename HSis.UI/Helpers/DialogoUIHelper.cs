#nullable enable
using System.Runtime.Versioning;
using HSis.UI.Forms.Otros;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public static class DialogoUIHelper
    {
        public static void MostrarError(string mensaje, string titulo = "Error", IWin32Window? propietario = null)
        {
            DialogoModernoForm.Mostrar(ResolverPropietario(propietario), titulo, mensaje, DialogoTipo.Error);
        }

        public static void MostrarExito(string mensaje, string titulo = "Éxito", IWin32Window? propietario = null)
        {
            DialogoModernoForm.Mostrar(ResolverPropietario(propietario), titulo, mensaje, DialogoTipo.Exito);
        }

        public static void MostrarAdvertencia(string mensaje, string titulo = "Validación", IWin32Window? propietario = null)
        {
            DialogoModernoForm.Mostrar(ResolverPropietario(propietario), titulo, mensaje, DialogoTipo.Advertencia);
        }

        public static void MostrarInformacion(string mensaje, string titulo = "Información", IWin32Window? propietario = null)
        {
            DialogoModernoForm.Mostrar(ResolverPropietario(propietario), titulo, mensaje, DialogoTipo.Informacion);
        }

        public static bool Confirmar(string mensaje, string titulo = "Confirmar", IWin32Window? propietario = null)
        {
            return DialogoModernoForm.Mostrar(ResolverPropietario(propietario), titulo, mensaje, DialogoTipo.Confirmacion, true) == DialogResult.Yes;
        }

        public static void MostrarExcepcion(Exception ex, string mensajeContexto = "Ocurrió un error inesperado", IWin32Window? propietario = null)
            => ManejadorErroresUI.Manejar(ex, mensajeContexto, propietario);

        private static IWin32Window? ResolverPropietario(IWin32Window? propietario)
            => propietario ?? Form.ActiveForm;
    }
}

