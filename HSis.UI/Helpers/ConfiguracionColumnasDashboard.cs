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
        // ── Columnas que SIEMPRE se ocultan en vistas de tickets ──────────────
        private static readonly string[] _columnasOcultasBase = [
            "IdTicket", "IdUsuario", "NombreUsuario", "DepartamentoUsuario",
            "FechaAtencion", "FechaCierre", "Estatus", "IdTecnico", "NombreTecnico",
            "TecnicoAsignado", "Calificacion", "ComentarioEvaluacion", "FechaEvaluacion",
            "Evaluacion", "Feedback", "FolioFormato"
        ];

        /// <summary>
        /// Vista de tickets del panel de cliente: muestra folio, fecha, estatus,
        /// técnico asignado, descripción y feedback.
        /// </summary>
        public static void AplicarPerfilCliente(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(
                "IdTicket", "IdUsuario", "NombreUsuario", "Usuario", "DepartamentoUsuario",
                "FechaAtencion", "FechaCierre", "Estatus", "Solucion", "IdTecnico",
                "NombreTecnico", "Prioridad", "Calificacion", "ComentarioEvaluacion",
                "FechaEvaluacion", "Evaluacion", "FolioFormato");

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 70, null),
                ("FechaAlta", "Fecha de Solicitud", 130, "dd/MM/yyyy HH:mm"),
                ("Status", "Estatus", 90, null),
                ("TecnicoAsignado", "Técnico Asignado", 140, null),
                ("Descripcion", "Descripción del Problema", 260, null),
                ("Feedback", "Calificación / Feedback", 120, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }

        /// <summary>
        /// Vista de tickets del panel técnico: folio, usuario, estatus, prioridad,
        /// fecha, descripción y solución.
        /// </summary>
        public static void AplicarPerfilTecnico(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(_columnasOcultasBase);

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 80, null),
                ("Usuario", "Usuario Solicitante", 160, null),
                ("Status", "Estatus", 100, null),
                ("Prioridad", "Prioridad", 100, null),
                ("FechaAlta", "Fecha Alta", 130, null),
                ("Descripcion", "Descripción del Problema", 260, null),
                ("Solucion", "Solución Aplicada", 260, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }

        /// <summary>
        /// Vista de calificaciones del panel técnico: usuario calificador, comentario,
        /// fecha y puntuación con emoji de estrella.
        /// </summary>
        public static void AplicarPerfilCalificaciones(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(_columnasOcultasBase);

            dgv.ConfigurarColumnas(
                ("NombreUsuario", "Usuario Calificador", 180, null),
                ("Comentario", "Comentario de Retroalimentación", 320, null),
                ("FechaRegistro", "Fecha Calificación", 140, null),
                ("Puntuacion", "Calificación ⭐", 130, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }

        /// <summary>
        /// Vista de tickets del panel administrador: columnas completas incluyendo
        /// técnico asignado, fechas de atención y cierre.
        /// </summary>
        public static void AplicarPerfilAdmin(DataGridView dgv)
        {
            if (dgv.Columns.Count == 0) return;

            dgv.ConfigurarOcultarColumnas(
                "IdTicket", "IdUsuario", "DepartamentoUsuario", "Calificacion",
                "ComentarioEvaluacion", "FechaEvaluacion", "Evaluacion", "Feedback", "FolioFormato");

            dgv.ConfigurarColumnas(
                ("Folio", "Folio", 50, null),
                ("NombreUsuario", "Usuario", 110, null),
                ("Estatus", "Estatus", 70, null),
                ("Prioridad", "Prioridad", 70, null),
                ("FechaAlta", "Fecha Alta", 85, "dd/MM/yyyy HH:mm"),
                ("FechaAtencion", "Fecha Atención", 85, "dd/MM/yyyy HH:mm"),
                ("FechaCierre", "Fecha Cierre", 85, "dd/MM/yyyy HH:mm"),
                ("TecnicoAsignado", "Técnico Asignado", 100, null),
                ("Descripcion", "Descripción", 150, null),
                ("Solucion", "Solución", 150, null)
            );

            dgv.AutoajustarAnchosMinimos();
        }
    }
}
