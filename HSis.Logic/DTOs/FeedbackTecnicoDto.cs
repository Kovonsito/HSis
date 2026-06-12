namespace HSis.Logic.DTOs

{
    public class FeedbackTecnicoDto
    {
        public int IdTicket { get; set; }
        public string Folio => $"TK-{IdTicket:D6}";
        public string Calificacion { get; set; } = null!;
        public string? Comentario { get; set; }
        public DateTime? Fecha { get; set; }
    }
}
