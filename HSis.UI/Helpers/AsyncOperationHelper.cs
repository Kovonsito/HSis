#nullable enable
using System.Runtime.Versioning;
using System.Runtime.CompilerServices;

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Ayudante para ejecutar operaciones asíncronas de manera segura en formularios Windows Forms,
    /// gestionando automáticamente el estado de carga por formulario, el cursor de espera,
    /// la deshabilitación de controles y el reporte uniforme de errores.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class AsyncOperationHelper
    {
        private static readonly ConditionalWeakTable<Form, EstadoCargaAsync> EstadosPorFormulario = new();

        public static EstadoCargaAsync ObtenerEstadoCarga(this Form form)
            => EstadosPorFormulario.GetValue(form, _ => new EstadoCargaAsync());

        public static async Task EjecutarOperacionAsync(
            this Form form,
            Func<Task> accionAsync,
            string? mensajeErrorContexto = null,
            params Control[]? controlesADeshabilitar)
            => await EjecutarOperacionAsync(
                form,
                accionAsync,
                mensajeErrorContexto,
                "general",
                controlesADeshabilitar);

        public static async Task<bool> EjecutarOperacionAsync(
            this Form form,
            Func<Task> accionAsync,
            string? mensajeErrorContexto,
            string claveOperacion,
            params Control[]? controlesADeshabilitar)
        {
            if (form.IsDisposed)
            {
                return false;
            }

            var estadoCarga = form.ObtenerEstadoCarga();
            if (!estadoCarga.IntentarIniciar(claveOperacion))
            {
                return false;
            }

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
            catch (Exception ex)
            {
                ManejadorErroresUI.Manejar(ex, mensajeErrorContexto, form);
            }
            finally
            {
                estadoCarga.Finalizar(claveOperacion);
                AlternarEstado(estadoCarga.EstaCargando);
            }

            return true;
        }
    }
}
