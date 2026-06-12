namespace HSis.UI
{
    partial class GeneradorReportesForm
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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.dtpInicio = new System.Windows.Forms.DateTimePicker();
            this.dtpFin = new System.Windows.Forms.DateTimePicker();
            this.btnExcel = new System.Windows.Forms.Button();
            this.btnPdf = new System.Windows.Forms.Button();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblInicio = new System.Windows.Forms.Label();
            this.lblFin = new System.Windows.Forms.Label();
            this.pnlBackground = new System.Windows.Forms.Panel();
            this.pnlBackground.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlBackground
            // 
            this.pnlBackground.BackColor = System.Drawing.Color.White;
            this.pnlBackground.Controls.Add(this.lblTitle);
            this.pnlBackground.Controls.Add(this.lblInicio);
            this.pnlBackground.Controls.Add(this.dtpInicio);
            this.pnlBackground.Controls.Add(this.lblFin);
            this.pnlBackground.Controls.Add(this.dtpFin);
            this.pnlBackground.Controls.Add(this.btnExcel);
            this.pnlBackground.Controls.Add(this.btnPdf);
            this.pnlBackground.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlBackground.Location = new System.Drawing.Point(0, 0);
            this.pnlBackground.Name = "pnlBackground";
            this.pnlBackground.Size = new System.Drawing.Size(434, 261);
            this.pnlBackground.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI Semibold", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            this.lblTitle.Location = new System.Drawing.Point(24, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(306, 25);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "Generador de Reportes Ejecutivos";
            // 
            // lblInicio
            // 
            this.lblInicio.AutoSize = true;
            this.lblInicio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblInicio.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblInicio.Location = new System.Drawing.Point(26, 68);
            this.lblInicio.Name = "lblInicio";
            this.lblInicio.Size = new System.Drawing.Size(83, 19);
            this.lblInicio.TabIndex = 1;
            this.lblInicio.Text = "Fecha Inicio:";
            // 
            // dtpInicio
            // 
            this.dtpInicio.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpInicio.Location = new System.Drawing.Point(120, 65);
            this.dtpInicio.Name = "dtpInicio";
            this.dtpInicio.Size = new System.Drawing.Size(280, 25);
            this.dtpInicio.TabIndex = 2;
            // 
            // lblFin
            // 
            this.lblFin.AutoSize = true;
            this.lblFin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.lblFin.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            this.lblFin.Location = new System.Drawing.Point(26, 118);
            this.lblFin.Name = "lblFin";
            this.lblFin.Size = new System.Drawing.Size(69, 19);
            this.lblFin.TabIndex = 3;
            this.lblFin.Text = "Fecha Fin:";
            // 
            // dtpFin
            // 
            this.dtpFin.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.dtpFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dtpFin.Location = new System.Drawing.Point(120, 115);
            this.dtpFin.Name = "dtpFin";
            this.dtpFin.Size = new System.Drawing.Size(280, 25);
            this.dtpFin.TabIndex = 4;
            // 
            // btnExcel
            // 
            this.btnExcel.BackColor = System.Drawing.Color.FromArgb(16, 185, 129);
            this.btnExcel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnExcel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnExcel.ForeColor = System.Drawing.Color.White;
            this.btnExcel.Location = new System.Drawing.Point(50, 180);
            this.btnExcel.Name = "btnExcel";
            this.btnExcel.Size = new System.Drawing.Size(160, 40);
            this.btnExcel.TabIndex = 5;
            this.btnExcel.Text = "Exportar a Excel";
            this.btnExcel.UseVisualStyleBackColor = false;
            this.btnExcel.Click += new System.EventHandler(this.btnExcel_Click);
            // 
            // btnPdf
            // 
            this.btnPdf.BackColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnPdf.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPdf.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.btnPdf.ForeColor = System.Drawing.Color.White;
            this.btnPdf.Location = new System.Drawing.Point(230, 180);
            this.btnPdf.Name = "btnPdf";
            this.btnPdf.Size = new System.Drawing.Size(160, 40);
            this.btnPdf.TabIndex = 6;
            this.btnPdf.Text = "Exportar a PDF";
            this.btnPdf.UseVisualStyleBackColor = false;
            this.btnPdf.Click += new System.EventHandler(this.btnPdf_Click);
            // 
            // GeneradorReportesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(434, 261);
            this.Controls.Add(this.pnlBackground);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GeneradorReportesForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Generar Reportes Especializados";
            this.Load += new System.EventHandler(this.frmGeneradorReportes_Load);
            this.pnlBackground.ResumeLayout(false);
            this.pnlBackground.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dtpInicio;
        private System.Windows.Forms.DateTimePicker dtpFin;
        private System.Windows.Forms.Button btnExcel;
        private System.Windows.Forms.Button btnPdf;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInicio;
        private System.Windows.Forms.Label lblFin;
        private System.Windows.Forms.Panel pnlBackground;
    }
}
