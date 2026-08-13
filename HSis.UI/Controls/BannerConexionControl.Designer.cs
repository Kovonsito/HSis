namespace HSis.UI.Controls;

partial class BannerConexionControl
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
        lblMensaje = new System.Windows.Forms.Label();
        SuspendLayout();
        // 
        // lblMensaje
        // 
        lblMensaje.Dock = System.Windows.Forms.DockStyle.Fill;
        lblMensaje.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblMensaje.ForeColor = System.Drawing.Color.White;
        lblMensaje.Location = new System.Drawing.Point(0, 0);
        lblMensaje.Name = "lblMensaje";
        lblMensaje.Size = new System.Drawing.Size(600, 35);
        lblMensaje.TabIndex = 0;
        lblMensaje.Text = "⚠️ Sin conexión con el servidor de notificaciones.";
        lblMensaje.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        // 
        // BannerConexionControl
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(231, 76, 60);
        Controls.Add(lblMensaje);
        Name = "BannerConexionControl";
        Size = new System.Drawing.Size(600, 35);
        Visible = false;
        ResumeLayout(false);
    }

    private System.Windows.Forms.Label lblMensaje;
}
