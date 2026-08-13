namespace HSis.Logic.DTOs
{
    public class TicketOperativoDto
    {
        public int IdTicket { get; set; }
        public string Folio => $"TK-{IdTicket:D6}";
        public DateTime? FechaAlta { get; set; }
        public string? Estatus { get; set; }
        public string? Usuario { get; set; }
        public string? Descripcion { get; set; }
        public string? Prioridad { get; set; }

        // Propiedad de compatibilidad
        public string? Status { get => Estatus; set => Estatus = value; }
    }
}

