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
            rtbDescripcion.Size = new Size(685, 80);
            rtbDescripcion.TabIndex = 12;
            rtbDescripcion.Text = "";
            // 
            // lblAtendido
            // 
            lblAtendido.Location = new Point(12, 186);
            lblAtendido.Name = "lblAtendido";
            lblAtendido.Size = new Size(100, 23);
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
            btnGuardar.Location = new Point(541, 418);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 19;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(622, 418);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 20;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
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
            // frmTicket
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(709, 695);
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
            Name = "frmTicket";
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
    }
}