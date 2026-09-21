namespace HSis.Logic.DTOs
{
    public class ReporteExportRequestDto
    {
        public ReporteKpisDto Kpis { get; set; } = new();
        public List<TicketDto> Tickets { get; set; } = [];
        public DateTime Inicio { get; set; }
        public DateTime Fin { get; set; }
    }
}
