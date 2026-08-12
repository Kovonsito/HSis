namespace HSis.Logic.DTOs
{
    public class TicketDto
    {
        public int IdTicket { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = null!;
        public string DepartamentoUsuario { get; set; } = null!;
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaAtencion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? Estatus { get; set; }
        public string? Descripcion { get; set; }
        public string? Solucion { get; set; }
        public int? IdTecnico { get; set; }
        public string NombreTecnico { get; set; } = null!;
        public string? Prioridad { get; set; }
        public int? Calificacion { get; set; }
        public string? ComentarioEvaluacion { get; set; }
        public DateTime? FechaEvaluacion { get; set; }
    }
}
