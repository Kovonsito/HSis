namespace HSis.Data.Models
{
    public partial class DetTicket
    {
        public int IdTicket { get; set; }

        public int IdMaterial { get; set; }

        public decimal CostoUnitarioAplicado { get; set; }

        public int Cantidad { get; set; }

        public virtual Material Material { get; set; } = null!;

        public virtual Ticket Ticket { get; set; } = null!;
    }
}
