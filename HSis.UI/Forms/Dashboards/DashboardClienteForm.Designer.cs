namespace HSis.UI
{
    partial class DashboardClienteForm
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
            this.ucMisActivos = new IndicadorControl();
            this.dgvMisTickets = new DataGridView();
            this.btnNuevoReporte = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvMisTickets)).BeginInit();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new Size(341, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Mi Panel - Mis Reportes";

            // ucMisActivos
            this.ucMisActivos.Location = new Point(12, 50);
            this.ucMisActivos.Name = "ucMisActivos";
            this.ucMisActivos.Size = new Size(200, 100);
            this.ucMisActivos.TabIndex = 1;

            // btnNuevoReporte
            this.btnNuevoReporte.BackColor = Color.FromArgb(52, 152, 219);
            this.btnNuevoReporte.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            this.btnNuevoReporte.ForeColor = Color.White;
            this.btnNuevoReporte.Location = new Point(220, 70);
            this.btnNuevoReporte.Name = "btnNuevoReporte";
            this.btnNuevoReporte.Size = new Size(150, 60);
            this.btnNuevoReporte.TabIndex = 2;
            this.btnNuevoReporte.Text = "+ Nuevo Reporte";
            this.btnNuevoReporte.UseVisualStyleBackColor = false;
            this.btnNuevoReporte.Click += new EventHandler(this.btnNuevoReporte_Click);

            // dgvMisTickets
            this.dgvMisTickets.AllowUserToAddRows = false;
            this.dgvMisTickets.AllowUserToDeleteRows = false;
            this.dgvMisTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvMisTickets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvMisTickets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.dgvMisTickets.Location = new Point(12, 170);
            this.dgvMisTickets.Name = "dgvMisTickets";
            this.dgvMisTickets.ReadOnly = true;
            this.dgvMisTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvMisTickets.Size = new Size(760, 280);
            this.dgvMisTickets.TabIndex = 3;
            this.dgvMisTickets.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvMisTickets_CellDoubleClick);

            // DashboardClienteForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1100, 700);
            this.MinimumSize = new Size(1030, 680);
            this.Controls.Add(this.dgvMisTickets);
            this.Controls.Add(this.btnNuevoReporte);
            this.Controls.Add(this.ucMisActivos);
            this.Controls.Add(this.lblTitulo);
            this.Name = "DashboardClienteForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Dashboard - Cliente";
            this.Load += new EventHandler(this.frmDashboardCliente_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvMisTickets)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTitulo;
        private IndicadorControl ucMisActivos;
        private DataGridView dgvMisTickets;
        private Button btnNuevoReporte;
    }
}
