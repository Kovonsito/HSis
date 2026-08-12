namespace HSis.Logic.DTOs
{
    public class TicketClienteDto
    {
        public int IdTicket { get; set; }
        public string Folio => $"TK-{IdTicket:D6}";
        public DateTime? FechaAlta { get; set; }
        public string? Estatus { get; set; }
        public string? TecnicoAsignado { get; set; }
        public string? Descripcion { get; set; }
        public string? Evaluacion { get; set; }

        // Propiedades de compatibilidad
        public string? Status { get => Estatus; set => Estatus = value; }
        public string? Feedback { get => Evaluacion; set => Evaluacion = value; }
    }
}
