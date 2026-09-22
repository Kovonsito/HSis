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

        // Compatibilidad con código de UI y Tests
        public int TicketsNuevos { get => TotalNuevos; set => TotalNuevos = value; }
        public int TicketsUrgentes { get => TotalUrgentes; set => TotalUrgentes = value; }
        public int TicketsEnProceso { get => TotalEnProceso; set => TotalEnProceso = value; }
        public int TicketsCerrados { get => TotalCerrados; set => TotalCerrados = value; }
        public int TicketsReabiertos { get => TotalReabiertos; set => TotalReabiertos = value; }
    }
}
