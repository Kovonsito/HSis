using HSis.Data.Models;

namespace HSis.UI.Presenters
{
    public interface IKardexView
    {
        void CargarMateriales(List<Material> materiales);
        void CargarHistorialKardex(List<VHistorialInventario> historial);
        void MostrarError(string mensaje);
        void MostrarCargando(bool cargando);
    }
}

