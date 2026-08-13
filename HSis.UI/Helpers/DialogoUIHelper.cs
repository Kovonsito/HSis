#nullable enable
using System;
using System.Runtime.Versioning;
using System.Windows.Forms;

namespace HSis.UI.Helpers
{
    [SupportedOSPlatform("windows")]
    public static class DialogoUIHelper
    {
        public static void MostrarError(string mensaje, string titulo = "Error")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void MostrarExito(string mensaje, string titulo = "Éxito")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void MostrarAdvertencia(string mensaje, string titulo = "Validación")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }

        public static void MostrarInformacion(string mensaje, string titulo = "Información")
        {
            MessageBox.Show(mensaje, titulo, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static bool Confirmar(string mensaje, string titulo = "Confirmar")
        {
            return MessageBox.Show(mensaje, titulo, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
        }

        public static void MostrarExcepcion(Exception ex, string mensajeContexto = "Ocurrió un error inesperado")
        {
            var realEx = ex.InnerException ?? ex;
            MessageBox.Show($"{mensajeContexto}: {realEx.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
