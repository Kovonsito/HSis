namespace HSis.Logic.DTOs

{
    public class TicketDto
    {
        public int IdTicket { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string DepartamentoUsuario { get; set; } = null!;
        public DateTime? Alta { get; set; }
        public DateTime? Cierre { get; set; }
        public DateTime? Atencion { get; set; }
        public string? Status { get; set; }
        public string? Descripcion { get; set; }
        public string? Solucion { get; set; }
        public int? IdTecnico { get; set; }
        public string NombreTecnico { get; set; } = null!;
        public string? Prioridad { get; set; }
        public int? Calificacion { get; set; }
        public string? ComentarioFeedback { get; set; }
        public DateTime? FechaFeedback { get; set; }
    }

    public class TicketCreateDto
    {
        public int IdUsuario { get; set; }
        public string? Descripcion { get; set; }
        public int? IdTecnico { get; set; }
        public string? Prioridad { get; set; }
    }

    public class TicketUpdateDto
    {
        public int IdTicket { get; set; }
        public string? Status { get; set; }
        public string? Solucion { get; set; }
        public int? IdTecnico { get; set; }
        public DateTime? Atencion { get; set; }
        public DateTime? Cierre { get; set; }
        public string? Prioridad { get; set; }
    }
}
