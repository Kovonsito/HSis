namespace HSis.Logic.DTOs
{
    public class TicketClienteDto
    {
        public int IdTicket { get; set; }
        public string Folio => $"TK-{IdTicket:D6}";
        public DateTime? FechaAlta { get; set; }
        public string Status { get; set; }
        public string TecnicoAsignado { get; set; }
        public string Descripcion { get; set; }
    }
}
