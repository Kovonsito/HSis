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
        pnlHeader = new System.Windows.Forms.Panel();
        lblNotifTitle = new System.Windows.Forms.Label();
        lblBadgeCount = new System.Windows.Forms.Label();
        btnMarcarTodas = new System.Windows.Forms.Button();
        btnLimpiar = new System.Windows.Forms.Button();
        flpNotificaciones = new System.Windows.Forms.FlowLayoutPanel();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = System.Drawing.Color.White;
        pnlHeader.Controls.Add(lblNotifTitle);
        pnlHeader.Controls.Add(lblBadgeCount);
        pnlHeader.Controls.Add(btnMarcarTodas);
        pnlHeader.Controls.Add(btnLimpiar);
        pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
        pnlHeader.Location = new System.Drawing.Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new System.Drawing.Size(360, 56);
        pnlHeader.TabIndex = 0;
        // 
        // lblNotifTitle
        // 
        lblNotifTitle.AutoSize = true;
        lblNotifTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblNotifTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        lblNotifTitle.Location = new System.Drawing.Point(14, 16);
        lblNotifTitle.Name = "lblNotifTitle";
        lblNotifTitle.Size = new System.Drawing.Size(117, 21);
        lblNotifTitle.TabIndex = 0;
        lblNotifTitle.Text = "Notificaciones";
        // 
        // lblBadgeCount
        // 
        lblBadgeCount.AutoSize = true;
        lblBadgeCount.BackColor = System.Drawing.Color.FromArgb(239, 68, 68);
        lblBadgeCount.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblBadgeCount.ForeColor = System.Drawing.Color.White;
        lblBadgeCount.Location = new System.Drawing.Point(135, 18);
        lblBadgeCount.Name = "lblBadgeCount";
        lblBadgeCount.Padding = new System.Windows.Forms.Padding(5, 1, 5, 1);
        lblBadgeCount.Size = new System.Drawing.Size(22, 15);
        lblBadgeCount.TabIndex = 1;
        lblBadgeCount.Text = "0";
        lblBadgeCount.Visible = false;
        // 
        // btnMarcarTodas
        // 
        btnMarcarTodas.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        btnMarcarTodas.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        btnMarcarTodas.Cursor = System.Windows.Forms.Cursors.Hand;
        btnMarcarTodas.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnMarcarTodas.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnMarcarTodas.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        btnMarcarTodas.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
        btnMarcarTodas.Location = new System.Drawing.Point(198, 14);
        btnMarcarTodas.Name = "btnMarcarTodas";
        btnMarcarTodas.Size = new System.Drawing.Size(86, 28);
        btnMarcarTodas.TabIndex = 2;
        btnMarcarTodas.Text = "Leídas";
        btnMarcarTodas.UseVisualStyleBackColor = false;
        btnMarcarTodas.Click += BtnMarcarTodasLeidas_Click;
        // 
        // btnLimpiar
        // 
        btnLimpiar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        btnLimpiar.BackColor = System.Drawing.Color.FromArgb(241, 245, 249);
        btnLimpiar.Cursor = System.Windows.Forms.Cursors.Hand;
        btnLimpiar.FlatAppearance.BorderColor = System.Drawing.Color.FromArgb(226, 232, 240);
        btnLimpiar.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
        btnLimpiar.Font = new System.Drawing.Font("Segoe UI", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        btnLimpiar.ForeColor = System.Drawing.Color.FromArgb(220, 38, 38);
        btnLimpiar.Location = new System.Drawing.Point(290, 14);
        btnLimpiar.Name = "btnLimpiar";
        btnLimpiar.Size = new System.Drawing.Size(56, 28);
        btnLimpiar.TabIndex = 3;
        btnLimpiar.Text = "Limpiar";
        btnLimpiar.UseVisualStyleBackColor = false;
        btnLimpiar.Click += BtnLimpiar_Click;
        // 
        // flpNotificaciones
        // 
        flpNotificaciones.AutoScroll = true;
        flpNotificaciones.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
        flpNotificaciones.Dock = System.Windows.Forms.DockStyle.Fill;
        flpNotificaciones.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        flpNotificaciones.Location = new System.Drawing.Point(0, 56);
        flpNotificaciones.Name = "flpNotificaciones";
        flpNotificaciones.Padding = new System.Windows.Forms.Padding(12, 10, 12, 10);
        flpNotificaciones.Size = new System.Drawing.Size(360, 404);
        flpNotificaciones.TabIndex = 1;
        flpNotificaciones.WrapContents = false;
        // 
        // NotificacionesControl
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.White;
        Controls.Add(flpNotificaciones);
        Controls.Add(pnlHeader);
        DoubleBuffered = true;
        Name = "NotificacionesControl";
        Size = new System.Drawing.Size(360, 460);
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.Label lblNotifTitle;
    private System.Windows.Forms.Label lblBadgeCount;
    private System.Windows.Forms.Button btnMarcarTodas;
    private System.Windows.Forms.Button btnLimpiar;
    private System.Windows.Forms.FlowLayoutPanel flpNotificaciones;
}
