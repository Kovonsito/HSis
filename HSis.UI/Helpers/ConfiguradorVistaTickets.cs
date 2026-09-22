#nullable enable
using System.Runtime.Versioning;
using HSis.UI.Controls;

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Opciones de comportamiento para configurar <see cref="VistaTicketsDashboardControl"/>.
    /// Cada propiedad corresponde a un paso que antes se repetía manualmente en el Load de cada dashboard.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public sealed class ConfiguracionVistaDashboard
    {
        /// <summary>Controles KPI y botones de acción que se muestran en la franja superior.</summary>
        public Control[] Indicadores { get; init; } = [];

        /// <summary>Campos de filtro que se pasan a <see cref="FiltroGenericoControl"/>.</summary>
        public List<FiltroCampo> Filtros { get; init; } = [];

        /// <summary>Función (sync o async) que repinta la página actual del grid.</summary>
        public Func<Task>? AlMostrarPagina { get; init; }

        /// <summary>Función asíncrona que recarga los datos desde el servidor (botón Recargar).</summary>
        public Func<Task>? AlRecargar { get; init; }

        /// <summary>
        /// Función (sync o async) que se ejecuta al limpiar filtros.
        /// Si es null, el control simplemente reinicia la paginación y llama a <see cref="AlMostrarPagina"/>.
        /// </summary>
        public Func<Task>? AlLimpiar { get; init; }

        /// <summary>
        /// Handler de doble clic en una fila del grid.
        /// Recibe el índice de la fila seleccionada.
        /// </summary>
        public Func<int, Task>? AlDobleClicFila { get; init; }

        /// <summary>
        /// Opciones para configurar los combos del filtro genérico antes del primer load.
        /// Clave = nombre del campo, Valor = lista de opciones.
        /// </summary>
        public Dictionary<string, object>? OpcionesCombo { get; init; }
    }

    /// <summary>
    /// Extension methods para <see cref="VistaTicketsDashboardControl"/> que encapsulan
    /// el bloque repetido de configuración presente en el Load de cada dashboard.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class ConfiguradorVistaTickets
    {
        /// <summary>
        /// Configura de una sola llamada los indicadores KPI, los filtros, la paginación
        /// y todos los eventos de interacción del control.
        /// </summary>
        public static void ConfigurarComportamiento(
            this VistaTicketsDashboardControl vista,
            ConfiguracionVistaDashboard opciones)
        {
            // 1. Indicadores KPI y botones de acción en la franja superior
            if (opciones.Indicadores.Length > 0)
                vista.ConfigurarIndicadores(opciones.Indicadores);

            // 2. Campos del filtro genérico
            if (opciones.Filtros.Count > 0)
                vista.InicializarFiltros(opciones.Filtros);

            // 3. Paginación
            if (opciones.AlMostrarPagina != null)
                vista.VincularPaginacion(() => _ = opciones.AlMostrarPagina());

            // 4. FiltroCambiado → reiniciar paginación y repintar
            vista.FiltroCambiado += async (_, _) =>
            {
                vista.ReiniciarAPrimeraPagina();
                if (opciones.AlMostrarPagina != null)
                    await opciones.AlMostrarPagina();
            };

            // 5. RecargarClic → traer datos frescos del servidor
            if (opciones.AlRecargar != null)
                vista.RecargarClic += (_, _) => _ = opciones.AlRecargar();

            // 6. LimpiarClic → limpiar filtros y repintar (o delegado personalizado)
            vista.LimpiarClic += async (_, _) =>
            {
                if (opciones.AlLimpiar != null)
                    await opciones.AlLimpiar();
                else
                {
                    vista.ReiniciarAPrimeraPagina();
                    if (opciones.AlMostrarPagina != null)
                        await opciones.AlMostrarPagina();
                }
            };

            // 7. Doble clic en fila del grid
            if (opciones.AlDobleClicFila != null)
            {
                vista.Grid.CellDoubleClick += async (_, e) =>
                {
                    if (e.RowIndex >= 0)
                        await opciones.AlDobleClicFila(e.RowIndex);
                };
            }
        }
    }
}
