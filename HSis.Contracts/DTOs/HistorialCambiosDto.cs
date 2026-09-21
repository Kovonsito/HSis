namespace HSis.Logic.DTOs
{
    public class HistorialCambiosDto
    {
        public int IdTicket { get; set; }
        public string UsuarioCambio { get; set; } = string.Empty;
        public DateTime FechaMovimiento { get; set; }
        public string CampoModificado { get; set; } = string.Empty;
        public string ValorAnterior { get; set; } = string.Empty;
        public string ValorNuevo { get; set; } = string.Empty;
    }
}
