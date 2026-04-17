using System;
using System.Collections.Generic;
using System.Text;

namespace HSis.Logic.DTOs
{
    public class TicketGridDto
    {
        public int Folio { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Status { get; set; }
        public DateTime? Alta { get; set; }
        public DateTime? Atención { get; set; }
        public DateTime? Cierre { get; set; }
        public string? AtendidoPor { get; set; }
        public string? Descripción { get; set; }
        public string? Solución { get; set; }
    }
}
