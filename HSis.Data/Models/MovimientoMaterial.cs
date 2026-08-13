namespace HSis.Data.Models
{
    public partial class MovimientoMaterial
    {
        public int IdMovimiento { get; set; }

        public int IdMaterial { get; set; }

        public int Cantidad { get; set; }

        public decimal CostoUnitario { get; set; }

        public DateTime FechaMovimiento { get; set; }

        public int IdUsuario { get; set; }

        public string? Motivo { get; set; }

        public virtual Material Material { get; set; } = null!;

        public virtual Usuario Usuario { get; set; } = null!;
    }
}

