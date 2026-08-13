#nullable enable
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI.Controls;

partial class FiltroGenericoControl
{
    private System.ComponentModel.IContainer? components = null;
    private FlowLayoutPanel flowLayoutPanelMain = null!;

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
        flowLayoutPanelMain = new FlowLayoutPanel();
        SuspendLayout();
        // 
        // flowLayoutPanelMain
        // 
        flowLayoutPanelMain.AutoScroll = true;
        flowLayoutPanelMain.Dock = DockStyle.Fill;
        flowLayoutPanelMain.Location = new Point(0, 0);
        flowLayoutPanelMain.Name = "flowLayoutPanelMain";
        flowLayoutPanelMain.Size = new Size(800, 93);
        flowLayoutPanelMain.TabIndex = 0;
        // 
        // FiltroGenericoControl
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        Controls.Add(flowLayoutPanelMain);
        Name = "FiltroGenericoControl";
        Size = new Size(800, 93);
        ResumeLayout(false);

    }
}
