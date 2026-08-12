using System;
using System.Drawing;
using System.Windows.Forms;
using HSis.UI.Controls;
using HSis.UI.Helpers;

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
            btnNuevoTicket = new Button();
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
            ucNuevos.IndicadorClic += UcNuevosUcIndicadorEvent;
            // 
            // ucUrgentes
            // 
            ucUrgentes.Location = new Point(218, 32);
            ucUrgentes.Name = "ucUrgentes";
            ucUrgentes.Size = new Size(200, 100);
            ucUrgentes.TabIndex = 1;
            ucUrgentes.IndicadorClic += UcUrgentes_ucIndicadorEvent;
            // 
            // ucEnProceso
            // 
            ucEnProceso.Location = new Point(424, 32);
            ucEnProceso.Name = "ucEnProceso";
            ucEnProceso.Size = new Size(200, 100);
            ucEnProceso.TabIndex = 2;
            ucEnProceso.IndicadorClic += UcEnProceso_ucIndicadorEvent;
            // 
            // ucCerrados
            // 
            ucCerrados.Location = new Point(630, 32);
            ucCerrados.Name = "ucCerrados";
            ucCerrados.Size = new Size(200, 100);
            ucCerrados.TabIndex = 3;
            ucCerrados.IndicadorClic += UcCerrados_ucIndicadorEvent;
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
            // btnNuevoTicket
            // 
            btnNuevoTicket.BackColor = Color.FromArgb(16, 185, 129);
            btnNuevoTicket.FlatStyle = FlatStyle.Flat;
            btnNuevoTicket.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            btnNuevoTicket.ForeColor = Color.White;
            btnNuevoTicket.Location = new Point(430, 80);
            btnNuevoTicket.Name = "btnNuevoTicket";
            btnNuevoTicket.Size = new Size(140, 26);
            btnNuevoTicket.TabIndex = 4;
            btnNuevoTicket.Text = "+ Registrar Ticket";
            btnNuevoTicket.UseVisualStyleBackColor = false;
            btnNuevoTicket.Click += btnNuevoTicket_Click;
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
            ucReabiertos.IndicadorClic += UcReabiertos_ucIndicadorEvent;
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
            pnlFiltros.Controls.Add(btnNuevoTicket);
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
        private Button btnNuevoTicket;
        private Button btnRecargar;
        private HSis.UI.Controls.IndicadorControl ucReabiertos;
        private TabControl tabMain;
        private TabPage tabTickets;
        private Panel pnlFiltros;
        private Label lblFiltrosTitle;
        private HSis.UI.Controls.FiltroGenericoControl filtroGenerico;
        private Button btnLimpiarFiltros;
        private Button btnAbrirReportes;
        
        private void InicializarLayoutDashboard()
        {
            // Instanciar control de paginación reutilizable
            PaginacionControl = new PaginacionControl
            {
                Dock = DockStyle.Fill
            };
            PaginacionControl.PaginaCambiada += async (s, e) => { if (!_estaCargando) await FiltrarTicketsAsync(); };
            PaginacionControl.Margin = new Padding(12, 0, 12, 6);

            var tblPrincipal = AyudanteDisenoPanel.CrearPanelPrincipal(tabTickets.ClientSize, incluirFiltros: true);
            tblPrincipal.Name = "tblPrincipalTickets";

            _ucCalificacion = new IndicadorControl();
            _ucCalificacion.IndicadorClic += UcCalificacion_Click;
            var tblIndicadores = AyudanteDisenoPanel.CrearPanelIndicadores(
                "tblIndicadoresAdmin",
                6,
                ucNuevos, ucUrgentes, ucEnProceso, ucCerrados, ucReabiertos, _ucCalificacion
            );

            // Configurar otros paneles accesorios
            pnlFiltros.Dock = DockStyle.Fill;
            pnlFiltros.Margin = new Padding(12, 5, 12, 5);

            dgvTickets.Dock = DockStyle.Fill;
            dgvTickets.Margin = new Padding(12, 10, 12, 12);

            // Ensamblar el layout en el grid principal
            tblPrincipal.Controls.Add(lblTitulo, 0, 0);
            tblPrincipal.Controls.Add(tblIndicadores, 0, 1);
            tblPrincipal.Controls.Add(pnlFiltros, 0, 2);
            tblPrincipal.Controls.Add(dgvTickets, 0, 3);
            tblPrincipal.Controls.Add(PaginacionControl, 0, 4);

            // Reubicar controles desde el contenedor original al panel principal
            AyudanteDisenoPanel.ReubicarControles(tabTickets, tblPrincipal, lblTitulo, ucNuevos, ucUrgentes, ucEnProceso, ucCerrados, ucReabiertos, pnlFiltros, dgvTickets);
        }
    }
}
