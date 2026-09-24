#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace HSis.UI.Controls;

partial class PaginacionControl
{
    private TableLayoutPanel? _tablaPrincipal;
    private Label? _etiquetaTamanoPagina;
    private ComboBox? _comboTamanoPagina;
    private BotonModerno? _botonPrimero;
    private BotonModerno? _botonAnterior;
    private Label? _etiquetaInformacionPagina;
    private BotonModerno? _botonSiguiente;
    private BotonModerno? _botonUltimo;
    private Label? _etiquetaTotal;
    private Panel? _pnlCenterContainer;
    private FlowLayoutPanel? _pnlCenter;

    private void InitializeComponent()
    {
        _tablaPrincipal = new TableLayoutPanel();
        pnlLeft = new FlowLayoutPanel();
        _etiquetaTamanoPagina = new Label();
        _comboTamanoPagina = new ComboBox();
        _pnlCenterContainer = new Panel();
        _pnlCenter = new FlowLayoutPanel();
        _botonPrimero = new BotonModerno();
        _botonAnterior = new BotonModerno();
        _etiquetaInformacionPagina = new Label();
        _botonSiguiente = new BotonModerno();
        _botonUltimo = new BotonModerno();
        pnlRight = new FlowLayoutPanel();
        _etiquetaTotal = new Label();
        _tablaPrincipal.SuspendLayout();
        pnlLeft.SuspendLayout();
        _pnlCenterContainer.SuspendLayout();
        _pnlCenter.SuspendLayout();
        pnlRight.SuspendLayout();
        SuspendLayout();
        // 
        // _tablaPrincipal
        // 
        _tablaPrincipal.ColumnCount = 3;
        _tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        _tablaPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        _tablaPrincipal.Controls.Add(pnlLeft, 0, 0);
        _tablaPrincipal.Controls.Add(_pnlCenterContainer, 1, 0);
        _tablaPrincipal.Controls.Add(pnlRight, 2, 0);
        _tablaPrincipal.Dock = DockStyle.Fill;
        _tablaPrincipal.Location = new Point(0, 0);
        _tablaPrincipal.Name = "_tablaPrincipal";
        _tablaPrincipal.RowCount = 1;
        _tablaPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        _tablaPrincipal.Size = new Size(1000, 42);
        _tablaPrincipal.TabIndex = 0;
        // 
        // pnlLeft
        // 
        pnlLeft.AutoSize = true;
        pnlLeft.Controls.Add(_etiquetaTamanoPagina);
        pnlLeft.Controls.Add(_comboTamanoPagina);
        pnlLeft.Dock = DockStyle.Left;
        pnlLeft.FlowDirection = FlowDirection.LeftToRight;
        pnlLeft.Location = new Point(3, 3);
        pnlLeft.Name = "pnlLeft";
        pnlLeft.Size = new Size(185, 36);
        pnlLeft.WrapContents = false;
        pnlLeft.TabIndex = 0;
        // 
        // _etiquetaTamanoPagina
        // 
        _etiquetaTamanoPagina.AutoSize = true;
        _etiquetaTamanoPagina.Font = new Font("Segoe UI", 9F);
        _etiquetaTamanoPagina.ForeColor = Color.FromArgb(100, 116, 139);
        _etiquetaTamanoPagina.Location = new Point(3, 8);
        _etiquetaTamanoPagina.Margin = new Padding(3, 8, 3, 0);
        _etiquetaTamanoPagina.Name = "_etiquetaTamanoPagina";
        _etiquetaTamanoPagina.Size = new Size(95, 15);
        _etiquetaTamanoPagina.TabIndex = 0;
        _etiquetaTamanoPagina.Text = "Mostrar por pág.:";
        // 
        // _comboTamanoPagina
        // 
        _comboTamanoPagina.DropDownStyle = ComboBoxStyle.DropDownList;
        _comboTamanoPagina.Font = new Font("Segoe UI", 9F);
        _comboTamanoPagina.FormattingEnabled = true;
        _comboTamanoPagina.Items.AddRange(new object[] { "10", "20", "50", "100" });
        _comboTamanoPagina.Location = new Point(104, 5);
        _comboTamanoPagina.Name = "_comboTamanoPagina";
        _comboTamanoPagina.Size = new Size(65, 23);
        _comboTamanoPagina.TabIndex = 1;
        // 
        // _pnlCenterContainer
        // 
        _pnlCenterContainer.Controls.Add(_pnlCenter);
        _pnlCenterContainer.Dock = DockStyle.Fill;
        _pnlCenterContainer.Location = new Point(194, 3);
        _pnlCenterContainer.Name = "_pnlCenterContainer";
        _pnlCenterContainer.Size = new Size(595, 36);
        _pnlCenterContainer.TabIndex = 1;
        _pnlCenterContainer.SizeChanged += PnlCenterContainer_SizeChanged;
        // 
        // _pnlCenter
        // 
        _pnlCenter.AutoSize = true;
        _pnlCenter.Controls.Add(_botonPrimero);
        _pnlCenter.Controls.Add(_botonAnterior);
        _pnlCenter.Controls.Add(_etiquetaInformacionPagina);
        _pnlCenter.Controls.Add(_botonSiguiente);
        _pnlCenter.Controls.Add(_botonUltimo);
        _pnlCenter.FlowDirection = FlowDirection.LeftToRight;
        _pnlCenter.Location = new Point(0, 0);
        _pnlCenter.Name = "_pnlCenter";
        _pnlCenter.Size = new Size(380, 36);
        _pnlCenter.WrapContents = false;
        _pnlCenter.TabIndex = 0;
        // 
        // _botonPrimero
        // 
        _botonPrimero.Estilo = EstiloBotonModerno.Secundario;
        _botonPrimero.Icono = IconChar.AnglesLeft;
        _botonPrimero.IconoTamano = 13;
        _botonPrimero.Location = new Point(3, 4);
        _botonPrimero.Name = "_botonPrimero";
        _botonPrimero.RadioBorde = 6;
        _botonPrimero.Size = new Size(32, 28);
        _botonPrimero.TabIndex = 0;
        _botonPrimero.Text = "";
        // 
        // _botonAnterior
        // 
        _botonAnterior.Estilo = EstiloBotonModerno.Secundario;
        _botonAnterior.Icono = IconChar.AngleLeft;
        _botonAnterior.IconoTamano = 13;
        _botonAnterior.Location = new Point(41, 4);
        _botonAnterior.Name = "_botonAnterior";
        _botonAnterior.RadioBorde = 6;
        _botonAnterior.Size = new Size(32, 28);
        _botonAnterior.TabIndex = 1;
        _botonAnterior.Text = "";
        // 
        // _etiquetaInformacionPagina
        // 
        _etiquetaInformacionPagina.AutoSize = true;
        _etiquetaInformacionPagina.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        _etiquetaInformacionPagina.ForeColor = Color.FromArgb(51, 65, 85);
        _etiquetaInformacionPagina.Location = new Point(81, 8);
        _etiquetaInformacionPagina.Margin = new Padding(8, 8, 8, 0);
        _etiquetaInformacionPagina.Name = "_etiquetaInformacionPagina";
        _etiquetaInformacionPagina.Size = new Size(88, 17);
        _etiquetaInformacionPagina.TabIndex = 2;
        _etiquetaInformacionPagina.Text = "Página 1 de 1";
        // 
        // _botonSiguiente
        // 
        _botonSiguiente.Estilo = EstiloBotonModerno.Secundario;
        _botonSiguiente.Icono = IconChar.AngleRight;
        _botonSiguiente.IconoTamano = 13;
        _botonSiguiente.Location = new Point(183, 4);
        _botonSiguiente.Name = "_botonSiguiente";
        _botonSiguiente.RadioBorde = 6;
        _botonSiguiente.Size = new Size(32, 28);
        _botonSiguiente.TabIndex = 3;
        _botonSiguiente.Text = "";
        // 
        // _botonUltimo
        // 
        _botonUltimo.Estilo = EstiloBotonModerno.Secundario;
        _botonUltimo.Icono = IconChar.AnglesRight;
        _botonUltimo.IconoTamano = 13;
        _botonUltimo.Location = new Point(221, 4);
        _botonUltimo.Name = "_botonUltimo";
        _botonUltimo.RadioBorde = 6;
        _botonUltimo.Size = new Size(32, 28);
        _botonUltimo.TabIndex = 4;
        _botonUltimo.Text = "";
        // 
        // pnlRight
        // 
        pnlRight.AutoSize = true;
        pnlRight.Controls.Add(_etiquetaTotal);
        pnlRight.Dock = DockStyle.Right;
        pnlRight.FlowDirection = FlowDirection.RightToLeft;
        pnlRight.Location = new Point(795, 3);
        pnlRight.Name = "pnlRight";
        pnlRight.Size = new Size(110, 36);
        pnlRight.WrapContents = false;
        pnlRight.TabIndex = 2;
        // 
        // _etiquetaTotal
        // 
        _etiquetaTotal.AutoSize = true;
        _etiquetaTotal.Font = new Font("Segoe UI", 9F);
        _etiquetaTotal.ForeColor = Color.FromArgb(100, 116, 139);
        _etiquetaTotal.Location = new Point(3, 8);
        _etiquetaTotal.Margin = new Padding(3, 8, 6, 0);
        _etiquetaTotal.Name = "_etiquetaTotal";
        _etiquetaTotal.Size = new Size(95, 15);
        _etiquetaTotal.TabIndex = 0;
        _etiquetaTotal.Text = "Total: 0 registros";
        // 
        // PaginacionControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.White;
        Controls.Add(_tablaPrincipal);
        Margin = new Padding(0);
        Name = "PaginacionControl";
        Size = new Size(1000, 42);
        _tablaPrincipal.ResumeLayout(false);
        _tablaPrincipal.PerformLayout();
        pnlLeft.ResumeLayout(false);
        pnlLeft.PerformLayout();
        _pnlCenterContainer.ResumeLayout(false);
        _pnlCenterContainer.PerformLayout();
        _pnlCenter.ResumeLayout(false);
        _pnlCenter.PerformLayout();
        pnlRight.ResumeLayout(false);
        pnlRight.PerformLayout();
        ResumeLayout(false);
    }

    private void PnlCenterContainer_SizeChanged(object? sender, EventArgs e)
    {
        if (_pnlCenterContainer != null && _pnlCenter != null)
        {
            _pnlCenter.Location = new Point(
                Math.Max(0, (_pnlCenterContainer.Width - _pnlCenter.Width) / 2),
                Math.Max(0, (_pnlCenterContainer.Height - _pnlCenter.Height) / 2)
            );
        }
    }

    private FlowLayoutPanel? pnlLeft;
    private FlowLayoutPanel? pnlRight;
}
