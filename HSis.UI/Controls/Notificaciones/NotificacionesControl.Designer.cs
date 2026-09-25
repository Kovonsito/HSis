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
        lblNotifSubtitle = new System.Windows.Forms.Label();
        lblBadgeCount = new System.Windows.Forms.Label();
        btnMarcarTodas = new BotonModerno();
        btnLimpiar = new BotonModerno();
        flpNotificaciones = new System.Windows.Forms.FlowLayoutPanel();
        pnlHeader.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = System.Drawing.Color.White;
        pnlHeader.Controls.Add(lblNotifTitle);
        pnlHeader.Controls.Add(lblNotifSubtitle);
        pnlHeader.Controls.Add(lblBadgeCount);
        pnlHeader.Controls.Add(btnMarcarTodas);
        pnlHeader.Controls.Add(btnLimpiar);
        pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
        pnlHeader.Location = new System.Drawing.Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new System.Drawing.Size(360, 100);
        pnlHeader.TabIndex = 0;
        // 
        // lblNotifTitle
        // 
        lblNotifTitle.AutoSize = true;
        lblNotifTitle.Font = new System.Drawing.Font("Segoe UI", 11.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblNotifTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        lblNotifTitle.Location = new System.Drawing.Point(16, 10);
        lblNotifTitle.Name = "lblNotifTitle";
        lblNotifTitle.Size = new System.Drawing.Size(117, 21);
        lblNotifTitle.TabIndex = 0;
        lblNotifTitle.Text = "Notificaciones";
        //
        // lblNotifSubtitle
        //
        lblNotifSubtitle.AutoSize = false;
        lblNotifSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
        lblNotifSubtitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
        lblNotifSubtitle.Location = new System.Drawing.Point(16, 38);
        lblNotifSubtitle.Name = "lblNotifSubtitle";
        lblNotifSubtitle.Size = new System.Drawing.Size(138, 18);
        lblNotifSubtitle.TabIndex = 1;
        lblNotifSubtitle.Text = "Actividad reciente";
        //
        // lblBadgeCount
        //
        lblBadgeCount.AutoSize = true;
        lblBadgeCount.BackColor = System.Drawing.Color.FromArgb(239, 68, 68);
        lblBadgeCount.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
        lblBadgeCount.ForeColor = System.Drawing.Color.White;
        lblBadgeCount.Location = new System.Drawing.Point(138, 12);
        lblBadgeCount.Name = "lblBadgeCount";
        lblBadgeCount.Padding = new System.Windows.Forms.Padding(5, 2, 5, 2);
        lblBadgeCount.Size = new System.Drawing.Size(22, 17);
        lblBadgeCount.TabIndex = 2;
        lblBadgeCount.Text = "0";
        lblBadgeCount.Visible = false;
        // 
        // btnMarcarTodas
        // 
        btnMarcarTodas.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        btnMarcarTodas.Estilo = EstiloBotonModerno.Secundario;
        btnMarcarTodas.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
        btnMarcarTodas.Icono = FontAwesome.Sharp.IconChar.Check;
        btnMarcarTodas.IconoTamano = 12;
        btnMarcarTodas.Location = new System.Drawing.Point(150, 58);
        btnMarcarTodas.Name = "btnMarcarTodas";
        btnMarcarTodas.RadioBorde = 6;
        btnMarcarTodas.Size = new System.Drawing.Size(106, 34);
        btnMarcarTodas.TabIndex = 3;
        btnMarcarTodas.Text = "Leídas";
        btnMarcarTodas.Click += BtnMarcarTodasLeidas_Click;
        // 
        // btnLimpiar
        // 
        btnLimpiar.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        btnLimpiar.Estilo = EstiloBotonModerno.Ghost;
        btnLimpiar.Font = new System.Drawing.Font("Segoe UI Semibold", 8.5F, System.Drawing.FontStyle.Bold);
        btnLimpiar.Icono = FontAwesome.Sharp.IconChar.Eraser;
        btnLimpiar.IconoTamano = 12;
        btnLimpiar.Location = new System.Drawing.Point(258, 58);
        btnLimpiar.Name = "btnLimpiar";
        btnLimpiar.RadioBorde = 6;
        btnLimpiar.Size = new System.Drawing.Size(86, 34);
        btnLimpiar.TabIndex = 4;
        btnLimpiar.Text = "Limpiar";
        btnLimpiar.Click += BtnLimpiar_Click;
        // 
        // flpNotificaciones
        // 
        flpNotificaciones.AutoScroll = true;
        flpNotificaciones.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
        flpNotificaciones.Dock = System.Windows.Forms.DockStyle.Fill;
        flpNotificaciones.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
        flpNotificaciones.Location = new System.Drawing.Point(0, 100);
        flpNotificaciones.Name = "flpNotificaciones";
        flpNotificaciones.Padding = new System.Windows.Forms.Padding(12, 12, 12, 12);
        flpNotificaciones.Size = new System.Drawing.Size(360, 360);
        flpNotificaciones.TabIndex = 1;
        flpNotificaciones.WrapContents = false;
        // 
        // NotificacionesControl
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.Transparent;
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
    private System.Windows.Forms.Label lblNotifSubtitle;
    private System.Windows.Forms.Label lblBadgeCount;
    private BotonModerno btnMarcarTodas;
    private BotonModerno btnLimpiar;
    private System.Windows.Forms.FlowLayoutPanel flpNotificaciones;
}
