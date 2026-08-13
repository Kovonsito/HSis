namespace HSis.UI.Presenters
{
    public interface IGeneradorReportesView
    {
        DateTime FechaInicio { get; set; }
        DateTime FechaFin { get; set; }
        void MostrarError(string mensaje);
        void MostrarExito(string mensaje);
        void MostrarCargando(bool cargando);
    }
}
