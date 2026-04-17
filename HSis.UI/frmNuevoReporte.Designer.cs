namespace HSis.UI
{
    partial class frmNuevoReporte
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
            this.lblDescripcion = new Label();
            this.rtbDescripcion = new RichTextBox();
            this.btnGuardar = new Button();
            this.btnCancelar = new Button();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(12, 12);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new Size(164, 25);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Nuevo Reporte";

            // lblDescripcion
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Location = new Point(12, 50);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new Size(140, 15);
            this.lblDescripcion.TabIndex = 1;
            this.lblDescripcion.Text = "Descripción del problema:";

            // rtbDescripcion
            this.rtbDescripcion.Location = new Point(12, 68);
            this.rtbDescripcion.Name = "rtbDescripcion";
            this.rtbDescripcion.Size = new Size(360, 150);
            this.rtbDescripcion.TabIndex = 2;
            this.rtbDescripcion.Text = "";

            // btnGuardar
            this.btnGuardar.BackColor = Color.FromArgb(46, 204, 113);
            this.btnGuardar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnGuardar.ForeColor = Color.White;
            this.btnGuardar.Location = new Point(216, 230);
            this.btnGuardar.Name = "btnGuardar";
            this.btnGuardar.Size = new Size(75, 30);
            this.btnGuardar.TabIndex = 3;
            this.btnGuardar.Text = "Guardar";
            this.btnGuardar.UseVisualStyleBackColor = false;
            this.btnGuardar.Click += new EventHandler(this.btnGuardar_Click);

            // btnCancelar
            this.btnCancelar.BackColor = Color.FromArgb(231, 76, 60);
            this.btnCancelar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnCancelar.ForeColor = Color.White;
            this.btnCancelar.Location = new Point(297, 230);
            this.btnCancelar.Name = "btnCancelar";
            this.btnCancelar.Size = new Size(75, 30);
            this.btnCancelar.TabIndex = 4;
            this.btnCancelar.Text = "Cancelar";
            this.btnCancelar.UseVisualStyleBackColor = false;
            this.btnCancelar.Click += new EventHandler(this.btnCancelar_Click);

            // frmNuevoReporte
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(384, 271);
            this.Controls.Add(this.btnCancelar);
            this.Controls.Add(this.btnGuardar);
            this.Controls.Add(this.rtbDescripcion);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.lblTitulo);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmNuevoReporte";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Nuevo Reporte";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTitulo;
        private Label lblDescripcion;
        private RichTextBox rtbDescripcion;
        private Button btnGuardar;
        private Button btnCancelar;
    }
}
