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
        lblMaterial = new System.Windows.Forms.Label();
        cbMaterial = new System.Windows.Forms.ComboBox();
        dgvKardex = new System.Windows.Forms.DataGridView();
        panelTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)picIcon).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvKardex).BeginInit();
        SuspendLayout();
        // 
        // panelTop
        // 
        panelTop.BackColor = System.Drawing.Color.White;
        panelTop.Controls.Add(picIcon);
        panelTop.Controls.Add(lblTituloHeader);
        panelTop.Controls.Add(lblMaterial);
        panelTop.Controls.Add(cbMaterial);
        panelTop.Dock = System.Windows.Forms.DockStyle.Top;
        panelTop.Height = 64;
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
        lblTituloHeader.Location = new System.Drawing.Point(58, 20);
        lblTituloHeader.Name = "lblTituloHeader";
        lblTituloHeader.Size = new System.Drawing.Size(252, 21);
        lblTituloHeader.TabIndex = 1;
        lblTituloHeader.Text = "Kardex / Historial de Inventario";
        // 
        // lblMaterial
        // 
        lblMaterial.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        lblMaterial.AutoSize = true;
        lblMaterial.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
        lblMaterial.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
        lblMaterial.Location = new System.Drawing.Point(400, 24);
        lblMaterial.Name = "lblMaterial";
        lblMaterial.Size = new System.Drawing.Size(120, 15);
        lblMaterial.TabIndex = 2;
        lblMaterial.Text = "Filtrar por Material:";
        // 
        // cbMaterial
        // 
        cbMaterial.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
        cbMaterial.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        cbMaterial.Font = new System.Drawing.Font("Segoe UI", 9.5F);
        cbMaterial.Location = new System.Drawing.Point(525, 19);
        cbMaterial.Name = "cbMaterial";
        cbMaterial.Size = new System.Drawing.Size(275, 25);
        cbMaterial.TabIndex = 3;
        cbMaterial.SelectedIndexChanged += CbMaterial_SelectedIndexChanged;
        // 
        // dgvKardex
        // 
        dgvKardex.AllowUserToAddRows = false;
        dgvKardex.AllowUserToDeleteRows = false;
        dgvKardex.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
        dgvKardex.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvKardex.Dock = System.Windows.Forms.DockStyle.Fill;
        dgvKardex.Location = new System.Drawing.Point(0, 64);
        dgvKardex.Name = "dgvKardex";
        dgvKardex.ReadOnly = true;
        dgvKardex.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        dgvKardex.Size = new System.Drawing.Size(820, 456);
        dgvKardex.TabIndex = 1;
        // 
        // KardexForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        BackColor = System.Drawing.Color.FromArgb(248, 250, 252);
        ClientSize = new System.Drawing.Size(820, 520);
        Controls.Add(dgvKardex);
        Controls.Add(panelTop);
        Font = new System.Drawing.Font("Segoe UI", 9F);
        MinimumSize = new System.Drawing.Size(800, 500);
        Name = "KardexForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
        Text = "Kardex / Historial de Inventario";
        Load += FrmKardex_Load;
        panelTop.ResumeLayout(false);
        panelTop.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)picIcon).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvKardex).EndInit();
        ResumeLayout(false);
    }

    private System.Windows.Forms.Panel panelTop;
    private System.Windows.Forms.PictureBox picIcon;
    private System.Windows.Forms.Label lblTituloHeader;
    private System.Windows.Forms.Label lblMaterial;
    private System.Windows.Forms.ComboBox cbMaterial;
    private System.Windows.Forms.DataGridView dgvKardex;
}
