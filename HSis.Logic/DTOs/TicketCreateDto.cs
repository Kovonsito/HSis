namespace HSis.Logic.DTOs
{
    public class TicketCreateDto
    {
        public int IdUsuario { get; set; }
        public string? Descripcion { get; set; }
        public int? IdTecnico { get; set; }
        public string? Prioridad { get; set; }
    }
}
