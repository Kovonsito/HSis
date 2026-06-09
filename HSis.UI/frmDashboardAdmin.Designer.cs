namespace HSis.UI
{
    partial class frmDashboardAdmin
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
            ucNuevos = new ucIndicador();
            ucUrgentes = new ucIndicador();
            ucEnProceso = new ucIndicador();
            ucCerrados = new ucIndicador();
            dgvTickets = new DataGridView();
            btnRecargar = new Button();
            ucReabiertos = new ucIndicador();
            tabMain = new TabControl();
            tabTickets = new TabPage();
            pnlFiltros = new Panel();
            lblFiltrosTitle = new Label();
            lblFiltroEstatus = new Label();
            cmbFiltroEstatus = new ComboBox();
            lblFiltroPrioridad = new Label();
            cmbFiltroPrioridad = new ComboBox();
            lblFiltroTecnico = new Label();
            cmbFiltroTecnico = new ComboBox();
            lblFiltroUsuario = new Label();
            txtFiltroUsuario = new TextBox();
            lblFiltroTemporal = new Label();
            cmbFiltroTemporal = new ComboBox();
            btnLimpiarFiltros = new Button();
            btnAbrirReportes = new Button();
            lblFechaInicio = new Label();
            dtpFechaInicio = new DateTimePicker();
            lblFechaFin = new Label();
            dtpFechaFin = new DateTimePicker();

            ((System.ComponentModel.ISupportInitialize)dgvTickets).BeginInit();
            tabMain.SuspendLayout();
            tabTickets.SuspendLayout();
            pnlFiltros.SuspendLayout();
            SuspendLayout();
            // 
            // ucNuevos
            // 
            ucNuevos.Location = new Point(12, 32);
            ucNuevos.Name = "ucNuevos";
            ucNuevos.Size = new Size(200, 100);
            ucNuevos.TabIndex = 0;
            ucNuevos.ucIndicadorEvent += ucNuevos_ucIndicadorEvent;
            // 
            // ucUrgentes
            // 
            ucUrgentes.Location = new Point(218, 32);
            ucUrgentes.Name = "ucUrgentes";
            ucUrgentes.Size = new Size(200, 100);
            ucUrgentes.TabIndex = 1;
            ucUrgentes.ucIndicadorEvent += ucUrgentes_ucIndicadorEvent;
            // 
            // ucEnProceso
            // 
            ucEnProceso.Location = new Point(424, 32);
            ucEnProceso.Name = "ucEnProceso";
            ucEnProceso.Size = new Size(200, 100);
            ucEnProceso.TabIndex = 2;
            ucEnProceso.ucIndicadorEvent += ucEnProceso_ucIndicadorEvent;
            // 
            // ucCerrados
            // 
            ucCerrados.Location = new Point(630, 32);
            ucCerrados.Name = "ucCerrados";
            ucCerrados.Size = new Size(200, 100);
            ucCerrados.TabIndex = 3;
            ucCerrados.ucIndicadorEvent += ucCerrados_ucIndicadorEvent;
            // 
            // dgvTickets
            // 
            dgvTickets.AllowUserToAddRows = false;
            dgvTickets.AllowUserToDeleteRows = false;
            dgvTickets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTickets.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvTickets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgvTickets.Location = new Point(12, 297);
            dgvTickets.MultiSelect = false;
            dgvTickets.Name = "dgvTickets";
            dgvTickets.ReadOnly = true;
            dgvTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTickets.Size = new Size(1024, 248);
            dgvTickets.TabIndex = 4;
            dgvTickets.CellDoubleClick += dgvTickets_CellDoubleClick;
            // 
            // btnRecargar
            // 
            btnRecargar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRecargar.Location = new Point(906, 268);
            btnRecargar.Name = "btnRecargar";
            btnRecargar.Size = new Size(130, 25);
            btnRecargar.TabIndex = 5;
            btnRecargar.Text = "Recargar tabla";
            btnRecargar.UseVisualStyleBackColor = true;
            btnRecargar.Click += btnRecargar_Click;
            // 
            // ucReabiertos
            // 
            ucReabiertos.Location = new Point(836, 32);
            ucReabiertos.Name = "ucReabiertos";
            ucReabiertos.Size = new Size(200, 100);
            ucReabiertos.TabIndex = 6;
            ucReabiertos.ucIndicadorEvent += ucReabiertos_ucIndicadorEvent;
            // 
            // tabMain
            // 
            tabMain.Controls.Add(tabTickets);
            tabMain.Dock = DockStyle.Fill;
            tabMain.Location = new Point(0, 0);
            tabMain.Name = "tabMain";
            tabMain.SelectedIndex = 0;
            tabMain.Size = new Size(1049, 617);
            tabMain.TabIndex = 7;
            // 
            // tabTickets
            // 
            tabTickets.Controls.Add(pnlFiltros);
            tabTickets.Controls.Add(ucReabiertos);
            tabTickets.Controls.Add(btnRecargar);
            tabTickets.Controls.Add(dgvTickets);
            tabTickets.Controls.Add(ucCerrados);
            tabTickets.Controls.Add(ucEnProceso);
            tabTickets.Controls.Add(ucUrgentes);
            tabTickets.Controls.Add(ucNuevos);
            tabTickets.Location = new Point(4, 24);
            tabTickets.Name = "tabTickets";
            tabTickets.Padding = new Padding(3);
            tabTickets.Size = new Size(1041, 589);
            tabTickets.TabIndex = 0;
            tabTickets.Text = "Tickets";
            tabTickets.UseVisualStyleBackColor = true;
            tabTickets.AutoScroll = true;
            // 
            // pnlFiltros
            // 
            pnlFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            pnlFiltros.BorderStyle = BorderStyle.FixedSingle;
            pnlFiltros.AutoScroll = true;
            pnlFiltros.Controls.Add(lblFiltrosTitle);
            pnlFiltros.Controls.Add(lblFiltroEstatus);
            pnlFiltros.Controls.Add(cmbFiltroEstatus);
            pnlFiltros.Controls.Add(lblFiltroPrioridad);
            pnlFiltros.Controls.Add(cmbFiltroPrioridad);
            pnlFiltros.Controls.Add(lblFiltroTecnico);
            pnlFiltros.Controls.Add(cmbFiltroTecnico);
            pnlFiltros.Controls.Add(lblFiltroUsuario);
            pnlFiltros.Controls.Add(txtFiltroUsuario);
            pnlFiltros.Controls.Add(lblFiltroTemporal);
            pnlFiltros.Controls.Add(cmbFiltroTemporal);
            pnlFiltros.Controls.Add(btnLimpiarFiltros);
            pnlFiltros.Controls.Add(btnAbrirReportes);
            pnlFiltros.Controls.Add(lblFechaInicio);
            pnlFiltros.Controls.Add(dtpFechaInicio);
            pnlFiltros.Controls.Add(lblFechaFin);
            pnlFiltros.Controls.Add(dtpFechaFin);
            pnlFiltros.Location = new Point(12, 145);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(1024, 115);
            pnlFiltros.TabIndex = 8;
            pnlFiltros.BackColor = Color.White;
            // 
            // lblFechaInicio
            // 
            lblFechaInicio.AutoSize = true;
            lblFechaInicio.Font = new Font("Segoe UI", 9F);
            lblFechaInicio.Location = new Point(10, 85);
            lblFechaInicio.Name = "lblFechaInicio";
            lblFechaInicio.Size = new Size(42, 15);
            lblFechaInicio.Text = "Desde:";
            // 
            // dtpFechaInicio
            // 
            dtpFechaInicio.Format = DateTimePickerFormat.Short;
            dtpFechaInicio.Font = new Font("Segoe UI", 9F);
            dtpFechaInicio.Location = new Point(58, 81);
            dtpFechaInicio.Name = "dtpFechaInicio";
            dtpFechaInicio.Size = new Size(130, 23);
            dtpFechaInicio.TabIndex = 7;
            // 
            // lblFechaFin
            // 
            lblFechaFin.AutoSize = true;
            lblFechaFin.Font = new Font("Segoe UI", 9F);
            lblFechaFin.Location = new Point(205, 85);
            lblFechaFin.Name = "lblFechaFin";
            lblFechaFin.Size = new Size(40, 15);
            lblFechaFin.Text = "Hasta:";
            // 
            // dtpFechaFin
            // 
            dtpFechaFin.Format = DateTimePickerFormat.Short;
            dtpFechaFin.Font = new Font("Segoe UI", 9F);
            dtpFechaFin.Location = new Point(250, 81);
            dtpFechaFin.Name = "dtpFechaFin";
            dtpFechaFin.Size = new Size(130, 23);
            dtpFechaFin.TabIndex = 8;

            // 
            // lblFiltrosTitle
            // 
            lblFiltrosTitle.AutoSize = true;
            lblFiltrosTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblFiltrosTitle.ForeColor = Color.FromArgb(31, 41, 55);
            lblFiltrosTitle.Location = new Point(10, 5);
            lblFiltrosTitle.Name = "lblFiltrosTitle";
            lblFiltrosTitle.Size = new Size(180, 19);
            lblFiltrosTitle.Text = "Filtros de Búsqueda Rápida";
            // 
            // lblFiltroEstatus
            // 
            lblFiltroEstatus.AutoSize = true;
            lblFiltroEstatus.Font = new Font("Segoe UI", 9F);
            lblFiltroEstatus.Location = new Point(10, 30);
            lblFiltroEstatus.Name = "lblFiltroEstatus";
            lblFiltroEstatus.Text = "Estatus:";
            // 
            // cmbFiltroEstatus
            // 
            cmbFiltroEstatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroEstatus.Font = new Font("Segoe UI", 9F);
            cmbFiltroEstatus.Location = new Point(10, 50);
            cmbFiltroEstatus.Name = "cmbFiltroEstatus";
            cmbFiltroEstatus.Size = new Size(150, 23);
            cmbFiltroEstatus.SelectedIndexChanged += cmbFiltroEstatus_SelectedIndexChanged;
            // 
            // lblFiltroPrioridad
            // 
            lblFiltroPrioridad.AutoSize = true;
            lblFiltroPrioridad.Font = new Font("Segoe UI", 9F);
            lblFiltroPrioridad.Location = new Point(180, 30);
            lblFiltroPrioridad.Name = "lblFiltroPrioridad";
            lblFiltroPrioridad.Text = "Prioridad:";
            // 
            // cmbFiltroPrioridad
            // 
            cmbFiltroPrioridad.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroPrioridad.Font = new Font("Segoe UI", 9F);
            cmbFiltroPrioridad.Location = new Point(180, 50);
            cmbFiltroPrioridad.Name = "cmbFiltroPrioridad";
            cmbFiltroPrioridad.Size = new Size(150, 23);
            cmbFiltroPrioridad.SelectedIndexChanged += cmbFiltroPrioridad_SelectedIndexChanged;
            // 
            // lblFiltroTecnico
            // 
            lblFiltroTecnico.AutoSize = true;
            lblFiltroTecnico.Font = new Font("Segoe UI", 9F);
            lblFiltroTecnico.Location = new Point(350, 30);
            lblFiltroTecnico.Name = "lblFiltroTecnico";
            lblFiltroTecnico.Text = "Técnico:";
            // 
            // cmbFiltroTecnico
            // 
            cmbFiltroTecnico.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroTecnico.Font = new Font("Segoe UI", 9F);
            cmbFiltroTecnico.Location = new Point(350, 50);
            cmbFiltroTecnico.Name = "cmbFiltroTecnico";
            cmbFiltroTecnico.Size = new Size(180, 23);
            cmbFiltroTecnico.SelectedIndexChanged += cmbFiltroTecnico_SelectedIndexChanged;
            // 
            // lblFiltroUsuario
            // 
            lblFiltroUsuario.AutoSize = true;
            lblFiltroUsuario.Font = new Font("Segoe UI", 9F);
            lblFiltroUsuario.Location = new Point(550, 30);
            lblFiltroUsuario.Name = "lblFiltroUsuario";
            lblFiltroUsuario.Text = "Usuario Emisor:";
            // 
            // txtFiltroUsuario
            // 
            txtFiltroUsuario.Font = new Font("Segoe UI", 9F);
            txtFiltroUsuario.Location = new Point(550, 50);
            txtFiltroUsuario.Name = "txtFiltroUsuario";
            txtFiltroUsuario.Size = new Size(180, 23);
            txtFiltroUsuario.TextChanged += txtFiltroUsuario_TextChanged;
            // 
            // lblFiltroTemporal
            // 
            lblFiltroTemporal.AutoSize = true;
            lblFiltroTemporal.Font = new Font("Segoe UI", 9F);
            lblFiltroTemporal.Location = new Point(750, 30);
            lblFiltroTemporal.Name = "lblFiltroTemporal";
            lblFiltroTemporal.Text = "Vista Temporal:";
            // 
            // cmbFiltroTemporal
            // 
            cmbFiltroTemporal.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbFiltroTemporal.Font = new Font("Segoe UI", 9F);
            cmbFiltroTemporal.Location = new Point(750, 50);
            cmbFiltroTemporal.Name = "cmbFiltroTemporal";
            cmbFiltroTemporal.Size = new Size(150, 23);
            cmbFiltroTemporal.SelectedIndexChanged += cmbFiltroTemporal_SelectedIndexChanged;
            // 
            // btnLimpiarFiltros
            // 
            btnLimpiarFiltros.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnLimpiarFiltros.Location = new Point(720, 80);
            btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            btnLimpiarFiltros.Size = new Size(130, 26);
            btnLimpiarFiltros.Text = "Limpiar Filtros";
            btnLimpiarFiltros.Click += btnLimpiarFiltros_Click;
            // 
            // btnAbrirReportes
            // 
            btnAbrirReportes.BackColor = Color.FromArgb(37, 99, 235);
            btnAbrirReportes.FlatStyle = FlatStyle.Flat;
            btnAbrirReportes.ForeColor = Color.White;
            btnAbrirReportes.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnAbrirReportes.Location = new Point(860, 80);
            btnAbrirReportes.Name = "btnAbrirReportes";
            btnAbrirReportes.Size = new Size(150, 26);
            btnAbrirReportes.Text = "Generar Reportes";
            btnAbrirReportes.UseVisualStyleBackColor = false;
            btnAbrirReportes.Click += btnAbrirReportes_Click;
            // 
            // frmDashboardAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1049, 617);
            Controls.Add(tabMain);
            MinimumSize = new Size(680, 600);
            Name = "frmDashboardAdmin";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DashboardAdmin";
            Load += DashboardAdmin_Load;
            ((System.ComponentModel.ISupportInitialize)dgvTickets).EndInit();
            tabMain.ResumeLayout(false);
            tabTickets.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private ucIndicador ucNuevos;
        private ucIndicador ucUrgentes;
        private ucIndicador ucEnProceso;
        private ucIndicador ucCerrados;
        private DataGridView dgvTickets;
        private Button btnRecargar;
        private ucIndicador ucReabiertos;
        private TabControl tabMain;
        private TabPage tabTickets;
        private Panel pnlFiltros;
        private Label lblFiltrosTitle;
        private Label lblFiltroEstatus;
        private ComboBox cmbFiltroEstatus;
        private Label lblFiltroPrioridad;
        private ComboBox cmbFiltroPrioridad;
        private Label lblFiltroTecnico;
        private ComboBox cmbFiltroTecnico;
        private Label lblFiltroUsuario;
        private TextBox txtFiltroUsuario;
        private Label lblFiltroTemporal;
        private ComboBox cmbFiltroTemporal;
        private Button btnLimpiarFiltros;
        private Button btnAbrirReportes;
        private Label lblFechaInicio;
        private DateTimePicker dtpFechaInicio;
        private Label lblFechaFin;
        private DateTimePicker dtpFechaFin;

    }
}