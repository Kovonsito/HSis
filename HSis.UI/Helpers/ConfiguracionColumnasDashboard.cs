#nullable enable
using System.Runtime.Versioning;

namespace HSis.UI.Helpers
{
    /// <summary>
    /// Perfiles de columnas del DataGridView para cada rol de dashboard.
    /// Centraliza las llamadas a ConfigurarOcultarColumnas + ConfigurarColumnas + AutoajustarAnchosMinimos.
    /// </summary>
    [SupportedOSPlatform("windows")]
    public static class ConfiguracionColumnasDashboard
    {
        private static readonly string[] _columnasOcultasBase = [
            "IdTicket", "IdUsuario", "DepartamentoUsuario", "FechaAtencion", "FechaCierre",
            "IdTecnico", "Calificacion", "ComentarioEvaluacion", "FechaEvaluacion", "Evaluacion",
            "Feedback", "Usuario", "Status", "NombreTecnico", "TecnicoAsignado"
        ];

        /// <summary>
        /// Vista de tickets del panel de cliente.
        /// </summary>
        public static void AplicarPerfilCliente(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(
                "IdTicket", "IdUsuario", "NombreUsuario", "Usuario", "DepartamentoUsuario",
                "FechaAtencion", "FechaCierre", "Solucion", "IdTecnico",
                "TecnicoAsignado", "Calificacion", "ComentarioEvaluacion", "FechaEvaluacion",
                "Evaluacion", "Status");

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 80, null),
                ("Descripcion", "Descripción", 320, null),
                ("FechaAlta", "Fecha de solicitud", 140, "dd/MM/yyyy HH:mm"),
                ("Estatus", "Estatus", 105, null),
                ("Prioridad", "Prioridad", 100, null),
                ("NombreTecnico", "Técnico asignado", 170, null),
                ("Feedback", "Calificación / feedback", 170, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }

        /// <summary>
        /// Vista de tickets del panel técnico.
        /// </summary>
        public static void AplicarPerfilTecnico(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(_columnasOcultasBase);

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 80, null),
                ("Descripcion", "Descripción", 320, null),
                ("FechaAlta", "Fecha de alta", 140, "dd/MM/yyyy HH:mm"),
                ("Estatus", "Estatus", 105, null),
                ("Prioridad", "Prioridad", 100, null),
                ("NombreUsuario", "Usuario solicitante", 180, null),
                ("Solucion", "Solución aplicada", 300, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }

        /// <summary>
        /// Vista de calificaciones basada en TicketDto, usada por el panel administrador.
        /// </summary>
        public static void AplicarPerfilCalificaciones(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(
                "IdTicket", "IdUsuario", "DepartamentoUsuario", "FechaAlta", "FechaAtencion", "FechaCierre",
                "Estatus", "IdTecnico", "NombreTecnico", "TecnicoAsignado", "Prioridad", "Descripcion",
                "Solucion", "Evaluacion", "Feedback", "Usuario", "Status");

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 80, null),
                ("NombreUsuario", "Usuario Calificador", 180, null),
                ("ComentarioEvaluacion", "Comentario de retroalimentación", 320, null),
                ("FechaEvaluacion", "Fecha de calificación", 140, "dd/MM/yyyy HH:mm"),
                ("Calificacion", "Calificación ⭐", 130, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }

        /// <summary>
        /// Vista de calificaciones basada en FeedbackTecnicoDto, usada por el panel técnico.
        /// </summary>
        public static void AplicarPerfilFeedbackTecnico(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas("IdTicket", "Puntuacion", "FechaRegistro");

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 80, null),
                ("Comentario", "Comentario de retroalimentación", 320, null),
                ("Fecha", "Fecha de calificación", 140, "dd/MM/yyyy HH:mm"),
                ("NombreUsuario", "Usuario calificador", 180, null),
                ("Calificacion", "Calificación ⭐", 130, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }

        /// <summary>
        /// Vista de tickets del panel administrador.
        /// </summary>
        public static void AplicarPerfilAdmin(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(
                "IdTicket", "IdUsuario", "DepartamentoUsuario", "IdTecnico", "Calificacion",
                "ComentarioEvaluacion", "FechaEvaluacion", "Evaluacion", "Feedback", "Usuario",
                "TecnicoAsignado", "Status");

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 80, null),
                ("Descripcion", "Descripción", 320, null),
                ("FechaAlta", "Fecha de alta", 140, "dd/MM/yyyy HH:mm"),
                ("Estatus", "Estatus", 105, null),
                ("Prioridad", "Prioridad", 100, null),
                ("NombreUsuario", "Usuario", 170, null),
                ("NombreTecnico", "Técnico asignado", 170, null),
                ("Solucion", "Solución", 300, null),
                ("FechaAtencion", "Fecha de atención", 140, "dd/MM/yyyy HH:mm"),
                ("FechaCierre", "Fecha de cierre", 140, "dd/MM/yyyy HH:mm")
            );

            dgv.AutoajustarAnchosMinimos();
        }
    }
}
