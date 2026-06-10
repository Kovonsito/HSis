#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI
{
    public partial class ucPaginacion : UserControl
    {
        public event EventHandler? PageChanged;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int PageSize { get; set; } = 10;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int CurrentPage { get; set; } = 1;

        [System.ComponentModel.DesignerSerializationVisibility(System.ComponentModel.DesignerSerializationVisibility.Hidden)]
        public int TotalRecords { get; set; } = 0;

        public int TotalPages => Math.Max(1, (int)Math.Ceiling((double)TotalRecords / PageSize));

        public ucPaginacion()
        {
            InitializeComponent();
            SuscribirEventos();
        }

        private void SuscribirEventos()
        {
            if (_cmbPageSize != null)
                _cmbPageSize.SelectedIndexChanged += CmbPageSize_SelectedIndexChanged;

            if (_btnFirst != null)
                _btnFirst.Click += (s, e) => CambiarPagina(1);

            if (_btnPrev != null)
                _btnPrev.Click += (s, e) => CambiarPagina(CurrentPage - 1);

            if (_btnNext != null)
                _btnNext.Click += (s, e) => CambiarPagina(CurrentPage + 1);

            if (_btnLast != null)
                _btnLast.Click += (s, e) => CambiarPagina(TotalPages);

        }

        private void CmbPageSize_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (_cmbPageSize != null && int.TryParse(_cmbPageSize.SelectedItem?.ToString(), out int size))
            {
                PageSize = size;
                CurrentPage = 1;
                PageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void CambiarPagina(int nuevaPagina)
        {
            if (nuevaPagina < 1) nuevaPagina = 1;
            if (nuevaPagina > TotalPages) nuevaPagina = TotalPages;

            if (nuevaPagina != CurrentPage)
            {
                CurrentPage = nuevaPagina;
                PageChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public void ActualizarInterfaz()
        {
            if (_lblPageInfo != null)
            {
                _lblPageInfo.Text = $"Página {CurrentPage} de {TotalPages}";
            }

            if (_lblTotal != null)
            {
                _lblTotal.Text = $"Total: {TotalRecords} registros";
            }

            if (_btnFirst != null) _btnFirst.Enabled = CurrentPage > 1;
            if (_btnPrev != null) _btnPrev.Enabled = CurrentPage > 1;
            if (_btnNext != null) _btnNext.Enabled = CurrentPage < TotalPages;
            if (_btnLast != null) _btnLast.Enabled = CurrentPage < TotalPages;
        }
    }
}
