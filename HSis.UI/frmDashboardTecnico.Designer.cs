namespace HSis.UI
{
    partial class frmDashboardTecnico
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
            this.lblTitulo = new Label();
            this.ucMisAsignados = new ucIndicador();
            this.ucDisponibles = new ucIndicador();
            this.dgvTicketsOperativos = new DataGridView();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTicketsOperativos)).BeginInit();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new Size(362, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Panel de Control - Técnico";

            // ucMisAsignados
            this.ucMisAsignados.Location = new Point(12, 50);
            this.ucMisAsignados.Name = "ucMisAsignados";
            this.ucMisAsignados.Size = new Size(200, 100);
            this.ucMisAsignados.TabIndex = 1;

            // ucDisponibles
            this.ucDisponibles.Location = new Point(220, 50);
            this.ucDisponibles.Name = "ucDisponibles";
            this.ucDisponibles.Size = new Size(200, 100);
            this.ucDisponibles.TabIndex = 2;

            // dgvTicketsOperativos
            this.dgvTicketsOperativos.AllowUserToAddRows = false;
            this.dgvTicketsOperativos.AllowUserToDeleteRows = false;
            this.dgvTicketsOperativos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvTicketsOperativos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTicketsOperativos.Location = new Point(12, 170);
            this.dgvTicketsOperativos.Name = "dgvTicketsOperativos";
            this.dgvTicketsOperativos.ReadOnly = true;
            this.dgvTicketsOperativos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTicketsOperativos.Size = new Size(760, 280);
            this.dgvTicketsOperativos.TabIndex = 3;
            this.dgvTicketsOperativos.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvTicketsOperativos_CellDoubleClick);

            // frmDashboardTecnico
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(784, 461);
            this.Controls.Add(this.dgvTicketsOperativos);
            this.Controls.Add(this.ucDisponibles);
            this.Controls.Add(this.ucMisAsignados);
            this.Controls.Add(this.lblTitulo);
            this.Name = "frmDashboardTecnico";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Dashboard - Técnico";
            this.Load += new EventHandler(this.frmDashboardTecnico_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTicketsOperativos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTitulo;
        private ucIndicador ucMisAsignados;
        private ucIndicador ucDisponibles;
        private DataGridView dgvTicketsOperativos;
    }
}
