namespace HSis.UI.Controls;

partial class NotificacionesControl
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
        btnCampana = new System.Windows.Forms.Button();
        pnlHistorial = new System.Windows.Forms.Panel();
        pnlHeader = new System.Windows.Forms.Panel();
        lblNotifTitle = new System.Windows.Forms.Label();
        btnMarcarTodas = new System.Windows.Forms.Button();
        btnLimpiar = new System.Windows.Forms.Button();
        flpNotificaciones = new System.Windows.Forms.FlowLayoutPanel();
        pnlHistorial.SuspendLayout();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // btnCampana
        // 
        btnCampana.Dock = System.Windows.Forms.DockStyle.Right;
        btnCampana.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnCampana.FlatAppearance.BorderSize = 0;
        btnCampana.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        btnCampana.Location = new System.Drawing.Point(230, 0);
        btnCampana.Name = "btnCampana";
        btnCampana.Size = new System.Drawing.Size(100, 35);
        btnCampana.TabIndex = 0;
        btnCampana.Text = "🔔 (0)";
        btnCampana.UseVisualStyleBackColor = true;
        btnCampana.Click += BtnCampana_Click;
        // 
        // pnlHistorial
        // 
        pnlHistorial.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        pnlHistorial.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);
        pnlHistorial.Controls.Add(flpNotificaciones);
        pnlHistorial.Controls.Add(pnlHeader);
        pnlHistorial.Location = new System.Drawing.Point(0, 35);
        pnlHistorial.Name = "pnlHistorial";
        pnlHistorial.Padding = new System.Windows.Forms.Padding(10);
        pnlHistorial.Size = new System.Drawing.Size(330, 350);
        pnlHistorial.TabIndex = 1;
        pnlHistorial.Visible = false;
        // 
        // pnlHeader
        // 
        pnlHeader.Controls.Add(lblNotifTitle);
        pnlHeader.Controls.Add(btnMarcarTodas);
        pnlHeader.Controls.Add(btnLimpiar);
        pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
        pnlHeader.Location = new System.Drawing.Point(10, 10);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new System.Drawing.Size(310, 65);
        pnlHeader.TabIndex = 0;
        // 
        // lblNotifTitle
        // 
        lblNotifTitle.AutoSize = true;
        lblNotifTitle.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblNotifTitle.Location = new System.Drawing.Point(0, 0);
        lblNotifTitle.Name = "lblNotifTitle";
        lblNotifTitle.Size = new System.Drawing.Size(120, 21);
        lblNotifTitle.TabIndex = 0;
        lblNotifTitle.Text = "Notificaciones";
        // 
        // btnMarcarTodas
        // 
        btnMarcarTodas.BackColor = System.Drawing.Color.White;
        btnMarcarTodas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnMarcarTodas.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
        btnMarcarTodas.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        btnMarcarTodas.Location = new System.Drawing.Point(0, 32);
        btnMarcarTodas.Name = "btnMarcarTodas";
        btnMarcarTodas.Size = new System.Drawing.Size(145, 26);
        btnMarcarTodas.TabIndex = 1;
        btnMarcarTodas.Text = "Marcar todas leídas";
        btnMarcarTodas.UseVisualStyleBackColor = false;
        btnMarcarTodas.Click += BtnMarcarTodasLeidas_Click;
        // 
        // btnLimpiar
        // 
        btnLimpiar.BackColor = System.Drawing.Color.White;
        btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnLimpiar.FlatAppearance.BorderColor = System.Drawing.Color.LightGray;
        btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        btnLimpiar.Location = new System.Drawing.Point(152, 32);
        btnLimpiar.Name = "btnLimpiar";
        btnLimpiar.Size = new System.Drawing.Size(100, 26);
        btnLimpiar.TabIndex = 2;
        btnLimpiar.Text = "Limpiar todo";
        btnLimpiar.UseVisualStyleBackColor = false;
        btnLimpiar.Click += BtnLimpiar_Click;
        // 
        // flpNotificaciones
        // 
        flpNotificaciones.AutoScroll = true;
        flpNotificaciones.Dock = System.Windows.Forms.DockStyle.Fill;
        flpNotificaciones.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        flpNotificaciones.Location = new System.Drawing.Point(10, 75);
        flpNotificaciones.Name = "flpNotificaciones";
        flpNotificaciones.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
        flpNotificaciones.Size = new System.Drawing.Size(310, 265);
        flpNotificaciones.TabIndex = 1;
        flpNotificaciones.WrapContents = false;
        // 
        // NotificacionesControl
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        Controls.Add(pnlHistorial);
        Controls.Add(btnCampana);
        Name = "NotificacionesControl";
        Size = new System.Drawing.Size(330, 385);
        pnlHistorial.ResumeLayout(false);
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Button btnCampana;
    private System.Windows.Forms.Panel pnlHistorial;
    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblNotifTitle;
    private System.Windows.Forms.Button btnMarcarTodas;
    private System.Windows.Forms.Button btnLimpiar;
    private System.Windows.Forms.FlowLayoutPanel flpNotificaciones;
}
