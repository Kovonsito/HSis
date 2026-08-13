namespace HSis.UI.Presenters
{
    public interface IEditorDinamicoView
    {
        object Entidad { get; }
        void MostrarError(string mensaje);
    }
}

