namespace HSis.UI.Controls
{
    // El DataGridView estandar no usa doble bufer: con filas de alto variable (texto ajustado) quedan restos de pintado entre filas.
    public sealed class DataGridViewModerno : DataGridView
    {
        public DataGridViewModerno()
        {
            DoubleBuffered = true;
        }

        // Al enlazar datos WinForms marca la primera fila y celda; se limpia para que la tabla no abra con una fila resaltada.
        protected override void OnDataBindingComplete(DataGridViewBindingCompleteEventArgs e)
        {
            base.OnDataBindingComplete(e);
            CurrentCell = null;
            ClearSelection();
        }
    }
}