namespace HSis.UI.Controls

{
    public enum TipoFiltroControl
    {
        Texto,
        ComboSeleccion,
        Fecha
    }

    public class FiltroCampo
    {
        public string NombrePropiedad { get; set; } = string.Empty; // Clave para recuperar el valor
        public string Etiqueta { get; set; } = string.Empty;        // Texto que verá el usuario
        public TipoFiltroControl Tipo { get; set; }
        public object[]? ValoresCombo { get; set; }                  // Elementos si es un combobox
        public int Ancho { get; set; } = 150;                        // Ancho sugerido del input
        public object? ValorDefecto { get; set; }                    // Valor por defecto opcional
    }
}
