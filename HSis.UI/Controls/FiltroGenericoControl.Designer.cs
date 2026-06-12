#nullable enable
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI
{
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
            this.flowLayoutPanelMain = new FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // flowLayoutPanelMain
            // 
            this.flowLayoutPanelMain.Dock = DockStyle.Fill;
            this.flowLayoutPanelMain.FlowDirection = FlowDirection.LeftToRight;
            this.flowLayoutPanelMain.Location = new Point(0, 0);
            this.flowLayoutPanelMain.Name = "flowLayoutPanelMain";
            this.flowLayoutPanelMain.Size = new Size(800, 70);
            this.flowLayoutPanelMain.TabIndex = 0;
            this.flowLayoutPanelMain.WrapContents = true;
            this.flowLayoutPanelMain.AutoScroll = true;
            // 
            // FiltroGenericoControl
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanelMain);
            this.Name = "FiltroGenericoControl";
            this.Size = new Size(800, 70);
            this.ResumeLayout(false);
        }
    }
}
