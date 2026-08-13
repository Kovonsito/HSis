namespace HSis.Data.Models
{
    public partial class Material
    {
        public int IdMaterial { get; set; }

        public string Nombre { get; set; } = null!;

        public decimal Costo { get; set; }

        public int Inventario { get; set; }

        public string UnidadMedida { get; set; } = null!;

        public virtual ICollection<DetTicket> DetTickets { get; set; } = [];

        public virtual ICollection<MovimientoMaterial> MovimientosMateriales { get; set; } = [];
    }
}
