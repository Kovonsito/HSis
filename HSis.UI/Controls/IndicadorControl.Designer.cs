namespace HSis.UI.Controls;

partial class IndicadorControl
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

    #region Código generado por el Diseñador de componentes

    private void InitializeComponent()
    {
        pbxIcono = new System.Windows.Forms.PictureBox();
        lblCantidad = new System.Windows.Forms.Label();
        lblTitulo = new System.Windows.Forms.Label();
        ((System.ComponentModel.ISupportInitialize)pbxIcono).BeginInit();
        SuspendLayout();
        // 
        // pbxIcono
        // 
        pbxIcono.BackColor = System.Drawing.Color.Transparent;
        pbxIcono.Location = new System.Drawing.Point(150, 12);
        pbxIcono.Name = "pbxIcono";
        pbxIcono.Size = new System.Drawing.Size(34, 34);
        pbxIcono.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
        pbxIcono.TabIndex = 2;
        pbxIcono.TabStop = false;
        pbxIcono.Click += Indicador_Click;
        // 
        // lblCantidad
        // 
        lblCantidad.AutoSize = true;
        lblCantidad.BackColor = System.Drawing.Color.Transparent;
        lblCantidad.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
        lblCantidad.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        lblCantidad.Location = new System.Drawing.Point(14, 34);
        lblCantidad.Name = "lblCantidad";
        lblCantidad.Size = new System.Drawing.Size(33, 37);
        lblCantidad.TabIndex = 1;
        lblCantidad.Text = "0";
        lblCantidad.Click += Indicador_Click;
        // 
        // lblTitulo
        // 
        lblTitulo.AutoSize = true;
        lblTitulo.BackColor = System.Drawing.Color.Transparent;
        lblTitulo.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
        lblTitulo.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
        lblTitulo.Location = new System.Drawing.Point(14, 12);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new System.Drawing.Size(80, 15);
        lblTitulo.TabIndex = 0;
        lblTitulo.Text = "INDICADOR";
        lblTitulo.Click += Indicador_Click;
        // 
        // IndicadorControl
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.Transparent;
        Controls.Add(pbxIcono);
        Controls.Add(lblCantidad);
        Controls.Add(lblTitulo);
        Cursor = System.Windows.Forms.Cursors.Hand;
        Name = "IndicadorControl";
        Size = new System.Drawing.Size(200, 85);
        Click += Indicador_Click;
        ((System.ComponentModel.ISupportInitialize)pbxIcono).EndInit();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label lblTitulo;
    private System.Windows.Forms.Label lblCantidad;
    private System.Windows.Forms.PictureBox pbxIcono;
}
