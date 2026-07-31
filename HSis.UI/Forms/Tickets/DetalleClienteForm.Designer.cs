using System;
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI.Forms.Tickets
{
    partial class DetalleClienteForm
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
            lblFolio = new Label();
            lblFolioValor = new Label();
            lblFechaAlta = new Label();
            lblFechaAltaValor = new Label();
            lblEstatus = new Label();
            lblEstatusValor = new Label();
            lblTecnico = new Label();
            lblTecnicoValor = new Label();
            lblDescripcion = new Label();
            txtDescripcion = new TextBox();
            lblSolucion = new Label();
            txtSolucion = new TextBox();
            btnCerrar = new Button();
            lblFechaCierre = new Label();
            lblFechaCierreValor = new Label();
            grpFeedback = new GroupBox();
            lblEstrellas = new Label();
            lblStar1 = new Label();
            lblStar2 = new Label();
            lblStar3 = new Label();
            lblStar4 = new Label();
            lblStar5 = new Label();
            lblComentario = new Label();
            txtComentario = new TextBox();
            btnEnviar = new Button();
            lblResumen = new Label();
            lblComentarioLectura = new Label();
            grpFeedback.SuspendLayout();
            SuspendLayout();
            // 
            // lblFolio
            // 
            lblFolio.AutoSize = true;
            lblFolio.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblFolio.Location = new Point(12, 15);
            lblFolio.Name = "lblFolio";
            lblFolio.Size = new Size(47, 20);
            lblFolio.TabIndex = 0;
            lblFolio.Text = "Folio:";
            // 
            // lblFolioValor
            // 
            lblFolioValor.AutoSize = true;
            lblFolioValor.Font = new Font("Segoe UI", 11F);
            lblFolioValor.Location = new Point(60, 15);
            lblFolioValor.Name = "lblFolioValor";
            lblFolioValor.Size = new Size(36, 20);
            lblFolioValor.TabIndex = 1;
            lblFolioValor.Text = "N/A";
            // 
            // lblFechaAlta
            // 
            lblFechaAlta.AutoSize = true;
            lblFechaAlta.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblFechaAlta.Location = new Point(12, 40);
            lblFechaAlta.Name = "lblFechaAlta";
            lblFechaAlta.Size = new Size(86, 20);
            lblFechaAlta.TabIndex = 2;
            lblFechaAlta.Text = "Fecha Alta:";
            // 
            // lblFechaAltaValor
            // 
            lblFechaAltaValor.AutoSize = true;
            lblFechaAltaValor.Font = new Font("Segoe UI", 11F);
            lblFechaAltaValor.Location = new Point(101, 40);
            lblFechaAltaValor.Name = "lblFechaAltaValor";
            lblFechaAltaValor.Size = new Size(36, 20);
            lblFechaAltaValor.TabIndex = 3;
            lblFechaAltaValor.Text = "N/A";
            // 
            // lblEstatus
            // 
            lblEstatus.AutoSize = true;
            lblEstatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblEstatus.Location = new Point(12, 65);
            lblEstatus.Name = "lblEstatus";
            lblEstatus.Size = new Size(64, 20);
            lblEstatus.TabIndex = 4;
            lblEstatus.Text = "Estatus:";
            // 
            // lblEstatusValor
            // 
            lblEstatusValor.AutoSize = true;
            lblEstatusValor.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblEstatusValor.Location = new Point(71, 60);
            lblEstatusValor.Name = "lblEstatusValor";
            lblEstatusValor.Padding = new Padding(5);
            lblEstatusValor.Size = new Size(49, 30);
            lblEstatusValor.TabIndex = 5;
            lblEstatusValor.Text = "N/A";
            // 
            // lblTecnico
            // 
            lblTecnico.AutoSize = true;
            lblTecnico.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblTecnico.Location = new Point(12, 100);
            lblTecnico.Name = "lblTecnico";
            lblTecnico.Size = new Size(135, 20);
            lblTecnico.TabIndex = 6;
            lblTecnico.Text = "Técnico Asignado:";
            // 
            // lblTecnicoValor
            // 
            lblTecnicoValor.AutoSize = true;
            lblTecnicoValor.Font = new Font("Segoe UI", 11F);
            lblTecnicoValor.Location = new Point(152, 100);
            lblTecnicoValor.Name = "lblTecnicoValor";
            lblTecnicoValor.Size = new Size(36, 20);
            lblTecnicoValor.TabIndex = 7;
            lblTecnicoValor.Text = "N/A";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblDescripcion.Location = new Point(12, 125);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(94, 20);
            lblDescripcion.TabIndex = 8;
            lblDescripcion.Text = "Descripción:";
            // 
            // txtDescripcion
            // 
            txtDescripcion.BackColor = SystemColors.Control;
            txtDescripcion.Font = new Font("Segoe UI", 11F);
            txtDescripcion.Location = new Point(12, 145);
            txtDescripcion.Multiline = true;
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.ReadOnly = true;
            txtDescripcion.Size = new Size(496, 80);
            txtDescripcion.TabIndex = 9;
            // 
            // lblSolucion
            // 
            lblSolucion.AutoSize = true;
            lblSolucion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblSolucion.Location = new Point(12, 235);
            lblSolucion.Name = "lblSolucion";
            lblSolucion.Size = new Size(72, 20);
            lblSolucion.TabIndex = 10;
            lblSolucion.Text = "Solución:";
            // 
            // txtSolucion
            // 
            txtSolucion.BackColor = SystemColors.Control;
            txtSolucion.Font = new Font("Segoe UI", 11F);
            txtSolucion.Location = new Point(12, 255);
            txtSolucion.Multiline = true;
            txtSolucion.Name = "txtSolucion";
            txtSolucion.ReadOnly = true;
            txtSolucion.Size = new Size(496, 80);
            txtSolucion.TabIndex = 11;
            // 
            // btnCerrar
            // 
            btnCerrar.BackColor = Color.FromArgb(231, 76, 60);
            btnCerrar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCerrar.ForeColor = Color.White;
            btnCerrar.Location = new Point(413, 590);
            btnCerrar.Name = "btnCerrar";
            btnCerrar.Size = new Size(95, 33);
            btnCerrar.TabIndex = 12;
            btnCerrar.Text = "Cerrar";
            btnCerrar.UseVisualStyleBackColor = false;
            btnCerrar.Click += btnCerrar_Click;
            // 
            // lblFechaCierre
            // 
            lblFechaCierre.AutoSize = true;
            lblFechaCierre.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblFechaCierre.Location = new Point(220, 40);
            lblFechaCierre.Name = "lblFechaCierre";
            lblFechaCierre.Size = new Size(98, 20);
            lblFechaCierre.TabIndex = 13;
            lblFechaCierre.Text = "Fecha Cierre:";
            lblFechaCierre.Visible = false;
            // 
            // lblFechaCierreValor
            // 
            lblFechaCierreValor.AutoSize = true;
            lblFechaCierreValor.Font = new Font("Segoe UI", 11F);
            lblFechaCierreValor.Location = new Point(321, 40);
            lblFechaCierreValor.Name = "lblFechaCierreValor";
            lblFechaCierreValor.Size = new Size(36, 20);
            lblFechaCierreValor.TabIndex = 14;
            lblFechaCierreValor.Text = "N/A";
            lblFechaCierreValor.Visible = false;
            // 
            // grpFeedback
            // 
            grpFeedback.Controls.Add(lblEstrellas);
            grpFeedback.Controls.Add(lblStar1);
            grpFeedback.Controls.Add(lblStar2);
            grpFeedback.Controls.Add(lblStar3);
            grpFeedback.Controls.Add(lblStar4);
            grpFeedback.Controls.Add(lblStar5);
            grpFeedback.Controls.Add(lblComentario);
            grpFeedback.Controls.Add(txtComentario);
            grpFeedback.Controls.Add(btnEnviar);
            grpFeedback.Controls.Add(lblResumen);
            grpFeedback.Controls.Add(lblComentarioLectura);
            grpFeedback.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpFeedback.Location = new Point(12, 350);
            grpFeedback.Name = "grpFeedback";
            grpFeedback.Size = new Size(496, 120);
            grpFeedback.TabIndex = 0;
            grpFeedback.TabStop = false;
            grpFeedback.Text = "Retroalimentación de la Atención";
            grpFeedback.Visible = false;
            // 
            // lblEstrellas
            // 
            lblEstrellas.AutoSize = true;
            lblEstrellas.Font = new Font("Segoe UI", 9F);
            lblEstrellas.Location = new Point(15, 23);
            lblEstrellas.Name = "lblEstrellas";
            lblEstrellas.Size = new Size(110, 15);
            lblEstrellas.TabIndex = 0;
            lblEstrellas.Text = "Calificación (1 al 5):";
            // 
            // lblStar1
            // 
            lblStar1.Cursor = Cursors.Hand;
            lblStar1.Font = new Font("Segoe UI", 16F);
            lblStar1.ForeColor = Color.Gray;
            lblStar1.Location = new Point(140, 18);
            lblStar1.Name = "lblStar1";
            lblStar1.Size = new Size(30, 30);
            lblStar1.TabIndex = 1;
            lblStar1.Tag = "1";
            lblStar1.Text = "☆";
            lblStar1.Click += lblStar_Click;
            lblStar1.MouseEnter += lblStar_MouseEnter;
            lblStar1.MouseLeave += lblStar_MouseLeave;
            // 
            // lblStar2
            // 
            lblStar2.Cursor = Cursors.Hand;
            lblStar2.Font = new Font("Segoe UI", 16F);
            lblStar2.ForeColor = Color.Gray;
            lblStar2.Location = new Point(175, 18);
            lblStar2.Name = "lblStar2";
            lblStar2.Size = new Size(30, 30);
            lblStar2.TabIndex = 2;
            lblStar2.Tag = "2";
            lblStar2.Text = "☆";
            lblStar2.Click += lblStar_Click;
            lblStar2.MouseEnter += lblStar_MouseEnter;
            lblStar2.MouseLeave += lblStar_MouseLeave;
            // 
            // lblStar3
            // 
            lblStar3.Cursor = Cursors.Hand;
            lblStar3.Font = new Font("Segoe UI", 16F);
            lblStar3.ForeColor = Color.Gray;
            lblStar3.Location = new Point(210, 18);
            lblStar3.Name = "lblStar3";
            lblStar3.Size = new Size(30, 30);
            lblStar3.TabIndex = 3;
            lblStar3.Tag = "3";
            lblStar3.Text = "☆";
            lblStar3.Click += lblStar_Click;
            lblStar3.MouseEnter += lblStar_MouseEnter;
            lblStar3.MouseLeave += lblStar_MouseLeave;
            // 
            // lblStar4
            // 
            lblStar4.Cursor = Cursors.Hand;
            lblStar4.Font = new Font("Segoe UI", 16F);
            lblStar4.ForeColor = Color.Gray;
            lblStar4.Location = new Point(245, 18);
            lblStar4.Name = "lblStar4";
            lblStar4.Size = new Size(30, 30);
            lblStar4.TabIndex = 4;
            lblStar4.Tag = "4";
            lblStar4.Text = "☆";
            lblStar4.Click += lblStar_Click;
            lblStar4.MouseEnter += lblStar_MouseEnter;
            lblStar4.MouseLeave += lblStar_MouseLeave;
            // 
            // lblStar5
            // 
            lblStar5.Cursor = Cursors.Hand;
            lblStar5.Font = new Font("Segoe UI", 16F);
            lblStar5.ForeColor = Color.Gray;
            lblStar5.Location = new Point(280, 18);
            lblStar5.Name = "lblStar5";
            lblStar5.Size = new Size(30, 30);
            lblStar5.TabIndex = 5;
            lblStar5.Tag = "5";
            lblStar5.Text = "☆";
            lblStar5.Click += lblStar_Click;
            lblStar5.MouseEnter += lblStar_MouseEnter;
            lblStar5.MouseLeave += lblStar_MouseLeave;
            // 
            // lblComentario
            // 
            lblComentario.AutoSize = true;
            lblComentario.Font = new Font("Segoe UI", 9F);
            lblComentario.Location = new Point(15, 50);
            lblComentario.Name = "lblComentario";
            lblComentario.Size = new Size(132, 15);
            lblComentario.TabIndex = 6;
            lblComentario.Text = "Comentario (Opcional):";
            // 
            // txtComentario
            // 
            txtComentario.Font = new Font("Segoe UI", 9F);
            txtComentario.Location = new Point(15, 70);
            txtComentario.Name = "txtComentario";
            txtComentario.Size = new Size(330, 23);
            txtComentario.TabIndex = 7;
            // 
            // btnEnviar
            // 
            btnEnviar.BackColor = Color.FromArgb(52, 152, 219);
            btnEnviar.FlatAppearance.BorderSize = 0;
            btnEnviar.FlatStyle = FlatStyle.Flat;
            btnEnviar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEnviar.ForeColor = Color.White;
            btnEnviar.Location = new Point(365, 68);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new Size(115, 30);
            btnEnviar.TabIndex = 8;
            btnEnviar.Text = "Enviar";
            btnEnviar.UseVisualStyleBackColor = false;
            btnEnviar.Click += btnEnviarFeedback_Click;
            // 
            // lblResumen
            // 
            lblResumen.AutoSize = true;
            lblResumen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblResumen.ForeColor = Color.FromArgb(41, 128, 185);
            lblResumen.Location = new Point(15, 25);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(0, 19);
            lblResumen.TabIndex = 9;
            // 
            // lblComentarioLectura
            // 
            lblComentarioLectura.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblComentarioLectura.Location = new Point(15, 50);
            lblComentarioLectura.Name = "lblComentarioLectura";
            lblComentarioLectura.Size = new Size(430, 50);
            lblComentarioLectura.TabIndex = 10;
            // 
            // DetalleClienteForm
            // 
            AutoSize = true;
            AutoSizeMode = AutoSizeMode.GrowAndShrink;
            ClientSize = new Size(559, 482);
            Controls.Add(grpFeedback);
            Controls.Add(lblFechaCierre);
            Controls.Add(lblFechaCierreValor);
            Controls.Add(btnCerrar);
            Controls.Add(txtSolucion);
            Controls.Add(lblSolucion);
            Controls.Add(txtDescripcion);
            Controls.Add(lblDescripcion);
            Controls.Add(lblTecnicoValor);
            Controls.Add(lblTecnico);
            Controls.Add(lblEstatusValor);
            Controls.Add(lblEstatus);
            Controls.Add(lblFechaAltaValor);
            Controls.Add(lblFechaAlta);
            Controls.Add(lblFolioValor);
            Controls.Add(lblFolio);
            Font = new Font("Segoe UI", 11F);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "DetalleClienteForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Detalle del Ticket - Solo Lectura";
            Load += frmDetalleCliente_Load;
            grpFeedback.ResumeLayout(false);
            grpFeedback.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
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
