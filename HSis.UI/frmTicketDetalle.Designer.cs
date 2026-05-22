namespace HSis.UI
{
    partial class frmTicketDetalle
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
            rtbSolucion = new RichTextBox();
            lblAlta = new Label();
            lblDescripcion = new Label();
            dtpAlta = new DateTimePicker();
            lblSolucion = new Label();
            rtbDescripcion = new RichTextBox();
            lblAtendido = new Label();
            lblAtencion = new Label();
            dtpAtencion = new DateTimePicker();
            lblCierre = new Label();
            dtpCierre = new DateTimePicker();
            cmbAtendido = new ComboBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            dgvHistorial = new DataGridView();
            lblHistoria = new Label();
            lblPrioridad = new Label();
            cmbPrioridad = new ComboBox();
            this.lblDepartamento = new Label();
            txtDepartamento = new TextBox();
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).BeginInit();
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
            cmbEstatus.Items.AddRange(new object[] { "Abierto", "En proceso", "Cerrado", "Reabierto" });
            cmbEstatus.Location = new Point(134, 64);
            cmbEstatus.Name = "cmbEstatus";
            cmbEstatus.Size = new Size(136, 23);
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
            lblEstatus.Location = new Point(12, 72);
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
            // rtbSolucion
            // 
            rtbSolucion.Location = new Point(12, 332);
            rtbSolucion.Name = "rtbSolucion";
            rtbSolucion.Size = new Size(685, 80);
            rtbSolucion.TabIndex = 8;
            rtbSolucion.Text = "";
            // 
            // lblAlta
            // 
            lblAlta.AutoSize = true;
            lblAlta.Location = new Point(12, 99);
            lblAlta.Name = "lblAlta";
            lblAlta.Size = new Size(65, 15);
            lblAlta.TabIndex = 9;
            lblAlta.Text = "Fecha Alta:";
            // 
            // lblDescripcion
            // 
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(12, 209);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(69, 15);
            lblDescripcion.TabIndex = 1;
            lblDescripcion.Text = "Descripción";
            // 
            // dtpAlta
            // 
            dtpAlta.CustomFormat = "dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss";
            dtpAlta.Format = DateTimePickerFormat.Custom;
            dtpAlta.Location = new Point(134, 93);
            dtpAlta.Name = "dtpAlta";
            dtpAlta.Size = new Size(343, 23);
            dtpAlta.TabIndex = 10;
            // 
            // lblSolucion
            // 
            lblSolucion.AutoSize = true;
            lblSolucion.Location = new Point(12, 314);
            lblSolucion.Name = "lblSolucion";
            lblSolucion.Size = new Size(53, 15);
            lblSolucion.TabIndex = 11;
            lblSolucion.Text = "Solución";
            // 
            // rtbDescripcion
            // 
            rtbDescripcion.Enabled = false;
            rtbDescripcion.Location = new Point(12, 231);
            rtbDescripcion.Name = "rtbDescripcion";
            rtbDescripcion.ReadOnly = true;
            rtbDescripcion.Size = new Size(685, 80);
            rtbDescripcion.TabIndex = 12;
            rtbDescripcion.Text = "";
            // 
            // lblAtendido
            // 
            lblAtendido.AutoSize = true;
            lblAtendido.Location = new Point(12, 186);
            lblAtendido.Name = "lblAtendido";
            lblAtendido.Size = new Size(80, 15);
            lblAtendido.TabIndex = 24;
            lblAtendido.Text = "Atendido por:";
            // 
            // lblAtencion
            // 
            lblAtencion.AutoSize = true;
            lblAtencion.Location = new Point(12, 129);
            lblAtencion.Name = "lblAtencion";
            lblAtencion.Size = new Size(92, 15);
            lblAtencion.TabIndex = 14;
            lblAtencion.Text = "Fecha Atención:";
            // 
            // dtpAtencion
            // 
            dtpAtencion.CustomFormat = "dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss";
            dtpAtencion.Format = DateTimePickerFormat.Custom;
            dtpAtencion.Location = new Point(134, 121);
            dtpAtencion.Name = "dtpAtencion";
            dtpAtencion.Size = new Size(343, 23);
            dtpAtencion.TabIndex = 15;
            // 
            // lblCierre
            // 
            lblCierre.AutoSize = true;
            lblCierre.Location = new Point(12, 158);
            lblCierre.Name = "lblCierre";
            lblCierre.Size = new Size(75, 15);
            lblCierre.TabIndex = 16;
            lblCierre.Text = "Fecha Cierra:";
            // 
            // dtpCierre
            // 
            dtpCierre.CustomFormat = "dddd, dd 'de' MMMM 'de' yyyy 'a las' HH:mm:ss";
            dtpCierre.Format = DateTimePickerFormat.Custom;
            dtpCierre.Location = new Point(134, 150);
            dtpCierre.Name = "dtpCierre";
            dtpCierre.Size = new Size(343, 23);
            dtpCierre.TabIndex = 17;
            // 
            // cmbAtendido
            // 
            cmbAtendido.Location = new Point(134, 183);
            cmbAtendido.Name = "cmbAtendido";
            cmbAtendido.Size = new Size(121, 23);
            cmbAtendido.TabIndex = 23;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.FromArgb(39, 174, 96);
            btnGuardar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.White;
            btnGuardar.Location = new Point(497, 418);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(95, 30);
            btnGuardar.TabIndex = 19;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.BackColor = Color.FromArgb(231, 76, 60);
            btnCancelar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.White;
            btnCancelar.Location = new Point(602, 418);
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
            dgvHistorial.Location = new Point(12, 456);
            dgvHistorial.Name = "dgvHistorial";
            dgvHistorial.ReadOnly = true;
            dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistorial.Size = new Size(685, 231);
            dgvHistorial.TabIndex = 21;
            // 
            // lblHistoria
            // 
            lblHistoria.AutoSize = true;
            lblHistoria.Location = new Point(12, 438);
            lblHistoria.Name = "lblHistoria";
            lblHistoria.Size = new Size(115, 15);
            lblHistoria.TabIndex = 22;
            lblHistoria.Text = "Historial de cambios";
            // 
            // lblPrioridad
            // 
            lblPrioridad.AutoSize = true;
            lblPrioridad.Location = new Point(280, 68);
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
            cmbPrioridad.Location = new Point(350, 64);
            cmbPrioridad.Name = "cmbPrioridad";
            cmbPrioridad.Size = new Size(120, 23);
            cmbPrioridad.TabIndex = 26;
            // 
            // lblDepartamento
            // 
            this.lblDepartamento.AutoSize = true;
            this.lblDepartamento.Location = new Point(368, 43);
            this.lblDepartamento.Name = "lblDepartamento";
            this.lblDepartamento.Size = new Size(83, 15);
            this.lblDepartamento.TabIndex = 27;
            this.lblDepartamento.Text = "Departamento";
            // 
            // txtDepartamento
            // 
            txtDepartamento.Enabled = false;
            txtDepartamento.Location = new Point(457, 40);
            txtDepartamento.Name = "txtDepartamento";
            txtDepartamento.Size = new Size(200, 23);
            txtDepartamento.TabIndex = 28;
            // 
            // frmTicketDetalle
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 695);
            Controls.Add(txtDepartamento);
            Controls.Add(lblDepartamento);
            Controls.Add(lblPrioridad);
            Controls.Add(cmbPrioridad);
            Controls.Add(lblHistoria);
            Controls.Add(dgvHistorial);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            Controls.Add(cmbAtendido);
            Controls.Add(dtpCierre);
            Controls.Add(lblCierre);
            Controls.Add(dtpAtencion);
            Controls.Add(lblAtencion);
            Controls.Add(lblAtendido);
            Controls.Add(rtbDescripcion);
            Controls.Add(lblSolucion);
            Controls.Add(dtpAlta);
            Controls.Add(lblAlta);
            Controls.Add(rtbSolucion);
            Controls.Add(txtUsuario);
            Controls.Add(lblEstatus);
            Controls.Add(lblUsuario);
            Controls.Add(cmbEstatus);
            Controls.Add(lblDescripcion);
            Controls.Add(lblFolio);
            Name = "frmTicketDetalle";
            StartPosition = FormStartPosition.CenterParent;
            Text = "FormularioTicket";
            Load += FormularioTicket_Load;
            ((System.ComponentModel.ISupportInitialize)dgvHistorial).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblFolio;
        private ComboBox cmbEstatus;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblDescripcion;
        private RichTextBox rtbSolucion;
        private Label lblAlta;
        private Label lblEstatus;
        private DateTimePicker dtpAlta;
        private Label lblSolucion;
        private RichTextBox rtbDescripcion;
        private Label lblAtendido;
        private Label lblAtencion;
        private DateTimePicker dtpAtencion;
        private Label lblCierre;
        private DateTimePicker dtpCierre;
        private ComboBox cmbAtendido;
        private Button btnGuardar;
        private Button btnCancelar;
        private DataGridView dgvHistorial;
        private Label lblHistoria;
        private Label lblPrioridad;
        private ComboBox cmbPrioridad;
        private Label lblDepartamento;
        private TextBox txtDepartamento;
    }
}