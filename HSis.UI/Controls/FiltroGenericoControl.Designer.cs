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
            BackColor = Color.Transparent,
            Margin = new Padding(0),
            Padding = new Padding(12, 6, 12, 6)
        };
        tblLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 225F));

        flowLayoutPanelMain = new FlowLayoutPanel
        {
            AutoScroll = false,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Location = new Point(0, 0),
            Margin = new Padding(0),
            Padding = new Padding(0),
            TabIndex = 0,
            WrapContents = false
        };

        var pnlBotones = new Panel
        {
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };

        btnRecargar = new BotonModerno
        {
            Estilo = EstiloBotonModerno.Secundario,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
            Icono = IconChar.RotateRight,
            IconoTamano = 13,
            Location = new Point(4, 14),
            Name = "btnRecargar",
            Size = new Size(100, 34),
            TabIndex = 0,
            Text = "Recargar"
        };
        btnRecargar.Click += (s, e) => RecargarClic?.Invoke(this, EventArgs.Empty);

        btnLimpiar = new BotonModerno
        {
            Estilo = EstiloBotonModerno.Ghost,
            Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
            Icono = IconChar.Eraser,
            IconoTamano = 13,
            Location = new Point(110, 14),
            Name = "btnLimpiar",
            Size = new Size(95, 34),
            TabIndex = 1,
            Text = "Limpiar"
        };
        btnLimpiar.Click += (s, e) => Limpiar_Click();

        pnlBotones.Controls.Add(btnRecargar);
        pnlBotones.Controls.Add(btnLimpiar);

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
