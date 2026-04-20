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
            ((System.ComponentModel.ISupportInitialize)dgvTickets).BeginInit();
            tabMain.SuspendLayout();
            tabTickets.SuspendLayout();
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
            dgvTickets.EditMode = DataGridViewEditMode.EditProgrammatically;
            dgvTickets.Location = new Point(12, 297);
            dgvTickets.MultiSelect = false;
            dgvTickets.Name = "dgvTickets";
            dgvTickets.ReadOnly = true;
            dgvTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTickets.Size = new Size(1024, 308);
            dgvTickets.TabIndex = 4;
            dgvTickets.CellDoubleClick += dgvTickets_CellDoubleClick;
            // 
            // btnRecargar
            // 
            btnRecargar.Location = new Point(932, 268);
            btnRecargar.Name = "btnRecargar";
            btnRecargar.Size = new Size(104, 23);
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
            // 
            // frmDashboardAdmin
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1049, 617);
            Controls.Add(tabMain);
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
    }
}