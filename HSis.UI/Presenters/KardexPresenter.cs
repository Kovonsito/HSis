using HSis.Data.Models;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class KardexPresenter(ICatalogoService catalogoService)
    {
        private IKardexView? _view;

        public void SetView(IKardexView view)
        {
            _view = view;
        }

        public async Task CargarMaterialesAsync()
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var materiales = await catalogoService.ObtenerTodosAsync<Material>();
                _view.CargarMateriales(materiales);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar materiales: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }

        public async Task CargarKardexPorMaterialAsync(int idMaterial)
        {
            if (_view == null) return;
            try
            {
                _view.MostrarCargando(true);
                var historialCompleto = await catalogoService.ObtenerFiltradoAsync<VHistorialInventario>(h => h.IdMaterial == idMaterial);
                var historialFiltrado = historialCompleto.OrderByDescending(h => h.Fecha).ToList();
                _view.CargarHistorialKardex(historialFiltrado);
            }
            catch (Exception ex)
            {
                _view.MostrarError($"Error al cargar Kardex: {ex.Message}");
            }
            finally
            {
                _view.MostrarCargando(false);
            }
        }
    }
}
