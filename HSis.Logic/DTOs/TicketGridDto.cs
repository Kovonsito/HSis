namespace HSis.Logic.DTOs
{
    public class TicketGridDto
    {
        public int Folio { get; set; }
        public string? NombreUsuario { get; set; }
        public string? Estatus { get; set; }
        public DateTime? FechaAlta { get; set; }
        public DateTime? FechaAtencion { get; set; }
        public DateTime? FechaCierre { get; set; }
        public string? TecnicoAsignado { get; set; }
        public string? Descripcion { get; set; }
        public string? Solucion { get; set; }
        public string? Prioridad { get; set; }
    }
}
