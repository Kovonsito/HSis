namespace HSis.Data.Models;

public partial class IngresosMaterial
{
    public int IdIngreso { get; set; }

    public int IdMaterial { get; set; }

    public DateTime FechaIngreso { get; set; }

    public int Cantidad { get; set; }

    public decimal CostoCompra { get; set; }

    public int IdUsuario { get; set; }

    public virtual Material IdMaterialNavigation { get; set; } = null!;

    public virtual Usuario IdUsuarioNavigation { get; set; } = null!;
}
