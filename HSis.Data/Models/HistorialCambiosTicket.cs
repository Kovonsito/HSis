using System;
using System.Collections.Generic;

namespace HSis.Data.Models;

public partial class HistorialCambiosTicket
{
    public int IdHistorial { get; set; }

    public int IdTicket { get; set; }

    public int IdUsuarioCambio { get; set; }

    public DateTime FechaMovimiento { get; set; }

    public string CampoModificado { get; set; } = null!;

    public string? ValorAnterior { get; set; }

    public string? ValorNuevo { get; set; }

    public virtual Ticket IdTicketNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioCambioNavigation { get; set; } = null!;
}
