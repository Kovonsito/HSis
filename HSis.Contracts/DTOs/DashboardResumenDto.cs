namespace HSis.Logic.DTOs
{
    public class DashboardResumenDto
    {
        public int TotalNuevos { get; set; }
        public int TotalUrgentes { get; set; }
        public int TotalEnProceso { get; set; }
        public int TotalCerrados { get; set; }
        public int TotalReabiertos { get; set; }
        public double PromedioCalificacion { get; set; }
    }
}
