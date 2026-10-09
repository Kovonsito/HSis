namespace HSis.UI.Controls
{
    // El DataGridView estandar no usa doble bufer: con filas de alto variable (texto ajustado) quedan restos de pintado entre filas.
    public sealed class DataGridViewModerno : DataGridView
    {
        public DataGridViewModerno()
        {
            DoubleBuffered = true;
        }
    }
}