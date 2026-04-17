namespace HSis.UI
{
    partial class frmCrearUsuario
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
            lblNombre = new Label();
            lblContraseña = new Label();
            lblDepartamento = new Label();
            lblPuesto = new Label();
            lblSucursal = new Label();
            txtNombre = new TextBox();
            txtContraseña = new TextBox();
            cmbDepartamento = new ComboBox();
            cmbPuesto = new ComboBox();
            cmbSucursal = new ComboBox();
            btnGuardar = new Button();
            btnCancelar = new Button();
            SuspendLayout();
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(12, 15);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(54, 15);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre:";
            // 
            // lblContraseña
            // 
            lblContraseña.AutoSize = true;
            lblContraseña.Location = new Point(12, 50);
            lblContraseña.Name = "lblContraseña";
            lblContraseña.Size = new Size(70, 15);
            lblContraseña.TabIndex = 1;
            lblContraseña.Text = "Contraseña:";
            // 
            // lblDepartamento
            // 
            lblDepartamento.AutoSize = true;
            lblDepartamento.Location = new Point(12, 85);
            lblDepartamento.Name = "lblDepartamento";
            lblDepartamento.Size = new Size(86, 15);
            lblDepartamento.TabIndex = 2;
            lblDepartamento.Text = "Departamento:";
            // 
            // lblPuesto
            // 
            lblPuesto.AutoSize = true;
            lblPuesto.Location = new Point(12, 120);
            lblPuesto.Name = "lblPuesto";
            lblPuesto.Size = new Size(46, 15);
            lblPuesto.TabIndex = 3;
            lblPuesto.Text = "Puesto:";
            // 
            // lblSucursal
            // 
            lblSucursal.AutoSize = true;
            lblSucursal.Location = new Point(12, 155);
            lblSucursal.Name = "lblSucursal";
            lblSucursal.Size = new Size(54, 15);
            lblSucursal.TabIndex = 4;
            lblSucursal.Text = "Sucursal:";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(120, 12);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(240, 23);
            txtNombre.TabIndex = 5;
            // 
            // txtContraseña
            // 
            txtContraseña.Location = new Point(120, 47);
            txtContraseña.Name = "txtContraseña";
            txtContraseña.Size = new Size(240, 23);
            txtContraseña.TabIndex = 6;
            txtContraseña.UseSystemPasswordChar = true;
            // 
            // cmbDepartamento
            // 
            cmbDepartamento.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbDepartamento.FormattingEnabled = true;
            cmbDepartamento.Location = new Point(120, 82);
            cmbDepartamento.Name = "cmbDepartamento";
            cmbDepartamento.Size = new Size(240, 23);
            cmbDepartamento.TabIndex = 7;
            // 
            // cmbPuesto
            // 
            cmbPuesto.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPuesto.FormattingEnabled = true;
            cmbPuesto.Location = new Point(120, 117);
            cmbPuesto.Name = "cmbPuesto";
            cmbPuesto.Size = new Size(240, 23);
            cmbPuesto.TabIndex = 8;
            // 
            // cmbSucursal
            // 
            cmbSucursal.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbSucursal.FormattingEnabled = true;
            cmbSucursal.Location = new Point(120, 152);
            cmbSucursal.Name = "cmbSucursal";
            cmbSucursal.Size = new Size(240, 23);
            cmbSucursal.TabIndex = 9;
            // 
            // btnGuardar
            // 
            btnGuardar.Location = new Point(204, 195);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(75, 23);
            btnGuardar.TabIndex = 10;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = true;
            btnGuardar.Click += btnGuardar_Click;
            // 
            // btnCancelar
            // 
            btnCancelar.Location = new Point(285, 195);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(75, 23);
            btnCancelar.TabIndex = 11;
            btnCancelar.Text = "Cancelar";
            btnCancelar.UseVisualStyleBackColor = true;
            btnCancelar.Click += btnCancelar_Click;
            // 
            // frmCrearUsuario
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(377, 230);
            Controls.Add(btnCancelar);
            Controls.Add(btnGuardar);
            Controls.Add(cmbSucursal);
            Controls.Add(cmbPuesto);
            Controls.Add(cmbDepartamento);
            Controls.Add(txtContraseña);
            Controls.Add(txtNombre);
            Controls.Add(lblSucursal);
            Controls.Add(lblPuesto);
            Controls.Add(lblDepartamento);
            Controls.Add(lblContraseña);
            Controls.Add(lblNombre);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "frmCrearUsuario";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Crear Usuario";
            Load += frmCrearUsuario_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblNombre;
        private System.Windows.Forms.Label lblContraseña;
        private System.Windows.Forms.Label lblDepartamento;
        private System.Windows.Forms.Label lblPuesto;
        private System.Windows.Forms.Label lblSucursal;
        private System.Windows.Forms.TextBox txtNombre;
        private System.Windows.Forms.TextBox txtContraseña;
        private System.Windows.Forms.ComboBox cmbDepartamento;
        private System.Windows.Forms.ComboBox cmbPuesto;
        private System.Windows.Forms.ComboBox cmbSucursal;
        private System.Windows.Forms.Button btnGuardar;
        private System.Windows.Forms.Button btnCancelar;
    }
}
