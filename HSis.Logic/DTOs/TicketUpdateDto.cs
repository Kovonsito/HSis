namespace HSis.Logic.DTOs
{
    public class TicketUpdateDto
    {
        public int IdTicket { get; set; }
        public string? Estatus { get; set; }
        public string? Solucion { get; set; }
        public int? IdTecnico { get; set; }
        public DateTime? FechaAtencion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? Prioridad { get; set; }
    }
}
