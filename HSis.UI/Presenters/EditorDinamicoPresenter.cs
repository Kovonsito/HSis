using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class EditorDinamicoPresenter(ICatalogoService catalogoService)
    {
        private IEditorDinamicoView? _view;

        public void SetView(IEditorDinamicoView view)
        {
            _view = view;
        }


        public async Task<List<object>> ObtenerTodosPorTipoAsync(Type tipo)
        {
            try
            {
                return await catalogoService.ObtenerTodosPorTipoAsync(tipo);
            }
            catch (Exception ex)
            {
                _view?.MostrarError($"Error al obtener catálogo: {ex.Message}");
                return [];
            }
        }

        public async Task<int> ObtenerSiguienteIdAsync(Type tipoEntidad, string nombrePropiedadKey)
        {
            try
            {
                return await catalogoService.ObtenerSiguienteIdAsync(tipoEntidad, nombrePropiedadKey);
            }
            catch
            {
                return 1;
            }
        }
    }
}

