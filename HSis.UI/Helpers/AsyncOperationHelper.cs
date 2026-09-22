#nullable enable
using System.Net.Http;
using System.Runtime.Versioning;
using Serilog;

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Ayudante para ejecutar operaciones asíncronas de manera segura en formularios Windows Forms,
    /// gestionando automáticamente el cursor de espera, deshabilitación de controles y reporte uniforme de errores.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class AsyncOperationHelper
    {
        public static async Task EjecutarOperacionAsync(
            this Form form,
            Func<Task> accionAsync,
            string? mensajeErrorContexto = null,
            params Control[]? controlesADeshabilitar)
        {
            if (form.IsDisposed) return;

            void AlternarEstado(bool activo)
            {
                if (form.IsDisposed) return;

                if (form.InvokeRequired)
                {
                    form.Invoke(new Action(() => AlternarEstado(activo)));
                    return;
                }

                form.Cursor = activo ? Cursors.WaitCursor : Cursors.Default;

                if (controlesADeshabilitar != null)
                {
                    foreach (var c in controlesADeshabilitar)
                    {
                        if (c != null && !c.IsDisposed)
                        {
                            c.Enabled = !activo;
                        }
                    }
                }
            }

            try
            {
                AlternarEstado(true);
                await accionAsync();
            }
            catch (HttpRequestException httpEx)
            {
                Log.Warning(httpEx, "Fallo de conexión HTTP durante la operación en {FormName}: {Mensaje}", form.Name, httpEx.Message);
                DialogoUIHelper.MostrarAdvertencia(
                    "No se pudo completar la solicitud debido a un problema de comunicación con el servidor.\nVerifique su conexión.",
                    "Problema de Conexión"
                );
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error inesperado durante la operación en {FormName}: {Mensaje}", form.Name, ex.Message);
                DialogoUIHelper.MostrarExcepcion(ex, mensajeErrorContexto ?? "Ocurrió un error al procesar los datos");
            }
            finally
            {
                AlternarEstado(false);
            }
        }
    }
}
