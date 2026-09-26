namespace HSis.Contracts.DTOs
{
    public class FeedbackTecnicoDto
    {
        public int IdTicket { get; set; }
        public string Folio => IdTicket.ToString(System.Globalization.CultureInfo.InvariantCulture);
        public string? NombreUsuario { get; set; }
        public string Calificacion { get; set; } = null!;
        public string? Puntuacion => Calificacion;
        public string? Comentario { get; set; }
        public DateTime? Fecha { get; set; }
        public DateTime? FechaRegistro => Fecha;
    }
}

