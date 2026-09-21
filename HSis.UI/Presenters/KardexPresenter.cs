using HSis.Logic.DTOs;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class KardexPresenter(IMaterialService materialService)
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
                var materiales = await materialService.ObtenerMaterialesAsync();
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
                var historial = await materialService.ObtenerKardexPorMaterialAsync(idMaterial);
                _view.CargarHistorialKardex(historial);
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
