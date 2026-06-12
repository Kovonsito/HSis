#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI.Controls
{
    partial class PaginacionControl
    {
        private TableLayoutPanel? _tablaPrincipal;
        private Label? _etiquetaTamanoPagina;
        private ComboBox? _comboTamanoPagina;
        private Button? _botonPrimero;
        private Button? _botonAnterior;
        private Label? _etiquetaInformacionPagina;
        private Button? _botonSiguiente;
        private Button? _botonUltimo;
        private Label? _etiquetaTotal;

        private void InitializeComponent()
        {
            this.Height = 40;
            this.BackColor = Color.White;
            this.Dock = DockStyle.Fill;
            this.Margin = new Padding(0);

            _tablaPrincipal = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 3,
                Margin = new Padding(0),
                Padding = new Padding(10, 0, 10, 0)
            };

            _tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));
            _tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.34F));
            _tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.33F));

            // 1. Lado Izquierdo: Combo de tamaño de página
            var pnlLeft = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0)
            };

            _etiquetaTamanoPagina = new Label
            {
                Text = "Registros por página:",
                AutoSize = true,
                Anchor = AnchorStyles.Left,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 8, 5, 0)
            };

            _comboTamanoPagina = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 60,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 5, 0, 0)
            };
            _comboTamanoPagina.Items.AddRange(new object[] { "10", "20", "50", "100" });
            _comboTamanoPagina.SelectedIndex = 0; // Default: 10

            pnlLeft.Controls.Add(_etiquetaTamanoPagina);
            pnlLeft.Controls.Add(_comboTamanoPagina);

            // 2. Centro: Controles de Navegación
            var pnlCenter = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Margin = new Padding(0),
                Anchor = AnchorStyles.None // Centrado
            };

            _botonPrimero = new Button
            {
                Text = "|<",
                Width = 35,
                Height = 26,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(2, 4, 2, 0)
            };

            _botonAnterior = new Button
            {
                Text = "<",
                Width = 35,
                Height = 26,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(2, 4, 2, 0)
            };

            _etiquetaInformacionPagina = new Label
            {
                Text = "Página 1 de 1",
                AutoSize = false,
                Width = 140,
                Height = 26,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(5, 7, 5, 0)
            };

            _botonSiguiente = new Button
            {
                Text = ">",
                Width = 35,
                Height = 26,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(2, 4, 2, 0)
            };

            _botonUltimo = new Button
            {
                Text = ">|",
                Width = 35,
                Height = 26,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                Margin = new Padding(2, 4, 2, 0)
            };

            pnlCenter.Controls.Add(_botonPrimero);
            pnlCenter.Controls.Add(_botonAnterior);
            pnlCenter.Controls.Add(_etiquetaInformacionPagina);
            pnlCenter.Controls.Add(_botonSiguiente);
            pnlCenter.Controls.Add(_botonUltimo);

            // Para centrar el FlowLayoutPanel dentro de su celda de TableLayoutPanel,
            // colocamos un Panel normal contenedor
            var pnlCenterContainer = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
            pnlCenterContainer.Controls.Add(pnlCenter);

            pnlCenterContainer.SizeChanged += (s, e) =>
            {
                pnlCenter.Location = new Point(
                    (pnlCenterContainer.Width - pnlCenter.Width) / 2,
                    (pnlCenterContainer.Height - pnlCenter.Height) / 2
                );
            };

            // 3. Lado Derecho: Totalizador
            var pnlRight = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                Margin = new Padding(0)
            };

            _etiquetaTotal = new Label
            {
                Text = "Total: 0 registros",
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                Margin = new Padding(0, 8, 10, 0)
            };

            pnlRight.Controls.Add(_etiquetaTotal);

            // Agregar paneles al TableLayoutPanel
            _tablaPrincipal.Controls.Add(pnlLeft, 0, 0);
            _tablaPrincipal.Controls.Add(pnlCenterContainer, 1, 0);
            _tablaPrincipal.Controls.Add(pnlRight, 2, 0);

            this.Controls.Add(_tablaPrincipal);
        }
    }
}
