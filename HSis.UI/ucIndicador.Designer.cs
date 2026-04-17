namespace HSis.UI
{
    partial class ucIndicador
    {
        /// <summary> 
        /// Variable del diseñador necesaria.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpiar los recursos que se estén usando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben desechar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de componentes

        /// <summary> 
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido de este método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            pnlPrincipal = new Panel();
            pbxIcono = new PictureBox();
            lblCantidad = new Label();
            lblTitulo = new Label();
            pnlPrincipal.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pbxIcono).BeginInit();
            SuspendLayout();
            // 
            // pnlPrincipal
            // 
            pnlPrincipal.BackColor = Color.DodgerBlue;
            pnlPrincipal.Controls.Add(pbxIcono);
            pnlPrincipal.Controls.Add(lblCantidad);
            pnlPrincipal.Controls.Add(lblTitulo);
            pnlPrincipal.Dock = DockStyle.Fill;
            pnlPrincipal.Location = new Point(0, 0);
            pnlPrincipal.Name = "pnlPrincipal";
            pnlPrincipal.Size = new Size(200, 100);
            pnlPrincipal.TabIndex = 0;
            pnlPrincipal.Click += ucIndicador_Click;
            // 
            // pbxIcono
            // 
            pbxIcono.BackColor = Color.Transparent;
            pbxIcono.Location = new Point(152, 57);
            pbxIcono.Name = "pbxIcono";
            pbxIcono.Size = new Size(45, 43);
            pbxIcono.SizeMode = PictureBoxSizeMode.Zoom;
            pbxIcono.TabIndex = 2;
            pbxIcono.TabStop = false;
            pbxIcono.Click += ucIndicador_Click;
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblCantidad.Location = new Point(76, 31);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(38, 45);
            lblCantidad.TabIndex = 1;
            lblCantidad.Text = "1";
            lblCantidad.Click += ucIndicador_Click;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(114, 21);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Tickets Nuevos";
            lblTitulo.Click += ucIndicador_Click;
            // 
            // ucIndicador
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlPrincipal);
            Name = "ucIndicador";
            Size = new Size(200, 100);
            pnlPrincipal.ResumeLayout(false);
            pnlPrincipal.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pbxIcono).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlPrincipal;
        private Label lblTitulo;
        private Label lblCantidad;
        private PictureBox pbxIcono;
    }
}
