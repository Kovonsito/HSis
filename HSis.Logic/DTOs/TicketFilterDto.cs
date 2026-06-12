namespace HSis.Logic.DTOs

{
    public enum VistaTemporal
    {
        Dia,
        Semana,
        Mes,
        Ano,
        Todos
    }

    public class TicketFilterDto
    {
        public string? UsuarioEmisor { get; set; }
        public string? Estatus { get; set; }
        public int? IdTecnico { get; set; }
        public string? Prioridad { get; set; }
        public DateTime? FechaAltaInicio { get; set; }
        public DateTime? FechaAltaFin { get; set; }
        public VistaTemporal? RangoTemporal { get; set; }
    }
}
