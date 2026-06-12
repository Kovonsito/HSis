using System;
using System.Drawing;
using System.Windows.Forms;

namespace HSis.UI.Forms.Dashboards
{
    partial class DashboardAdminForm
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
            lblTitulo = new Label();
            ucNuevos = new HSis.UI.Controls.IndicadorControl();
            ucUrgentes = new HSis.UI.Controls.IndicadorControl();
            ucEnProceso = new HSis.UI.Controls.IndicadorControl();
            ucCerrados = new HSis.UI.Controls.IndicadorControl();
            dgvTickets = new DataGridView();
            btnRecargar = new Button();
            ucReabiertos = new HSis.UI.Controls.IndicadorControl();
            tabMain = new TabControl();
            tabTickets = new TabPage();
            pnlFiltros = new Panel();
            lblFiltrosTitle = new Label();
            filtroGenerico = new HSis.UI.Controls.FiltroGenericoControl();
            btnLimpiarFiltros = new Button();
            btnAbrirReportes = new Button();

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
            ucNuevos.IndicadorClic += ucNuevos_ucIndicadorEvent;
            // 
            // ucUrgentes
            // 
            ucUrgentes.Location = new Point(218, 32);
            ucUrgentes.Name = "ucUrgentes";
            ucUrgentes.Size = new Size(200, 100);
            ucUrgentes.TabIndex = 1;
            ucUrgentes.IndicadorClic += ucUrgentes_ucIndicadorEvent;
            // 
            // ucEnProceso
            // 
            ucEnProceso.Location = new Point(424, 32);
            ucEnProceso.Name = "ucEnProceso";
            ucEnProceso.Size = new Size(200, 100);
            ucEnProceso.TabIndex = 2;
            ucEnProceso.IndicadorClic += ucEnProceso_ucIndicadorEvent;
            // 
            // ucCerrados
            // 
            ucCerrados.Location = new Point(630, 32);
            ucCerrados.Name = "ucCerrados";
            ucCerrados.Size = new Size(200, 100);
            ucCerrados.TabIndex = 3;
            ucCerrados.IndicadorClic += ucCerrados_ucIndicadorEvent;
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
            // lblTitulo
            // 
            lblTitulo.AutoSize = true;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.Location = new Point(12, 9);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(400, 32);
            lblTitulo.TabIndex = 9;
            lblTitulo.Text = "Panel de Control - Administrador";
            // 
            // btnRecargar
            // 
            btnRecargar.Location = new Point(580, 80);
            btnRecargar.Name = "btnRecargar";
            btnRecargar.Size = new Size(130, 26);
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
            ucReabiertos.IndicadorClic += ucReabiertos_ucIndicadorEvent;
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
            tabTickets.Controls.Add(lblTitulo);
            tabTickets.Controls.Add(pnlFiltros);
            tabTickets.Controls.Add(ucReabiertos);
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
            pnlFiltros.Controls.Add(filtroGenerico);
            pnlFiltros.Controls.Add(btnLimpiarFiltros);
            pnlFiltros.Controls.Add(btnAbrirReportes);
            pnlFiltros.Controls.Add(btnRecargar);
            pnlFiltros.Location = new Point(12, 145);
            pnlFiltros.Name = "pnlFiltros";
            pnlFiltros.Size = new Size(1024, 115);
            pnlFiltros.TabIndex = 8;
            pnlFiltros.BackColor = Color.White;
            // 
            // filtroGenerico
            // 
            filtroGenerico.Location = new Point(10, 25);
            filtroGenerico.Name = "filtroGenerico";
            filtroGenerico.Size = new Size(1004, 52);
            filtroGenerico.TabIndex = 6;
            filtroGenerico.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
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
            // DashboardAdminForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1100, 700);
            Controls.Add(tabMain);
            MinimumSize = new Size(1030, 680);
            Name = "DashboardAdminForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "DashboardAdmin";
            Load += DashboardAdmin_Load;
            ((System.ComponentModel.ISupportInitialize)dgvTickets).EndInit();
            tabMain.ResumeLayout(false);
            tabTickets.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Label lblTitulo;
        private HSis.UI.Controls.IndicadorControl ucNuevos;
        private HSis.UI.Controls.IndicadorControl ucUrgentes;
        private HSis.UI.Controls.IndicadorControl ucEnProceso;
        private HSis.UI.Controls.IndicadorControl ucCerrados;
        private DataGridView dgvTickets;
        private Button btnRecargar;
        private HSis.UI.Controls.IndicadorControl ucReabiertos;
        private TabControl tabMain;
        private TabPage tabTickets;
        private Panel pnlFiltros;
        private Label lblFiltrosTitle;
        private HSis.UI.Controls.FiltroGenericoControl filtroGenerico;
        private Button btnLimpiarFiltros;
        private Button btnAbrirReportes;

    }
}