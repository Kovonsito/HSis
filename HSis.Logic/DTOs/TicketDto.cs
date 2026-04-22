using System;

namespace HSis.Logic.DTOs
{
    public class TicketDto
    {
        public int IdTicket { get; set; }
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; } = string.Empty;
        public DateTime? Alta { get; set; }
        public DateTime? Cierre { get; set; }
        public DateTime? Atencion { get; set; }
        public string? Status { get; set; }
        public string? Descripcion { get; set; }
        public string? Solucion { get; set; }
        public int? IdTecnico { get; set; }
        public string NombreTecnico { get; set; } = string.Empty;
    }

    public class TicketCreateDto
    {
        public int IdUsuario { get; set; }
        public string? Descripcion { get; set; }
    }

    public class TicketUpdateDto
    {
        public int IdTicket { get; set; }
        public string? Status { get; set; }
        public string? Solucion { get; set; }
        public int? IdTecnico { get; set; }
        public DateTime? Atencion { get; set; }
        public DateTime? Cierre { get; set; }
    }
}
