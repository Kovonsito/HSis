#nullable enable
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    /// <summary>
    /// Tipos estándar de KPIs utilizados en los diferentes dashboards del sistema.
    /// </summary>
    public enum TipoKpiDashboard
    {
        // Admin / Globales
        Disponibles,
        Urgentes,
        EnProceso,
        Cerrados,
        Reabiertos,

        // Técnico
        MisAsignados,
        MisCerrados,

        // Cliente
        MisActivos,

        // Común (⭐)
        Calificacion
    }

    /// <summary>
    /// Definición de metadatos predeterminados para un KPI (Título, Color de Acento, Imagen de recurso opcional).
    /// </summary>
    public static class CatalogoKpis
    {
        public static (string Titulo, Color ColorAcento, Image? Imagen) ObtenerMetadatos(TipoKpiDashboard tipo)
        {
            return tipo switch
            {
                TipoKpiDashboard.Disponibles => ("DISPONIBLES", TemaVisual.TicketDisponible, Properties.Resources.Disponible),
                TipoKpiDashboard.Urgentes => ("URGENTES", TemaVisual.TicketUrgente, Properties.Resources.Urgente),
                TipoKpiDashboard.EnProceso => ("EN PROCESO", TemaVisual.TicketEnProceso, Properties.Resources.En_proceso),
                TipoKpiDashboard.Cerrados => ("CERRADOS", TemaVisual.TicketCerrado, Properties.Resources.Cerrado),
                TipoKpiDashboard.Reabiertos => ("REABIERTOS", TemaVisual.TicketReabierto, null),

                TipoKpiDashboard.MisAsignados => ("MIS ASIGNADOS", TemaVisual.TicketDisponible, null),
                TipoKpiDashboard.MisCerrados => ("MIS CERRADOS", TemaVisual.TicketCerrado, Properties.Resources.Cerrado),

                TipoKpiDashboard.MisActivos => ("MIS TICKETS ACTIVOS", TemaVisual.TicketDisponible, Properties.Resources.Disponible),

                TipoKpiDashboard.Calificacion => ("MI CALIFICACIÓN", TemaVisual.TicketReabierto, null),
                _ => (tipo.ToString().ToUpperInvariant(), TemaVisual.Primario, null)
            };
        }
    }
}
