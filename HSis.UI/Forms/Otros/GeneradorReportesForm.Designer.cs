namespace HSis.UI.Forms.Otros;

partial class GeneradorReportesForm
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

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        pnlHeader = new System.Windows.Forms.Panel();
        picHeaderIcon = new System.Windows.Forms.PictureBox();
        lblTitle = new System.Windows.Forms.Label();
        lblSubtitle = new System.Windows.Forms.Label();
        pnlBody = new System.Windows.Forms.Panel();
        pnlCard = new System.Windows.Forms.Panel();
        lblRangoTitle = new System.Windows.Forms.Label();
        lblInicio = new System.Windows.Forms.Label();
        dtpInicio = new HSis.UI.Controls.SelectorFechaModerno();
        lblFin = new System.Windows.Forms.Label();
        dtpFin = new HSis.UI.Controls.SelectorFechaModerno();
        flpBotones = new System.Windows.Forms.FlowLayoutPanel();
        btnExcel = new HSis.UI.Controls.BotonModerno();
        btnPdf = new HSis.UI.Controls.BotonModerno();
        pnlHeader.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picHeaderIcon).BeginInit();
        pnlBody.SuspendLayout();
        pnlCard.SuspendLayout();
        flpBotones.SuspendLayout();
        SuspendLayout();
        // 
        // pnlHeader
        // 
        pnlHeader.BackColor = System.Drawing.Color.White;
        pnlHeader.Controls.Add(picHeaderIcon);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
        pnlHeader.Location = new System.Drawing.Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new System.Drawing.Size(600, 88);
        pnlHeader.TabIndex = 0;
        pnlHeader.Paint += PnlHeader_Paint;
        // 
        // picHeaderIcon
        // 
        picHeaderIcon.Location = new System.Drawing.Point(24, 22);
        picHeaderIcon.Name = "picHeaderIcon";
        picHeaderIcon.Size = new System.Drawing.Size(40, 40);
        picHeaderIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        picHeaderIcon.TabIndex = 0;
        picHeaderIcon.TabStop = false;
        // 
        // lblTitle
        // 
        lblTitle.AutoSize = true;
        lblTitle.Font = new System.Drawing.Font("Segoe UI", 13F, System.Drawing.FontStyle.Bold);
        lblTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        lblTitle.Location = new System.Drawing.Point(76, 20);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new System.Drawing.Size(284, 23);
        lblTitle.TabIndex = 1;
        lblTitle.Text = "Generador de Reportes Ejecutivos";
        // 
        // lblSubtitle
        // 
        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 8.5F);
        lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
        lblSubtitle.Location = new System.Drawing.Point(78, 49);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new System.Drawing.Size(415, 15);
        lblSubtitle.TabIndex = 2;
        lblSubtitle.Text = "Filtra por período de fechas y exporta las métricas en formato Excel o PDF.";
        // 
        // pnlBody
        // 
        pnlBody.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
        pnlBody.Controls.Add(pnlCard);
        pnlBody.Dock = System.Windows.Forms.DockStyle.Fill;
        pnlBody.Location = new System.Drawing.Point(0, 88);
        pnlBody.Name = "pnlBody";
        pnlBody.Padding = new System.Windows.Forms.Padding(24);
        pnlBody.Size = new System.Drawing.Size(600, 352);
        pnlBody.TabIndex = 1;
        // 
        // pnlCard
        // 
        pnlCard.BackColor = System.Drawing.Color.White;
        pnlCard.Controls.Add(lblRangoTitle);
        pnlCard.Controls.Add(lblInicio);
        pnlCard.Controls.Add(dtpInicio);
        pnlCard.Controls.Add(lblFin);
        pnlCard.Controls.Add(dtpFin);
        pnlCard.Controls.Add(flpBotones);
        pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;
        pnlCard.Location = new System.Drawing.Point(20, 20);
        pnlCard.Name = "pnlCard";
        pnlCard.Padding = new System.Windows.Forms.Padding(28);
        pnlCard.Size = new System.Drawing.Size(552, 304);
        pnlCard.TabIndex = 0;
        pnlCard.Paint += PnlCard_Paint;
        // 
        // lblRangoTitle
        // 
        lblRangoTitle.AutoSize = true;
        lblRangoTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
        lblRangoTitle.ForeColor = System.Drawing.Color.FromArgb(30, 41, 59);
        lblRangoTitle.Location = new System.Drawing.Point(28, 22);
        lblRangoTitle.Name = "lblRangoTitle";
        lblRangoTitle.Size = new System.Drawing.Size(225, 19);
        lblRangoTitle.TabIndex = 0;
        lblRangoTitle.Text = "Período de Análisis de Tickets";
        // 
        // lblInicio
        // 
        lblInicio.AutoSize = true;
        lblInicio.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        lblInicio.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
        lblInicio.Location = new System.Drawing.Point(28, 66);
        lblInicio.Name = "lblInicio";
        lblInicio.Size = new System.Drawing.Size(78, 15);
        lblInicio.TabIndex = 1;
        lblInicio.Text = "Fecha Inicial:";
        // 
        // dtpInicio
        // 
        dtpInicio.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        dtpInicio.Font = new System.Drawing.Font("Segoe UI", 10F);
        dtpInicio.Format = System.Windows.Forms.DateTimePickerFormat.Short;
        dtpInicio.Location = new System.Drawing.Point(28, 87);
        dtpInicio.Name = "dtpInicio";
        dtpInicio.Size = new System.Drawing.Size(496, 34);
        dtpInicio.TabIndex = 2;
        // 
        // lblFin
        // 
        lblFin.AutoSize = true;
        lblFin.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        lblFin.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
        lblFin.Location = new System.Drawing.Point(28, 132);
        lblFin.Name = "lblFin";
        lblFin.Size = new System.Drawing.Size(72, 15);
        lblFin.TabIndex = 3;
        lblFin.Text = "Fecha Final:";
        // 
        // dtpFin
        // 
        dtpFin.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        dtpFin.Font = new System.Drawing.Font("Segoe UI", 10F);
        dtpFin.Format = System.Windows.Forms.DateTimePickerFormat.Short;
        dtpFin.Location = new System.Drawing.Point(28, 153);
        dtpFin.Name = "dtpFin";
        dtpFin.Size = new System.Drawing.Size(496, 34);
        dtpFin.TabIndex = 4;
        // 
        // flpBotones
        // 
        flpBotones.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        flpBotones.Controls.Add(btnExcel);
        flpBotones.Controls.Add(btnPdf);
        flpBotones.Location = new System.Drawing.Point(28, 216);
        flpBotones.Name = "flpBotones";
        flpBotones.Size = new System.Drawing.Size(496, 50);
        flpBotones.TabIndex = 5;
        // 
        // btnExcel
        // 
        btnExcel.Cursor = System.Windows.Forms.Cursors.Hand;
        btnExcel.Estilo = HSis.UI.Controls.EstiloBotonModerno.Exito;
        btnExcel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
        btnExcel.Icono = FontAwesome.Sharp.IconChar.FileExcel;
        btnExcel.IconoTamano = 18;
        btnExcel.Location = new System.Drawing.Point(0, 0);
        btnExcel.Margin = new System.Windows.Forms.Padding(0, 0, 14, 0);
        btnExcel.Name = "btnExcel";
        btnExcel.RadioBorde = 8;
        btnExcel.Size = new System.Drawing.Size(216, 44);
        btnExcel.TabIndex = 0;
        btnExcel.Text = "Exportar a Excel";
        btnExcel.Click += btnExcel_Click;
        // 
        // btnPdf
        // 
        btnPdf.Cursor = System.Windows.Forms.Cursors.Hand;
        btnPdf.Estilo = HSis.UI.Controls.EstiloBotonModerno.Primario;
        btnPdf.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
        btnPdf.Icono = FontAwesome.Sharp.IconChar.FilePdf;
        btnPdf.IconoTamano = 18;
        btnPdf.Location = new System.Drawing.Point(230, 0);
        btnPdf.Margin = new System.Windows.Forms.Padding(0);
        btnPdf.Name = "btnPdf";
        btnPdf.RadioBorde = 8;
        btnPdf.Size = new System.Drawing.Size(216, 44);
        btnPdf.TabIndex = 1;
        btnPdf.Text = "Exportar a PDF";
        btnPdf.Click += btnPdf_Click;
        // 
        // GeneradorReportesForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
        ClientSize = new System.Drawing.Size(600, 440);
        Controls.Add(pnlBody);
        Controls.Add(pnlHeader);
        Font = new System.Drawing.Font("Segoe UI", 9F);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        MinimumSize = new System.Drawing.Size(600, 440);
        Name = "GeneradorReportesForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        Text = "Generar Reportes Especializados";
        Load += frmGeneradorReportes_Load;
        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picHeaderIcon).EndInit();
        pnlBody.ResumeLayout(false);
        pnlCard.ResumeLayout(false);
        pnlCard.PerformLayout();
        flpBotones.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private System.Windows.Forms.Panel pnlHeader;
    private System.Windows.Forms.PictureBox picHeaderIcon;
    private System.Windows.Forms.Label lblTitle;
    private System.Windows.Forms.Label lblSubtitle;
    private System.Windows.Forms.Panel pnlBody;
    private System.Windows.Forms.Panel pnlCard;
    private System.Windows.Forms.Label lblRangoTitle;
    private System.Windows.Forms.Label lblInicio;
    private HSis.UI.Controls.SelectorFechaModerno dtpInicio;
    private System.Windows.Forms.Label lblFin;
    private HSis.UI.Controls.SelectorFechaModerno dtpFin;
    private System.Windows.Forms.FlowLayoutPanel flpBotones;
    private HSis.UI.Controls.BotonModerno btnExcel;
    private HSis.UI.Controls.BotonModerno btnPdf;
}
