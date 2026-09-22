namespace HSis.Contracts.DTOs
{
    public class MaterialDto
    {
        public int IdMaterial { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public int StockActual { get; set; }
        public string UnidadMedida { get; set; } = string.Empty;
        public decimal CostoUnitario { get; set; }
    }

    public class KardexMovimientoDto
    {
        public int IdMovimiento { get; set; }
        public int IdMaterial { get; set; }
        public string MaterialNombre { get; set; } = string.Empty;
        public int IdUsuario { get; set; }
        public string UsuarioNombre { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public int Cantidad { get; set; }
        public decimal CostoUnitario { get; set; }
        public string Motivo { get; set; } = string.Empty;
        public string TipoMovimiento { get; set; } = string.Empty;
    }
}
