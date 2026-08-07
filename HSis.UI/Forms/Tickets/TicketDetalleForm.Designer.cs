using System;
using System.Drawing;
using System.Windows.Forms;
using HSis.UI.Controls;

namespace HSis.UI.Forms.Tickets
{
    partial class TicketDetalleForm
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
            cmbEstatus = new ComboBox();
            lblUsuario = new Label();
            lblEstatus = new Label();
            txtUsuario = new TextBox();
            lblAlta = new Label();
            lblDescripcion = new Label();
            txtAlta = new TextBox();
            lblSolucion = new Label();
            lblAtendido = new Label();
            lblAtencion = new Label();
            txtAtencion = new TextBox();
            lblCierre = new Label();
            txtCierre = new TextBox();
            cmbAtendido = new ComboBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            dgvHistorial = new DataGridView();
            lblPrioridad = new Label();
            cmbPrioridad = new ComboBox();
            lblDepartamento = new Label();
            txtDepartamento = new TextBox();
            grpFeedback = new GroupBox();
            lblEstrellas = new Label();
            cmbEstrellas = new ComboBox();
            lblComentario = new Label();
            txtComentario = new TextBox();
            btnEnviar = new Button();
            lblResumen = new Label();
            lblComentarioLectura = new Label();
            tabControlTicket = new TabControl();
            tabInfoGeneral = new TabPage();
            tabDescripcionSolucion = new TabPage();
            tbpHistorial = new TabPage();
            tbpFeedback = new TabPage();
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).BeginInit();
            grpFeedback.SuspendLayout();
            tabControlTicket.SuspendLayout();
            tabInfoGeneral.SuspendLayout();
            tabDescripcionSolucion.SuspendLayout();
            tbpHistorial.SuspendLayout();
            tbpFeedback.SuspendLayout();
            SuspendLayout();
            // 
            // lblFolio
            // 
            lblFolio.AutoSize = true;
            lblFolio.Location = new Point(12, 15);
            lblFolio.Name = "lblFolio";
            lblFolio.Size = new Size(39, 15);
            lblFolio.TabIndex = 0;
            lblFolio.Text = "Folio: ";
            // 
            // cmbEstatus
            // 
            cmbEstatus.FormattingEnabled = true;
            cmbEstatus.Items.AddRange(new object[] { "Abierto", "En proceso", "Cerrado", "Reabierto", "Abierto", "En proceso", "Cerrado", "Reabierto" });
            cmbEstatus.Location = new Point(134, 93);
            cmbEstatus.Name = "cmbEstatus";
            cmbEstatus.Size = new Size(228, 23);
            cmbEstatus.TabIndex = 2;
            cmbEstatus.SelectedIndexChanged += CmbEstatus_SelectedIndexChanged;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Location = new Point(12, 43);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(47, 15);
            lblUsuario.TabIndex = 3;
            lblUsuario.Text = "Usuario";
            // 
            // lblEstatus
            // 
            lblEstatus.AutoSize = true;
            lblEstatus.Location = new Point(12, 101);
            lblEstatus.Name = "lblEstatus";
            lblEstatus.Size = new Size(44, 15);
            lblEstatus.TabIndex = 4;
            lblEstatus.Text = "Estatus";
            // 
            // txtUsuario
            // 
            txtUsuario.Enabled = false;
            txtUsuario.Location = new Point(134, 35);
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(228, 23);
            txtUsuario.TabIndex = 7;
            // 
            // lblAlta
            // 
            lblAlta.AutoSize = true;
            lblAlta.Location = new Point(12, 157);
            lblAlta.Name = "lblAlta";
            lblAlta.Size = new Size(65, 15);
            lblAlta.TabIndex = 9;
            lblAlta.Text = "Fecha Alta:";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(12, 15);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(69, 15);
            lblDescripcion.TabIndex = 1;
            lblDescripcion.Text = "Descripción";
            // 
            // txtAlta
            // 
            txtAlta.Location = new Point(134, 151);
            txtAlta.Name = "txtAlta";
            txtAlta.Size = new Size(343, 23);
            txtAlta.TabIndex = 10;
            txtAlta.ReadOnly = true;
            // 
            // lblSolucion
            // 
            lblSolucion.AutoSize = true;
            lblSolucion.Location = new Point(12, 230);
            lblSolucion.Name = "lblSolucion";
            lblSolucion.Size = new Size(53, 15);
            lblSolucion.TabIndex = 11;
            lblSolucion.Text = "Solución";
            // 
            // lblAtendido
            // 
            lblAtendido.AutoSize = true;
            lblAtendido.Location = new Point(12, 241);
            lblAtendido.Name = "lblAtendido";
            lblAtendido.Size = new Size(80, 15);
            lblAtendido.TabIndex = 24;
            lblAtendido.Text = "Atendido por:";
            // 
            // lblAtencion
            // 
            lblAtencion.AutoSize = true;
            lblAtencion.Location = new Point(12, 188);
            lblAtencion.Name = "lblAtencion";
            lblAtencion.Size = new Size(92, 15);
            lblAtencion.TabIndex = 14;
            lblAtencion.Text = "Fecha Atención:";
            // 
            // txtAtencion
            // 
            txtAtencion.Location = new Point(134, 180);
            txtAtencion.Name = "txtAtencion";
            txtAtencion.Size = new Size(343, 23);
            txtAtencion.TabIndex = 15;
            txtAtencion.ReadOnly = true;
            // 
            // lblCierre
            // 
            lblCierre.AutoSize = true;
            lblCierre.Location = new Point(12, 217);
            lblCierre.Name = "lblCierre";
            lblCierre.Size = new Size(75, 15);
            lblCierre.TabIndex = 16;
            lblCierre.Text = "Fecha Cierra:";
            // 
            // txtCierre
            // 
            txtCierre.Location = new Point(134, 209);
            txtCierre.Name = "txtCierre";
            txtCierre.Size = new Size(343, 23);
            txtCierre.TabIndex = 17;
            txtCierre.ReadOnly = true;
            // 
            // cmbAtendido
            // 
            cmbAtendido.Location = new Point(134, 238);
            cmbAtendido.Name = "cmbAtendido";
            cmbAtendido.Size = new Size(121, 23);
            cmbAtendido.TabIndex = 23;
            // 
            // btnGuardar
            // 
            btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnGuardar.BackColor = Color.FromArgb(39, 174, 96);
            btnGuardar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(504, 490);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(95, 30);
            btnGuardar.TabIndex = 19;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            btnCancelar.BackColor = Color.FromArgb(231, 76, 60);
            btnCancelar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(609, 490);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(95, 30);
            btnCancelar.TabIndex = 20;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = false;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // dgvHistorial
            // 
            dgvHistorial.AllowUserToAddRows = false;
            dgvHistorial.AllowUserToDeleteRows = false;
            dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvHistorial.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvHistorial.Dock = DockStyle.Fill;
            dgvHistorial.Location = new Point(3, 3);
            dgvHistorial.Name = "dgvHistorial";
            dgvHistorial.ReadOnly = true;
            dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorial.Size = new Size(693, 441);
            dgvHistorial.TabIndex = 21;
            // 
            // lblPrioridad
            // 
            lblPrioridad.AutoSize = true;
            lblPrioridad.Location = new Point(12, 130);
            lblPrioridad.Name = "lblPrioridad";
            lblPrioridad.Size = new Size(58, 15);
            lblPrioridad.TabIndex = 25;
            lblPrioridad.Text = "Prioridad:";
            // 
            // cmbPrioridad
            // 
            cmbPrioridad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPrioridad.FormattingEnabled = true;
            cmbPrioridad.Items.AddRange(new object[] { "Alta", "Media", "Baja" });
            cmbPrioridad.Location = new Point(134, 122);
            cmbPrioridad.Name = "cmbPrioridad";
            cmbPrioridad.Size = new Size(228, 23);
            cmbPrioridad.TabIndex = 26;
            // 
            // lblDepartamento
            // 
            lblDepartamento.AutoSize = true;
            lblDepartamento.Location = new Point(12, 72);
            lblDepartamento.Name = "lblDepartamento";
            lblDepartamento.Size = new Size(83, 15);
            lblDepartamento.TabIndex = 27;
            lblDepartamento.Text = "Departamento";
            // 
            // txtDepartamento
            // 
            txtDepartamento.Enabled = false;
            txtDepartamento.Location = new Point(134, 64);
            txtDepartamento.Name = "txtDepartamento";
            txtDepartamento.Size = new Size(228, 23);
            txtDepartamento.TabIndex = 28;
            // 
            // grpFeedback
            // 
            grpFeedback.Controls.Add(lblEstrellas);
            grpFeedback.Controls.Add(cmbEstrellas);
            grpFeedback.Controls.Add(lblComentario);
            grpFeedback.Controls.Add(txtComentario);
            grpFeedback.Controls.Add(btnEnviar);
            grpFeedback.Controls.Add(lblResumen);
            grpFeedback.Controls.Add(lblComentarioLectura);
            grpFeedback.Dock = DockStyle.Fill;
            grpFeedback.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grpFeedback.Location = new Point(3, 3);
            grpFeedback.Name = "grpFeedback";
            grpFeedback.Size = new Size(693, 438);
            grpFeedback.TabIndex = 0;
            grpFeedback.TabStop = false;
            grpFeedback.Text = "Retroalimentación de la Atención";
            // 
            // lblEstrellas
            // 
            lblEstrellas.AutoSize = true;
            lblEstrellas.Font = new Font("Segoe UI", 9F);
            lblEstrellas.Location = new Point(15, 25);
            lblEstrellas.Name = "lblEstrellas";
            lblEstrellas.Size = new Size(110, 15);
            lblEstrellas.TabIndex = 0;
            lblEstrellas.Text = "Calificación (1 al 5):";
            // 
            // cmbEstrellas
            // 
            cmbEstrellas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbEstrellas.Font = new Font("Segoe UI", 9F);
            cmbEstrellas.Items.AddRange(new object[] { "1 - Muy Malo", "2 - Malo", "3 - Regular", "4 - Bueno", "5 - Excelente" });
            cmbEstrellas.Location = new Point(140, 22);
            cmbEstrellas.Name = "cmbEstrellas";
            cmbEstrellas.Size = new Size(80, 23);
            cmbEstrellas.TabIndex = 1;
            // 
            // lblComentario
            // 
            lblComentario.AutoSize = true;
            lblComentario.Font = new Font("Segoe UI", 9F);
            lblComentario.Location = new Point(15, 55);
            lblComentario.Name = "lblComentario";
            lblComentario.Size = new Size(132, 15);
            lblComentario.TabIndex = 2;
            lblComentario.Text = "Comentario (Opcional):";
            // 
            // txtComentario
            // 
            txtComentario.Font = new Font("Segoe UI", 9F);
            txtComentario.Location = new Point(15, 75);
            txtComentario.Name = "txtComentario";
            txtComentario.Size = new Size(510, 23);
            txtComentario.TabIndex = 3;
            // 
            // btnEnviar
            // 
            btnEnviar.BackColor = Color.FromArgb(52, 152, 219);
            btnEnviar.FlatAppearance.BorderSize = 0;
            btnEnviar.FlatStyle = FlatStyle.Flat;
            btnEnviar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEnviar.ForeColor = Color.White;
            btnEnviar.Location = new Point(540, 72);
            btnEnviar.Name = "btnEnviar";
            btnEnviar.Size = new Size(130, 30);
            btnEnviar.TabIndex = 4;
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
            lblResumen.TabIndex = 5;
            // 
            // lblComentarioLectura
            // 
            lblComentarioLectura.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
            lblComentarioLectura.Location = new Point(15, 55);
            lblComentarioLectura.Name = "lblComentarioLectura";
            lblComentarioLectura.Size = new Size(650, 45);
            lblComentarioLectura.TabIndex = 6;
            // 
            // tabControlTicket
            // 
            tabControlTicket.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            tabControlTicket.Controls.Add(tabInfoGeneral);
            tabControlTicket.Controls.Add(tabDescripcionSolucion);
            tabControlTicket.Controls.Add(tbpHistorial);
            tabControlTicket.Controls.Add(tbpFeedback);
            tabControlTicket.Location = new Point(1, 12);
            tabControlTicket.Name = "tabControlTicket";
            tabControlTicket.SelectedIndex = 0;
            tabControlTicket.Size = new Size(707, 472);
            tabControlTicket.TabIndex = 0;
            // 
            // tabInfoGeneral
            // 
            tabInfoGeneral.AutoScroll = true;
            tabInfoGeneral.Controls.Add(lblFolio);
            tabInfoGeneral.Controls.Add(lblUsuario);
            tabInfoGeneral.Controls.Add(txtUsuario);
            tabInfoGeneral.Controls.Add(lblDepartamento);
            tabInfoGeneral.Controls.Add(txtDepartamento);
            tabInfoGeneral.Controls.Add(lblEstatus);
            tabInfoGeneral.Controls.Add(cmbEstatus);
            tabInfoGeneral.Controls.Add(lblPrioridad);
            tabInfoGeneral.Controls.Add(cmbPrioridad);
            tabInfoGeneral.Controls.Add(lblAlta);
            tabInfoGeneral.Controls.Add(txtAlta);
            tabInfoGeneral.Controls.Add(lblAtencion);
            tabInfoGeneral.Controls.Add(txtAtencion);
            tabInfoGeneral.Controls.Add(lblCierre);
            tabInfoGeneral.Controls.Add(txtCierre);
            tabInfoGeneral.Controls.Add(lblAtendido);
            tabInfoGeneral.Controls.Add(cmbAtendido);
            tabInfoGeneral.Location = new Point(4, 24);
            tabInfoGeneral.Name = "tabInfoGeneral";
            tabInfoGeneral.Padding = new Padding(3);
            tabInfoGeneral.Size = new Size(699, 444);
            tabInfoGeneral.TabIndex = 0;
            tabInfoGeneral.Text = "Información General";
            tabInfoGeneral.UseVisualStyleBackColor = true;
            // 
            // tabDescripcionSolucion
            // 
            tabDescripcionSolucion.AutoScroll = true;
            tabDescripcionSolucion.Controls.Add(lblDescripcion);
            tabDescripcionSolucion.Controls.Add(lblSolucion);
            tabDescripcionSolucion.Location = new Point(4, 24);
            tabDescripcionSolucion.Name = "tabDescripcionSolucion";
            tabDescripcionSolucion.Padding = new Padding(3);
            tabDescripcionSolucion.Size = new Size(699, 444);
            tabDescripcionSolucion.TabIndex = 1;
            tabDescripcionSolucion.Text = "Descripción y Solución";
            tabDescripcionSolucion.UseVisualStyleBackColor = true;
            // 
            // tbpHistorial
            // 
            tbpHistorial.AutoScroll = true;
            tbpHistorial.Controls.Add(dgvHistorial);
            tbpHistorial.Location = new Point(4, 24);
            tbpHistorial.Name = "tbpHistorial";
            tbpHistorial.Padding = new Padding(3);
            tbpHistorial.Size = new Size(699, 447);
            tbpHistorial.TabIndex = 2;
            tbpHistorial.Text = "Historial de cambios";
            tbpHistorial.UseVisualStyleBackColor = true;
            // 
            // tbpFeedback
            // 
            tbpFeedback.AutoScroll = true;
            tbpFeedback.Controls.Add(grpFeedback);
            tbpFeedback.Location = new Point(4, 24);
            tbpFeedback.Name = "tbpFeedback";
            tbpFeedback.Padding = new Padding(3);
            tbpFeedback.Size = new Size(699, 444);
            tbpFeedback.TabIndex = 3;
            tbpFeedback.Text = "Retroalimentación";
            tbpFeedback.UseVisualStyleBackColor = true;
            // 
            // TicketDetalleForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 534);
            Controls.Add(tabControlTicket);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            MinimumSize = new Size(600, 500);
            Name = "TicketDetalleForm";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormularioTicket";
            Load += FormularioTicket_Load;
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).EndInit();
            grpFeedback.ResumeLayout(false);
            grpFeedback.PerformLayout();
            tabControlTicket.ResumeLayout(false);
            tabInfoGeneral.ResumeLayout(false);
            tabInfoGeneral.PerformLayout();
            tabDescripcionSolucion.ResumeLayout(false);
            tabDescripcionSolucion.PerformLayout();
            tbpHistorial.ResumeLayout(false);
            tbpFeedback.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Label lblFolio;
        private ComboBox cmbEstatus;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblDescripcion;
        private Label lblAlta;
        private Label lblEstatus;
        private TextBox txtAlta;
        private Label lblSolucion;
        private Label lblAtendido;
        private Label lblAtencion;
        private TextBox txtAtencion;
        private Label lblCierre;
        private TextBox txtCierre;
        private ComboBox cmbAtendido;
        private Button btnGuardar;
        private Button btnCancelar;
        private DataGridView dgvHistorial;
        private Label lblPrioridad;
        private ComboBox cmbPrioridad;
        private Label lblDepartamento;
        private TextBox txtDepartamento;
        private GroupBox grpFeedback;
        private Label lblEstrellas;
        private ComboBox cmbEstrellas;
        private Label lblComentario;
        private TextBox txtComentario;
        private Button btnEnviar;
        private Label lblResumen;
        private Label lblComentarioLectura;
        private TabControl tabControlTicket;
        private TabPage tabInfoGeneral;
        private TabPage tabDescripcionSolucion;
        private TabPage tbpHistorial;
        private TabPage tbpFeedback;
        // rtbSolucion y rtbDescripcion serán creados dinámicamente en el código del formulario
    }
}