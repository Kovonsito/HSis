namespace HSis.UI.Forms.Otros;

partial class KardexForm
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
        panelTop = new System.Windows.Forms.Panel();
        picIcon = new System.Windows.Forms.PictureBox();
        lblTituloHeader = new System.Windows.Forms.Label();
        lblSubtituloHeader = new System.Windows.Forms.Label();
        lblMaterial = new System.Windows.Forms.Label();
        cbMaterial = new HSis.UI.Controls.ComboModerno();
        pnlContenido = new System.Windows.Forms.Panel();
        dgvKardex = new System.Windows.Forms.DataGridView();
        lblEstadoKardex = new System.Windows.Forms.Label();
        panelTop.SuspendLayout();
        pnlContenido.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvKardex).BeginInit();
        SuspendLayout();
        // 
        // panelTop
        // 
        panelTop.BackColor = System.Drawing.Color.White;
        panelTop.Controls.Add(picIcon);
        panelTop.Controls.Add(lblTituloHeader);
        panelTop.Controls.Add(lblSubtituloHeader);
        panelTop.Controls.Add(lblMaterial);
        panelTop.Controls.Add(cbMaterial);
        panelTop.Dock = System.Windows.Forms.DockStyle.Top;
        panelTop.Height = 112;
        panelTop.Name = "panelTop";
        panelTop.Paint += PanelTop_Paint;
        // 
        // picIcon
        // 
        picIcon.Location = new System.Drawing.Point(16, 14);
        picIcon.Name = "picIcon";
        picIcon.Size = new System.Drawing.Size(36, 36);
        picIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
        picIcon.TabIndex = 0;
        picIcon.TabStop = false;
        // 
        // lblTituloHeader
        // 
        lblTituloHeader.AutoSize = true;
        lblTituloHeader.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
        lblTituloHeader.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
        lblTituloHeader.Location = new System.Drawing.Point(64, 13);
        lblTituloHeader.Name = "lblTituloHeader";
        lblTituloHeader.Size = new System.Drawing.Size(300, 21);
        lblTituloHeader.TabIndex = 1;
        lblTituloHeader.Text = "Kardex / Historial de Inventario";
        //
        // lblSubtituloHeader
        //
        lblSubtituloHeader.AutoSize = false;
        lblSubtituloHeader.Font = new System.Drawing.Font("Segoe UI", 8.5F);
        lblSubtituloHeader.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
        lblSubtituloHeader.Location = new System.Drawing.Point(66, 39);
        lblSubtituloHeader.Name = "lblSubtituloHeader";
        lblSubtituloHeader.Size = new System.Drawing.Size(360, 18);
        lblSubtituloHeader.TabIndex = 2;
        lblSubtituloHeader.Text = "Consulta los movimientos registrados por material.";
        //
        // lblMaterial
        //
        lblMaterial.AutoSize = true;
        lblMaterial.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        lblMaterial.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
        lblMaterial.Location = new System.Drawing.Point(16, 80);
        lblMaterial.Name = "lblMaterial";
        lblMaterial.Size = new System.Drawing.Size(58, 15);
        lblMaterial.TabIndex = 3;
        lblMaterial.Text = "Material:";
        //
        // cbMaterial
        //
        cbMaterial.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
        cbMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cbMaterial.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        cbMaterial.Location = new System.Drawing.Point(88, 70);
        cbMaterial.Name = "cbMaterial";
        cbMaterial.Size = new System.Drawing.Size(716, 34);
        cbMaterial.TabIndex = 4;
        cbMaterial.SelectedIndexChanged += CbMaterial_SelectedIndexChanged;
        //
        // pnlContenido
        //
        pnlContenido.BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
        pnlContenido.Controls.Add(dgvKardex);
        pnlContenido.Controls.Add(lblEstadoKardex);
        pnlContenido.Dock = System.Windows.Forms.DockStyle.Fill;
        pnlContenido.Location = new System.Drawing.Point(0, 112);
        pnlContenido.Name = "pnlContenido";
        pnlContenido.Padding = new System.Windows.Forms.Padding(16);
        pnlContenido.Size = new System.Drawing.Size(820, 408);
        pnlContenido.TabIndex = 1;
        //
        // dgvKardex
        //
        dgvKardex.AllowUserToAddRows = false;
        dgvKardex.AllowUserToDeleteRows = false;
        dgvKardex.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        dgvKardex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvKardex.Dock = System.Windows.Forms.DockStyle.Fill;
        dgvKardex.Location = new System.Drawing.Point(16, 16);
        dgvKardex.Name = "dgvKardex";
        dgvKardex.ReadOnly = true;
        dgvKardex.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        dgvKardex.Size = new System.Drawing.Size(788, 376);
        dgvKardex.TabIndex = 0;
        //
        // lblEstadoKardex
        //
        lblEstadoKardex.BackColor = System.Drawing.Color.White;
        lblEstadoKardex.Dock = System.Windows.Forms.DockStyle.Fill;
        lblEstadoKardex.Font = new System.Drawing.Font("Segoe UI", 10F);
        lblEstadoKardex.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
        lblEstadoKardex.Location = new System.Drawing.Point(16, 16);
        lblEstadoKardex.Name = "lblEstadoKardex";
        lblEstadoKardex.Padding = new System.Windows.Forms.Padding(24);
        lblEstadoKardex.Size = new System.Drawing.Size(788, 376);
        lblEstadoKardex.TabIndex = 1;
        lblEstadoKardex.Text = "Selecciona un material para consultar sus movimientos.";
        lblEstadoKardex.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
        //
        // KardexForm
        //
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
        ClientSize = new System.Drawing.Size(820, 560);
        Controls.Add(pnlContenido);
        Controls.Add(panelTop);
        Font = new System.Drawing.Font("Segoe UI", 9F);
        FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        MinimumSize = new System.Drawing.Size(760, 480);
        Name = "KardexForm";
        ShowInTaskbar = true;
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        Text = "Kardex / Historial de Inventario";
        Load += FrmKardex_Load;
        panelTop.ResumeLayout(false);
        panelTop.PerformLayout();
        pnlContenido.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvKardex).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Panel panelTop;
    private System.Windows.Forms.PictureBox picIcon;
    private System.Windows.Forms.Label lblTituloHeader;
    private System.Windows.Forms.Label lblSubtituloHeader;
    private System.Windows.Forms.Label lblMaterial;
    private HSis.UI.Controls.ComboModerno cbMaterial;
    private System.Windows.Forms.Panel pnlContenido;
    private System.Windows.Forms.DataGridView dgvKardex;
    private System.Windows.Forms.Label lblEstadoKardex;
}
