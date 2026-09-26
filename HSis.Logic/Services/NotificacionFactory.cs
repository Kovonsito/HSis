using System.Globalization;
using HSis.Contracts.Constants;
using HSis.Data.Models;

namespace HSis.Logic.Services;

public static class NotificacionFactory
{
    public static Notificacion Crear(
        int usuarioDestinoId,
        int? ticketId,
        string tipo,
        string mensaje,
        DateTime? fechaCreacion = null,
        int? materialId = null)
        => new()
        {
            UsuarioDestinoId = usuarioDestinoId,
            TicketId = ticketId,
            MaterialId = materialId,
            Tipo = tipo,
            Mensaje = mensaje,
            FechaCreacion = fechaCreacion ?? DateTime.Now,
            Leido = false
        };

    public static string CrearMensajeNuevoTicket(int ticketId, string? ticketFolio, string titulo)
        => $"Se ha registrado un nuevo ticket {ObtenerFolioNumerico(ticketFolio, ticketId)}: \"{titulo}\".";

    public static string CrearMensajeCambioEstado(int ticketId, string? ticketFolio, string nuevoEstado)
        => $"El ticket {ObtenerFolioNumerico(ticketFolio, ticketId)} ha cambiado al estatus: {nuevoEstado}.";

    public static string CrearMensajeAsignacion(
        int ticketId,
        string? ticketFolio,
        string? tecnicoNombre)
        => $"El ticket {ObtenerFolioNumerico(ticketFolio, ticketId)} fue asignado a {ObtenerNombre(tecnicoNombre, "un técnico")}.";

    public static string CrearMensajeReasignacion(
        int ticketId,
        string? ticketFolio,
        string? tecnicoAnterior,
        string? tecnicoNuevo)
        => $"El ticket {ObtenerFolioNumerico(ticketFolio, ticketId)} fue reasignado de {ObtenerNombre(tecnicoAnterior, "un técnico")} a {ObtenerNombre(tecnicoNuevo, "sin técnico")}.";

    public static string CrearMensajeCambioPrioridad(
        int ticketId,
        string? ticketFolio,
        string? prioridadAnterior,
        string? prioridadNueva)
        => $"La prioridad del ticket {ObtenerFolioNumerico(ticketFolio, ticketId)} cambió de {ObtenerNombre(prioridadAnterior, "sin prioridad")} a {ObtenerNombre(prioridadNueva, "sin prioridad")}.";

    public static string CrearMensajeSolucion(
        int ticketId,
        string? ticketFolio,
        string? solucion)
        => $"Se actualizó la solución del ticket {ObtenerFolioNumerico(ticketFolio, ticketId)}: {ObtenerNombre(solucion, "sin descripción")}.";

    public static string CrearMensajeMovimientoMaterial(
        int materialId,
        string? materialNombre,
        string? tipoMovimiento,
        int cantidad,
        int? existenciaActual,
        string? motivo)
    {
        var existencia = existenciaActual.HasValue
            ? $" Inventario actual: {existenciaActual.Value}."
            : string.Empty;
        var detalleMotivo = string.IsNullOrWhiteSpace(motivo)
            ? string.Empty
            : $" Motivo: {motivo.Trim()}.";

        return $"Movimiento de material {ObtenerNombre(materialNombre, $"#{materialId}")}: {ObtenerNombre(tipoMovimiento, "actualización")} de {Math.Abs(cantidad)} unidad(es).{existencia}{detalleMotivo}";
    }

    public static string CrearMensajeMaterialTicket(
        int ticketId,
        string? ticketFolio,
        string? materialNombre,
        int cantidad,
        bool agregado)
        => $"Se {(agregado ? "agregó" : "retiró")} {Math.Abs(cantidad)} unidad(es) de {ObtenerNombre(materialNombre, "un material")} al ticket {ObtenerFolioNumerico(ticketFolio, ticketId)}.";

    public static string CrearMensajeActualizacionMaterialTicket(
        int ticketId,
        string? ticketFolio,
        string? materialNombre,
        int cantidadAnterior,
        int cantidadNueva)
        => $"Se actualizó la cantidad de {ObtenerNombre(materialNombre, "un material")} en el ticket {ObtenerFolioNumerico(ticketFolio, ticketId)}: de {cantidadAnterior} a {cantidadNueva} unidad(es).";

    public static string CrearMensajeCalificacion(
        int ticketId,
        string? ticketFolio,
        int calificacion,
        string? comentario)
    {
        var estrellas = new string('⭐', calificacion);
        return $"El cliente calificó el ticket {ObtenerFolioNumerico(ticketFolio, ticketId)} con {estrellas} ({calificacion}/5). Comentario: \"{comentario ?? string.Empty}\"";
    }

    public static string ObtenerFolioNumerico(string? ticketFolio, int ticketId)
    {
        if (!string.IsNullOrWhiteSpace(ticketFolio))
        {
            var digitos = new string(ticketFolio.Where(char.IsDigit).ToArray()).TrimStart('0');
            if (!string.IsNullOrEmpty(digitos))
            {
                return digitos;
            }
        }

        return ticketId.ToString(CultureInfo.InvariantCulture);
    }

    private static string ObtenerNombre(string? valor, string valorPredeterminado)
        => string.IsNullOrWhiteSpace(valor) ? valorPredeterminado : valor.Trim();
}
