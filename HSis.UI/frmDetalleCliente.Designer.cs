namespace HSis.UI
{
    partial class frmDetalleCliente
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblFolio = new System.Windows.Forms.Label();
            this.lblFolioValor = new System.Windows.Forms.Label();
            this.lblFechaAlta = new System.Windows.Forms.Label();
            this.lblFechaAltaValor = new System.Windows.Forms.Label();
            this.lblEstatus = new System.Windows.Forms.Label();
            this.lblEstatusValor = new System.Windows.Forms.Label();
            this.lblTecnico = new System.Windows.Forms.Label();
            this.lblTecnicoValor = new System.Windows.Forms.Label();
            this.lblDescripcion = new System.Windows.Forms.Label();
            this.txtDescripcion = new System.Windows.Forms.TextBox();
            this.lblSolucion = new System.Windows.Forms.Label();
            this.txtSolucion = new System.Windows.Forms.TextBox();
            this.btnCerrar = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblFolio
            this.lblFolio.AutoSize = true;
            this.lblFolio.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFolio.Location = new System.Drawing.Point(12, 15);
            this.lblFolio.Name = "lblFolio";
            this.lblFolio.Size = new System.Drawing.Size(41, 15);
            this.lblFolio.TabIndex = 0;
            this.lblFolio.Text = "Folio:";

            // lblFolioValor
            this.lblFolioValor.AutoSize = true;
            this.lblFolioValor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFolioValor.Location = new System.Drawing.Point(60, 15);
            this.lblFolioValor.Name = "lblFolioValor";
            this.lblFolioValor.Size = new System.Drawing.Size(23, 15);
            this.lblFolioValor.TabIndex = 1;
            this.lblFolioValor.Text = "N/A";

            // lblFechaAlta
            this.lblFechaAlta.AutoSize = true;
            this.lblFechaAlta.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFechaAlta.Location = new System.Drawing.Point(12, 40);
            this.lblFechaAlta.Name = "lblFechaAlta";
            this.lblFechaAlta.Size = new System.Drawing.Size(66, 15);
            this.lblFechaAlta.TabIndex = 2;
            this.lblFechaAlta.Text = "Fecha Alta:";

            // lblFechaAltaValor
            this.lblFechaAltaValor.AutoSize = true;
            this.lblFechaAltaValor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFechaAltaValor.Location = new System.Drawing.Point(85, 40);
            this.lblFechaAltaValor.Name = "lblFechaAltaValor";
            this.lblFechaAltaValor.Size = new System.Drawing.Size(23, 15);
            this.lblFechaAltaValor.TabIndex = 3;
            this.lblFechaAltaValor.Text = "N/A";

            // lblEstatus
            this.lblEstatus.AutoSize = true;
            this.lblEstatus.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEstatus.Location = new System.Drawing.Point(12, 65);
            this.lblEstatus.Name = "lblEstatus";
            this.lblEstatus.Size = new System.Drawing.Size(52, 15);
            this.lblEstatus.TabIndex = 4;
            this.lblEstatus.Text = "Estatus:";

            // lblEstatusValor
            this.lblEstatusValor.AutoSize = true;
            this.lblEstatusValor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEstatusValor.Location = new System.Drawing.Point(71, 65);
            this.lblEstatusValor.Name = "lblEstatusValor";
            this.lblEstatusValor.Padding = new System.Windows.Forms.Padding(5);
            this.lblEstatusValor.Size = new System.Drawing.Size(35, 25);
            this.lblEstatusValor.TabIndex = 5;
            this.lblEstatusValor.Text = "N/A";

            // lblTecnico
            this.lblTecnico.AutoSize = true;
            this.lblTecnico.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTecnico.Location = new System.Drawing.Point(12, 100);
            this.lblTecnico.Name = "lblTecnico";
            this.lblTecnico.Size = new System.Drawing.Size(96, 15);
            this.lblTecnico.TabIndex = 6;
            this.lblTecnico.Text = "Técnico Asignado:";

            // lblTecnicoValor
            this.lblTecnicoValor.AutoSize = true;
            this.lblTecnicoValor.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTecnicoValor.Location = new System.Drawing.Point(115, 100);
            this.lblTecnicoValor.Name = "lblTecnicoValor";
            this.lblTecnicoValor.Size = new System.Drawing.Size(23, 15);
            this.lblTecnicoValor.TabIndex = 7;
            this.lblTecnicoValor.Text = "N/A";

            // lblDescripcion
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDescripcion.Location = new System.Drawing.Point(12, 125);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(75, 15);
            this.lblDescripcion.TabIndex = 8;
            this.lblDescripcion.Text = "Descripción:";

            // txtDescripcion
            this.txtDescripcion.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtDescripcion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtDescripcion.Location = new System.Drawing.Point(12, 145);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.ReadOnly = true;
            this.txtDescripcion.Size = new System.Drawing.Size(460, 80);
            this.txtDescripcion.TabIndex = 9;

            // lblSolucion
            this.lblSolucion.AutoSize = true;
            this.lblSolucion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSolucion.Location = new System.Drawing.Point(12, 235);
            this.lblSolucion.Name = "lblSolucion";
            this.lblSolucion.Size = new System.Drawing.Size(56, 15);
            this.lblSolucion.TabIndex = 10;
            this.lblSolucion.Text = "Solución:";

            // txtSolucion
            this.txtSolucion.BackColor = System.Drawing.SystemColors.ControlLight;
            this.txtSolucion.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSolucion.Location = new System.Drawing.Point(12, 255);
            this.txtSolucion.Multiline = true;
            this.txtSolucion.Name = "txtSolucion";
            this.txtSolucion.ReadOnly = true;
            this.txtSolucion.Size = new System.Drawing.Size(460, 80);
            this.txtSolucion.TabIndex = 11;

            // btnCerrar
            this.btnCerrar.BackColor = System.Drawing.Color.Gray;
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(397, 350);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(75, 30);
            this.btnCerrar.TabIndex = 12;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            // frmDetalleCliente
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 391);
            this.Controls.Add(this.btnCerrar);
            this.Controls.Add(this.txtSolucion);
            this.Controls.Add(this.lblSolucion);
            this.Controls.Add(this.txtDescripcion);
            this.Controls.Add(this.lblDescripcion);
            this.Controls.Add(this.lblTecnicoValor);
            this.Controls.Add(this.lblTecnico);
            this.Controls.Add(this.lblEstatusValor);
            this.Controls.Add(this.lblEstatus);
            this.Controls.Add(this.lblFechaAltaValor);
            this.Controls.Add(this.lblFechaAlta);
            this.Controls.Add(this.lblFolioValor);
            this.Controls.Add(this.lblFolio);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmDetalleCliente";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Detalle del Ticket - Solo Lectura";
            this.Load += new System.EventHandler(this.frmDetalleCliente_Load);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblFolio;
        private System.Windows.Forms.Label lblFolioValor;
        private System.Windows.Forms.Label lblFechaAlta;
        private System.Windows.Forms.Label lblFechaAltaValor;
        private System.Windows.Forms.Label lblEstatus;
        private System.Windows.Forms.Label lblEstatusValor;
        private System.Windows.Forms.Label lblTecnico;
        private System.Windows.Forms.Label lblTecnicoValor;
        private System.Windows.Forms.Label lblDescripcion;
        private System.Windows.Forms.TextBox txtDescripcion;
        private System.Windows.Forms.Label lblSolucion;
        private System.Windows.Forms.TextBox txtSolucion;
        private System.Windows.Forms.Button btnCerrar;
    }
}
