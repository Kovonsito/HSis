#nullable enable
using System.Runtime.Versioning;
using HSis.UI.Helpers;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class VistaTicketsDashboardControl : UserControl
    {
        private readonly TableLayoutPanel _tblPrincipal;
        private TableLayoutPanel? _tblIndicadores;
        private FlowLayoutPanel? _pnlAcciones;
        private readonly Dictionary<TipoKpiDashboard, IndicadorControl> _kpisActivos = [];

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
        public event Action<TipoKpiDashboard>? KpiClic;

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
            _tblPrincipal.Controls.Add(_filtroGenerico, 0, 2);
            _tblPrincipal.Controls.Add(_dgvTickets, 0, 3);
            _tblPrincipal.Controls.Add(_paginacionControl, 0, 4);

            Controls.Add(_tblPrincipal);
        }

        /// <summary>
        /// Configura los indicadores KPI seleccionados y opcionalmente los botones de acción en la cabecera.
        /// Crea internamente los controles de KPI en su propio contenedor y los botones en el suyo.
        /// </summary>
        public void ConfigurarKpis(IEnumerable<TipoKpiDashboard> tipos, IEnumerable<Control>? botonesAccion = null)
        {
            _kpisActivos.Clear();

            var listaTipos = tipos.ToList();
            var controlesKpi = new List<Control>();

            foreach (var tipo in listaTipos)
            {
                var (titulo, color, imagen) = CatalogoKpis.ObtenerMetadatos(tipo);
                var indicador = new IndicadorControl
                {
                    Titulo = titulo,
                    Cantidad = "0",
                    ColorFondo = color,
                    ImagenFondo = imagen
                };

                indicador.IndicadorClic += (s, e) => KpiClic?.Invoke(tipo);
                _kpisActivos[tipo] = indicador;
                controlesKpi.Add(indicador);
            }

            var listaBotones = botonesAccion?.ToList() ?? [];
            ReconstruirCabecera(controlesKpi, listaBotones);
        }

        private void ReconstruirCabecera(List<Control> controlesKpi, List<Control> botones)
        {
            foreach (Control control in _tblPrincipal.Controls.Cast<Control>().ToList())
            {
                var posicion = _tblPrincipal.GetPositionFromControl(control);
                if (posicion.Row is 0 or 1)
                {
                    _tblPrincipal.Controls.Remove(control);
                    control.Dispose();
                }
            }

            _tblIndicadores = null;
            _pnlAcciones = null;

            if (controlesKpi.Count > 0)
            {
                _tblIndicadores = AyudanteDisenoPanel.CrearPanelIndicadores(
                    "tblKPIsDashboard",
                    controlesKpi.Count,
                    controlesKpi.ToArray());
                _tblIndicadores.Dock = DockStyle.Fill;
                _tblIndicadores.AutoSize = false;
                _tblPrincipal.Controls.Add(_tblIndicadores, 0, 0);
            }

            if (botones.Count > 0)
            {
                _pnlAcciones = new FlowLayoutPanel
                {
                    Dock = DockStyle.Fill,
                    FlowDirection = FlowDirection.RightToLeft,
                    WrapContents = true,
                    AutoScroll = true,
                    Padding = new Padding(0, 4, 0, 4),
                    Margin = new Padding(0),
                    BackColor = TemaVisual.FondoApp
                };

                foreach (var btn in botones)
                {
                    btn.Visible = true;
                    btn.AutoSize = false;
                    btn.Size = new Size(Math.Max(140, btn.Width), 42);
                    btn.Margin = new Padding(8, 2, 0, 2);
                    _pnlAcciones.Controls.Add(btn);
                }

                _tblPrincipal.Controls.Add(_pnlAcciones, 0, 1);
            }
        }

        /// <summary>
        /// Actualiza la cantidad o valor visible de un KPI configurado.
        /// </summary>
        public void ActualizarValorKpi(TipoKpiDashboard tipo, object valor, bool esCalificacionEstrellas = false)
        {
            if (!_kpisActivos.TryGetValue(tipo, out var indicador)) return;

            if (esCalificacionEstrellas && valor is double d)
            {
                indicador.Cantidad = d > 0 ? $"⭐ {d:F1}" : "⭐ N/A";
            }
            else if (esCalificacionEstrellas && valor is float f)
            {
                indicador.Cantidad = f > 0 ? $"⭐ {f:F1}" : "⭐ N/A";
            }
            else
            {
                indicador.Cantidad = valor?.ToString() ?? "0";
            }
        }

        /// <summary>
        /// Devuelve el control del indicador específico si se requiere personalización avanzada.
        /// </summary>
        public IndicadorControl? ObtenerIndicador(TipoKpiDashboard tipo)
        {
            _kpisActivos.TryGetValue(tipo, out var ctrl);
            return ctrl;
        }
    }
}
