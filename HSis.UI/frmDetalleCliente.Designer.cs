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
            this.lblFechaCierre = new System.Windows.Forms.Label();
            this.lblFechaCierreValor = new System.Windows.Forms.Label();
            this.grpFeedback = new System.Windows.Forms.GroupBox();
            this.lblEstrellas = new System.Windows.Forms.Label();
            this.lblStar1 = new System.Windows.Forms.Label();
            this.lblStar2 = new System.Windows.Forms.Label();
            this.lblStar3 = new System.Windows.Forms.Label();
            this.lblStar4 = new System.Windows.Forms.Label();
            this.lblStar5 = new System.Windows.Forms.Label();
            this.lblComentario = new System.Windows.Forms.Label();
            this.txtComentario = new System.Windows.Forms.TextBox();
            this.btnEnviar = new System.Windows.Forms.Button();
            this.lblResumen = new System.Windows.Forms.Label();
            this.lblComentarioLectura = new System.Windows.Forms.Label();
            this.SuspendLayout();

            // lblFolio
            this.lblFolio.AutoSize = true;
            this.lblFolio.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFolio.Location = new System.Drawing.Point(12, 15);
            this.lblFolio.Name = "lblFolio";
            this.lblFolio.Size = new System.Drawing.Size(41, 15);
            this.lblFolio.TabIndex = 0;
            this.lblFolio.Text = "Folio:";

            // lblFolioValor
            this.lblFolioValor.AutoSize = true;
            this.lblFolioValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFolioValor.Location = new System.Drawing.Point(60, 15);
            this.lblFolioValor.Name = "lblFolioValor";
            this.lblFolioValor.Size = new System.Drawing.Size(23, 15);
            this.lblFolioValor.TabIndex = 1;
            this.lblFolioValor.Text = "N/A";

            // lblFechaAlta
            this.lblFechaAlta.AutoSize = true;
            this.lblFechaAlta.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFechaAlta.Location = new System.Drawing.Point(12, 40);
            this.lblFechaAlta.Name = "lblFechaAlta";
            this.lblFechaAlta.Size = new System.Drawing.Size(66, 15);
            this.lblFechaAlta.TabIndex = 2;
            this.lblFechaAlta.Text = "Fecha Alta:";

            // lblFechaAltaValor
            this.lblFechaAltaValor.AutoSize = true;
            this.lblFechaAltaValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFechaAltaValor.Location = new System.Drawing.Point(85, 40);
            this.lblFechaAltaValor.Name = "lblFechaAltaValor";
            this.lblFechaAltaValor.Size = new System.Drawing.Size(23, 15);
            this.lblFechaAltaValor.TabIndex = 3;
            this.lblFechaAltaValor.Text = "N/A";

            // lblEstatus
            this.lblEstatus.AutoSize = true;
            this.lblEstatus.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEstatus.Location = new System.Drawing.Point(12, 65);
            this.lblEstatus.Name = "lblEstatus";
            this.lblEstatus.Size = new System.Drawing.Size(52, 15);
            this.lblEstatus.TabIndex = 4;
            this.lblEstatus.Text = "Estatus:";

            // lblEstatusValor
            this.lblEstatusValor.AutoSize = true;
            this.lblEstatusValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblEstatusValor.Location = new System.Drawing.Point(71, 65);
            this.lblEstatusValor.Name = "lblEstatusValor";
            this.lblEstatusValor.Padding = new System.Windows.Forms.Padding(5);
            this.lblEstatusValor.Size = new System.Drawing.Size(35, 25);
            this.lblEstatusValor.TabIndex = 5;
            this.lblEstatusValor.Text = "N/A";

            // lblTecnico
            this.lblTecnico.AutoSize = true;
            this.lblTecnico.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblTecnico.Location = new System.Drawing.Point(12, 100);
            this.lblTecnico.Name = "lblTecnico";
            this.lblTecnico.Size = new System.Drawing.Size(96, 15);
            this.lblTecnico.TabIndex = 6;
            this.lblTecnico.Text = "Técnico Asignado:";

            // lblTecnicoValor
            this.lblTecnicoValor.AutoSize = true;
            this.lblTecnicoValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblTecnicoValor.Location = new System.Drawing.Point(115, 100);
            this.lblTecnicoValor.Name = "lblTecnicoValor";
            this.lblTecnicoValor.Size = new System.Drawing.Size(23, 15);
            this.lblTecnicoValor.TabIndex = 7;
            this.lblTecnicoValor.Text = "N/A";

            // lblDescripcion
            this.lblDescripcion.AutoSize = true;
            this.lblDescripcion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblDescripcion.Location = new System.Drawing.Point(12, 125);
            this.lblDescripcion.Name = "lblDescripcion";
            this.lblDescripcion.Size = new System.Drawing.Size(75, 15);
            this.lblDescripcion.TabIndex = 8;
            this.lblDescripcion.Text = "Descripción:";

            // txtDescripcion
            this.txtDescripcion.BackColor = System.Drawing.SystemColors.Control;
            this.txtDescripcion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtDescripcion.Location = new System.Drawing.Point(12, 145);
            this.txtDescripcion.Multiline = true;
            this.txtDescripcion.Name = "txtDescripcion";
            this.txtDescripcion.ReadOnly = true;
            this.txtDescripcion.Size = new System.Drawing.Size(496, 80);
            this.txtDescripcion.TabIndex = 9;

            // lblSolucion
            this.lblSolucion.AutoSize = true;
            this.lblSolucion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblSolucion.Location = new System.Drawing.Point(12, 235);
            this.lblSolucion.Name = "lblSolucion";
            this.lblSolucion.Size = new System.Drawing.Size(56, 15);
            this.lblSolucion.TabIndex = 10;
            this.lblSolucion.Text = "Solución:";

            // txtSolucion
            this.txtSolucion.BackColor = System.Drawing.SystemColors.Control;
            this.txtSolucion.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.txtSolucion.Location = new System.Drawing.Point(12, 255);
            this.txtSolucion.Multiline = true;
            this.txtSolucion.Name = "txtSolucion";
            this.txtSolucion.ReadOnly = true;
            this.txtSolucion.Size = new System.Drawing.Size(496, 80);
            this.txtSolucion.TabIndex = 11;

            // btnCerrar
            this.btnCerrar.BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
            this.btnCerrar.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.btnCerrar.ForeColor = System.Drawing.Color.White;
            this.btnCerrar.Location = new System.Drawing.Point(413, 590);
            this.btnCerrar.Name = "btnCerrar";
            this.btnCerrar.Size = new System.Drawing.Size(95, 33);
            this.btnCerrar.TabIndex = 12;
            this.btnCerrar.Text = "Cerrar";
            this.btnCerrar.UseVisualStyleBackColor = false;
            this.btnCerrar.Click += new System.EventHandler(this.btnCerrar_Click);

            // lblFechaCierre
            this.lblFechaCierre.AutoSize = true;
            this.lblFechaCierre.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.lblFechaCierre.Location = new System.Drawing.Point(220, 40);
            this.lblFechaCierre.Name = "lblFechaCierre";
            this.lblFechaCierre.Size = new System.Drawing.Size(80, 15);
            this.lblFechaCierre.TabIndex = 13;
            this.lblFechaCierre.Text = "Fecha Cierre:";
            this.lblFechaCierre.Visible = false;

            // lblFechaCierreValor
            this.lblFechaCierreValor.AutoSize = true;
            this.lblFechaCierreValor.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.lblFechaCierreValor.Location = new System.Drawing.Point(310, 40);
            this.lblFechaCierreValor.Name = "lblFechaCierreValor";
            this.lblFechaCierreValor.Size = new System.Drawing.Size(120, 15);
            this.lblFechaCierreValor.TabIndex = 14;
            this.lblFechaCierreValor.Text = "N/A";
            this.lblFechaCierreValor.Visible = false;
            // 
            // grpFeedback
            // 
            this.grpFeedback.Text = "Retroalimentación de la Atención";
            this.grpFeedback.Location = new System.Drawing.Point(12, 350);
            this.grpFeedback.Size = new System.Drawing.Size(496, 120);
            this.grpFeedback.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.grpFeedback.Visible = false;
            this.grpFeedback.Controls.Add(this.lblEstrellas);
            this.grpFeedback.Controls.Add(this.lblStar1);
            this.grpFeedback.Controls.Add(this.lblStar2);
            this.grpFeedback.Controls.Add(this.lblStar3);
            this.grpFeedback.Controls.Add(this.lblStar4);
            this.grpFeedback.Controls.Add(this.lblStar5);
            this.grpFeedback.Controls.Add(this.lblComentario);
            this.grpFeedback.Controls.Add(this.txtComentario);
            this.grpFeedback.Controls.Add(this.btnEnviar);
            this.grpFeedback.Controls.Add(this.lblResumen);
            this.grpFeedback.Controls.Add(this.lblComentarioLectura);
            // 
            // lblEstrellas
            // 
            this.lblEstrellas.Text = "Calificación (1 al 5):";
            this.lblEstrellas.Location = new System.Drawing.Point(15, 23);
            this.lblEstrellas.AutoSize = true;
            this.lblEstrellas.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            // 
            // lblStar1
            // 
            this.lblStar1.Text = "☆";
            this.lblStar1.Location = new System.Drawing.Point(140, 18);
            this.lblStar1.Size = new System.Drawing.Size(30, 30);
            this.lblStar1.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular);
            this.lblStar1.ForeColor = System.Drawing.Color.Gray;
            this.lblStar1.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStar1.Tag = "1";
            this.lblStar1.Click += new System.EventHandler(this.lblStar_Click);
            this.lblStar1.MouseEnter += new System.EventHandler(this.lblStar_MouseEnter);
            this.lblStar1.MouseLeave += new System.EventHandler(this.lblStar_MouseLeave);
            // 
            // lblStar2
            // 
            this.lblStar2.Text = "☆";
            this.lblStar2.Location = new System.Drawing.Point(175, 18);
            this.lblStar2.Size = new System.Drawing.Size(30, 30);
            this.lblStar2.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular);
            this.lblStar2.ForeColor = System.Drawing.Color.Gray;
            this.lblStar2.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStar2.Tag = "2";
            this.lblStar2.Click += new System.EventHandler(this.lblStar_Click);
            this.lblStar2.MouseEnter += new System.EventHandler(this.lblStar_MouseEnter);
            this.lblStar2.MouseLeave += new System.EventHandler(this.lblStar_MouseLeave);
            // 
            // lblStar3
            // 
            this.lblStar3.Text = "☆";
            this.lblStar3.Location = new System.Drawing.Point(210, 18);
            this.lblStar3.Size = new System.Drawing.Size(30, 30);
            this.lblStar3.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular);
            this.lblStar3.ForeColor = System.Drawing.Color.Gray;
            this.lblStar3.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStar3.Tag = "3";
            this.lblStar3.Click += new System.EventHandler(this.lblStar_Click);
            this.lblStar3.MouseEnter += new System.EventHandler(this.lblStar_MouseEnter);
            this.lblStar3.MouseLeave += new System.EventHandler(this.lblStar_MouseLeave);
            // 
            // lblStar4
            // 
            this.lblStar4.Text = "☆";
            this.lblStar4.Location = new System.Drawing.Point(245, 18);
            this.lblStar4.Size = new System.Drawing.Size(30, 30);
            this.lblStar4.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular);
            this.lblStar4.ForeColor = System.Drawing.Color.Gray;
            this.lblStar4.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStar4.Tag = "4";
            this.lblStar4.Click += new System.EventHandler(this.lblStar_Click);
            this.lblStar4.MouseEnter += new System.EventHandler(this.lblStar_MouseEnter);
            this.lblStar4.MouseLeave += new System.EventHandler(this.lblStar_MouseLeave);
            // 
            // lblStar5
            // 
            this.lblStar5.Text = "☆";
            this.lblStar5.Location = new System.Drawing.Point(280, 18);
            this.lblStar5.Size = new System.Drawing.Size(30, 30);
            this.lblStar5.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Regular);
            this.lblStar5.ForeColor = System.Drawing.Color.Gray;
            this.lblStar5.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblStar5.Tag = "5";
            this.lblStar5.Click += new System.EventHandler(this.lblStar_Click);
            this.lblStar5.MouseEnter += new System.EventHandler(this.lblStar_MouseEnter);
            this.lblStar5.MouseLeave += new System.EventHandler(this.lblStar_MouseLeave);
            // 
            // lblComentario
            // 
            this.lblComentario.Text = "Comentario (Opcional):";
            this.lblComentario.Location = new System.Drawing.Point(15, 50);
            this.lblComentario.AutoSize = true;
            this.lblComentario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            // 
            // txtComentario
            // 
            this.txtComentario.Location = new System.Drawing.Point(15, 70);
            this.txtComentario.Width = 330;
            this.txtComentario.Height = 25;
            this.txtComentario.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular);
            // 
            // btnEnviar
            // 
            this.btnEnviar.Text = "Enviar";
            this.btnEnviar.Location = new System.Drawing.Point(365, 68);
            this.btnEnviar.Width = 115;
            this.btnEnviar.Height = 30;
            this.btnEnviar.BackColor = System.Drawing.Color.FromArgb(52, 152, 219);
            this.btnEnviar.ForeColor = System.Drawing.Color.White;
            this.btnEnviar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnEnviar.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnEnviar.FlatAppearance.BorderSize = 0;
            this.btnEnviar.Click += new System.EventHandler(this.btnEnviarFeedback_Click);
            // 
            // lblResumen
            // 
            this.lblResumen.Location = new System.Drawing.Point(15, 25);
            this.lblResumen.AutoSize = true;
            this.lblResumen.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.lblResumen.ForeColor = System.Drawing.Color.FromArgb(41, 128, 185);
            // 
            // lblComentarioLectura
            // 
            this.lblComentarioLectura.Location = new System.Drawing.Point(15, 50);
            this.lblComentarioLectura.Width = 430;
            this.lblComentarioLectura.Height = 50;
            this.lblComentarioLectura.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Italic);

            // frmDetalleCliente
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.grpFeedback);
            this.Controls.Add(this.lblFechaCierre);
            this.Controls.Add(this.lblFechaCierreValor);
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
            this.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
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
        private System.Windows.Forms.Label lblFechaCierre;
        private System.Windows.Forms.Label lblFechaCierreValor;
        private System.Windows.Forms.GroupBox grpFeedback;
        private System.Windows.Forms.Label lblEstrellas;
        private System.Windows.Forms.Label lblStar1;
        private System.Windows.Forms.Label lblStar2;
        private System.Windows.Forms.Label lblStar3;
        private System.Windows.Forms.Label lblStar4;
        private System.Windows.Forms.Label lblStar5;
        private System.Windows.Forms.Label lblComentario;
        private System.Windows.Forms.TextBox txtComentario;
        private System.Windows.Forms.Button btnEnviar;
        private System.Windows.Forms.Label lblResumen;
        private System.Windows.Forms.Label lblComentarioLectura;
    }
}
