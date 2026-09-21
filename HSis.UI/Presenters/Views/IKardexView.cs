using HSis.Logic.DTOs;

namespace HSis.UI.Presenters
{
    public interface IKardexView
    {
        void CargarMateriales(List<MaterialDto> materiales);
        void CargarHistorialKardex(List<KardexMovimientoDto> historial);
        void MostrarError(string mensaje);
        void MostrarCargando(bool cargando);
    }
}
