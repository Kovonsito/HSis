#nullable enable
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;

namespace HSis.UI.Controls;

partial class FiltroGenericoControl
{
    private System.ComponentModel.IContainer? components = null;
    private FlowLayoutPanel flowLayoutPanelMain = null!;
    private BotonModerno btnRecargar = null!;
    private BotonModerno btnLimpiar = null!;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        var tblLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 2,
            BackColor = Color.White,
            Margin = new Padding(0),
            Padding = new Padding(12, 6, 12, 6)
        };
        tblLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        flowLayoutPanelMain = new FlowLayoutPanel
        {
            AutoScroll = true,
            BackColor = Color.White,
            Dock = DockStyle.Fill,
            Location = new Point(0, 0),
            Margin = new Padding(0),
            Padding = new Padding(0),
            TabIndex = 0,
            WrapContents = false
        };

        var pnlBotones = new FlowLayoutPanel
        {
            BackColor = Color.White,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            AutoSize = true,
            Margin = new Padding(0),
            Padding = new Padding(0, 10, 0, 0)
        };

        btnLimpiar = new BotonModerno
        {
            Estilo = EstiloBotonModerno.Ghost,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
            Icono = IconChar.Eraser,
            IconoTamano = 13,
            Name = "btnLimpiar",
            Size = new Size(95, 34),
            Margin = new Padding(0, 0, 0, 0),
            TabIndex = 1,
            Text = "Limpiar"
        };
        btnLimpiar.Click += (s, e) => Limpiar_Click();

        btnRecargar = new BotonModerno
        {
            Estilo = EstiloBotonModerno.Secundario,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
            Icono = IconChar.RotateRight,
            IconoTamano = 13,
            Name = "btnRecargar",
            Size = new Size(102, 34),
            Margin = new Padding(0, 0, 8, 0),
            TabIndex = 0,
            Text = "Recargar"
        };
        btnRecargar.Click += (s, e) => RecargarClic?.Invoke(this, EventArgs.Empty);

        pnlBotones.Controls.Add(btnLimpiar);
        pnlBotones.Controls.Add(btnRecargar);

        tblLayout.Controls.Add(flowLayoutPanelMain, 0, 0);
        tblLayout.Controls.Add(pnlBotones, 1, 0);

        // FiltroGenericoControl
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.Transparent;
        Controls.Add(tblLayout);
        Name = "FiltroGenericoControl";
        Size = new Size(820, 68);
        ResumeLayout(false);
    }
}
