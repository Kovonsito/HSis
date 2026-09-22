#nullable enable
using System.ComponentModel;
using System.Runtime.Versioning;
using HSis.Contracts.DTOs;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class VistaTicketsDashboardControl : UserControl
    {
        private readonly TableLayoutPanel _tblPrincipal;
        private TableLayoutPanel? _tblIndicadores;
        private readonly FiltroGenericoControl _filtroGenerico;
        private readonly DataGridView _dgvTickets;
        private readonly PaginacionControl _paginacionControl;
        private readonly ControladorPaginacionGrid _controladorPaginacion;

        // Exponer controles principales de forma limpia
        public FiltroGenericoControl Filtro => _filtroGenerico;
        public DataGridView Grid => _dgvTickets;
        public PaginacionControl Paginacion => _paginacionControl;
        public ControladorPaginacionGrid ControladorPaginacion => _controladorPaginacion;

        public event EventHandler? RecargarClic;
        public event EventHandler? LimpiarClic;
        public event EventHandler? FiltroCambiado;

        public VistaTicketsDashboardControl()
        {
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.UserPaint, true);

            DoubleBuffered = true;
            BackColor = TemaVisual.FondoApp;
            Dock = DockStyle.Fill;

            // 1. Grid estilizado
            _dgvTickets = new DataGridView
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 0, 6)
            };
            _dgvTickets.AplicarTemaModerno();

            // 2. Filtro genérico
            _filtroGenerico = new FiltroGenericoControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 4, 0, 4)
            };
            _filtroGenerico.RecargarClic += (s, e) => RecargarClic?.Invoke(this, e);
            _filtroGenerico.LimpiarClic += (s, e) => LimpiarClic?.Invoke(this, e);
            _filtroGenerico.FiltroCambiado += (s, e) => FiltroCambiado?.Invoke(this, e);

            // 3. Paginación
            _paginacionControl = new PaginacionControl
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(0)
            };
            _controladorPaginacion = new ControladorPaginacionGrid(_paginacionControl);

            // 4. Panel principal en franjas
            _tblPrincipal = AyudanteDisenoPanel.CrearPanelPrincipal(Size, incluirFiltros: true);
            _tblPrincipal.Controls.Add(_filtroGenerico, 0, 1);
            _tblPrincipal.Controls.Add(_dgvTickets, 0, 2);
            _tblPrincipal.Controls.Add(_paginacionControl, 0, 3);

            Controls.Add(_tblPrincipal);
        }

        /// <summary>
        /// Configura los indicadores KPI superiores en la franja 0.
        /// </summary>
        public void ConfigurarIndicadores(params Control[] controlesKPI)
        {
            if (_tblIndicadores != null)
            {
                _tblPrincipal.Controls.Remove(_tblIndicadores);
                _tblIndicadores.Dispose();
                _tblIndicadores = null;
            }

            if (controlesKPI.Length > 0)
            {
                _tblIndicadores = AyudanteDisenoPanel.CrearPanelIndicadores(
                    "tblKPIsDashboard",
                    controlesKPI.Length,
                    controlesKPI
                );
                _tblPrincipal.Controls.Add(_tblIndicadores, 0, 0);
            }
        }

        /// <summary>
        /// Inicializa los campos de filtro con las definiciones por rol.
        /// </summary>
        public void InicializarFiltros(List<FiltroCampo> campos)
        {
            _filtroGenerico.InicializarFiltros(campos);
        }

        /// <summary>
        /// Establece el valor de un filtro (por ejemplo, al hacer clic en un KPI).
        /// </summary>
        public void EstablecerValorFiltro(string nombrePropiedad, object? valor)
        {
            _filtroGenerico.EstablecerValorFiltro(nombrePropiedad, valor);
        }

        /// <summary>
        /// Obtiene los valores actuales de los filtros.
        /// </summary>
        public Dictionary<string, object?> ObtenerValoresFiltros()
        {
            return _filtroGenerico.ObtenerValoresFiltros();
        }

        /// <summary>
        /// Vincula la acción de cambio de página asíncrona al paginador.
        /// </summary>
        public void VincularPaginacion(Func<Task> alCambiarPagina)
        {
            _controladorPaginacion.Vincular(alCambiarPagina);
        }

        /// <summary>
        /// Vincula la acción de cambio de página sincrónica al paginador.
        /// </summary>
        public void VincularPaginacion(Action alCambiarPagina)
        {
            _controladorPaginacion.Vincular(alCambiarPagina);
        }

        /// <summary>
        /// Actualiza la vista de paginación con el total de registros actual.
        /// </summary>
        public void ActualizarPaginacion(int totalRegistros, int paginaActual, int tamanoPagina)
        {
            _controladorPaginacion.Actualizar(totalRegistros, paginaActual, tamanoPagina);
        }

        public void ActualizarPaginacion(int totalRegistros)
        {
            _controladorPaginacion.Actualizar(totalRegistros);
        }

        public void ReiniciarAPrimeraPagina()
        {
            _controladorPaginacion.ReiniciarAPrimeraPagina();
        }

        public IEnumerable<T> ObtenerPagina<T>(IEnumerable<T> fuente)
        {
            return _controladorPaginacion.ObtenerPagina(fuente);
        }
    }
}
