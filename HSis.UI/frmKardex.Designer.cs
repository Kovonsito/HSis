using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI
{
    partial class frmKardex
    {
        private System.ComponentModel.IContainer components = null;

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
            this.cbMaterial = new ComboBox();
            this.dgvKardex = new DataGridView();
            Panel panelTop = new Panel();
            Label lblMaterial = new Label();

            ((System.ComponentModel.ISupportInitialize)(this.dgvKardex)).BeginInit();
            this.SuspendLayout();

            // 
            // panelTop
            // 
            panelTop.Dock = DockStyle.Top;
            panelTop.Height = 50;
            panelTop.Controls.Add(lblMaterial);
            panelTop.Controls.Add(this.cbMaterial);

            // 
            // lblMaterial
            // 
            lblMaterial.Text = "Filtrar por Material:";
            lblMaterial.Top = 15;
            lblMaterial.Left = 20;
            lblMaterial.Width = 110;

            // 
            // cbMaterial
            // 
            this.cbMaterial.Top = 12;
            this.cbMaterial.Left = 130;
            this.cbMaterial.Width = 250;
            this.cbMaterial.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cbMaterial.SelectedIndexChanged += new System.EventHandler(this.CbMaterial_SelectedIndexChanged);

            // 
            // dgvKardex
            // 
            this.dgvKardex.Dock = DockStyle.Fill;
            this.dgvKardex.ReadOnly = true;
            this.dgvKardex.AllowUserToAddRows = false;
            this.dgvKardex.AllowUserToDeleteRows = false;
            this.dgvKardex.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvKardex.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            // 
            // frmKardex
            // 
            this.Text = "Kardex / Historial de Inventario";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.Controls.Add(this.dgvKardex);
            this.Controls.Add(panelTop);
            this.Load += new System.EventHandler(this.FrmKardex_Load);

            ((System.ComponentModel.ISupportInitialize)(this.dgvKardex)).EndInit();
            this.ResumeLayout(false);
        }

        private ComboBox cbMaterial;
        private DataGridView dgvKardex;
    }
}
