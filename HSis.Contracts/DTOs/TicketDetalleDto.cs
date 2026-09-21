namespace HSis.Logic.DTOs
{
    public class TicketDetalleDto
    {
        public int IdTicket { get; set; }
        public int IdMaterial { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitarioAplicado { get; set; }
        public string? NombreMaterial { get; set; }
        public string? UnidadMedidaMaterial { get; set; }
    }
}

