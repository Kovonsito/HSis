#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI.Controls
{
    public partial class PaginacionControl : UserControl
    {
        public event EventHandler? PaginaCambiada;

        private int _tamanoPagina = 10;
        private int _paginaActual = 1;
        private int _totalRegistros = 0;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int TamanoPagina
        {
            get => _tamanoPagina;
            set
            {
                if (_tamanoPagina != value)
                {
                    _tamanoPagina = value;
                    ActualizarInterfaz();
                }
            }
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int PaginaActual
        {
            get => _paginaActual;
            set
            {
                if (_paginaActual != value)
                {
                    _paginaActual = value;
                    ActualizarInterfaz();
                }
            }
        }

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int TotalRegistros
        {
            get => _totalRegistros;
            set
            {
                if (_totalRegistros != value)
                {
                    _totalRegistros = value;
                    ActualizarInterfaz();
                }
            }
        }

        public int TotalPaginas => Math.Max(1, (int)Math.Ceiling((double)TotalRegistros / TamanoPagina));

        public PaginacionControl()
        {
            InitializeComponent();
            SuscribirEventos();
        }

        private void SuscribirEventos()
        {
            if (_comboTamanoPagina != null)
                _comboTamanoPagina.SelectedIndexChanged += CmbTamanoPagina_SelectedIndexChanged;

            if (_botonPrimero != null)
                _botonPrimero.Click += (s, e) => CambiarPagina(1);

            if (_botonAnterior != null)
                _botonAnterior.Click += (s, e) => CambiarPagina(PaginaActual - 1);

            if (_botonSiguiente != null)
                _botonSiguiente.Click += (s, e) => CambiarPagina(PaginaActual + 1);

            if (_botonUltimo != null)
                _botonUltimo.Click += (s, e) => CambiarPagina(TotalPaginas);

        }

        private void CmbTamanoPagina_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_comboTamanoPagina != null && int.TryParse(_comboTamanoPagina.SelectedItem?.ToString(), out int size))
            {
                TamanoPagina = size;
                PaginaActual = 1;
                PaginaCambiada?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1) nuevaPagina = 1;
            if (nuevaPagina > TotalPaginas) nuevaPagina = TotalPaginas;

            if (nuevaPagina != PaginaActual)
            {
                PaginaActual = nuevaPagina;
                PaginaCambiada?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ActualizarInterfaz()
        {
            if (_etiquetaInformacionPagina != null)
            {
                _etiquetaInformacionPagina.Text = $"Página {PaginaActual} de {TotalPaginas}";
            }

            if (_etiquetaTotal != null)
            {
                _etiquetaTotal.Text = $"Total: {TotalRegistros} registros";
            }

            if (_botonPrimero != null) _botonPrimero.Enabled = PaginaActual > 1;
            if (_botonAnterior != null) _botonAnterior.Enabled = PaginaActual > 1;
            if (_botonSiguiente != null) _botonSiguiente.Enabled = PaginaActual < TotalPaginas;
            if (_botonUltimo != null) _botonUltimo.Enabled = PaginaActual < TotalPaginas;
        }

        private void _botonSiguiente_Click(object sender, EventArgs e)
        {

        }

        private void PaginacionControl_Load(object sender, EventArgs e)
        {

        }
    }
}
