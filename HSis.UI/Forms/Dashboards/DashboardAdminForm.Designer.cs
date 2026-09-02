using System;
using System.Drawing;
using System.Windows.Forms;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Dashboards;

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
        sidebarAdmin = new HSis.UI.Controls.SidebarControl();
        topBarAdmin = new HSis.UI.Controls.TopBarControl();
        pnlContenedorPrincipal = new Panel();
        lblTitulo = new Label();
        btnNuevoTicket = new HSis.UI.Controls.BotonModerno();
        ucNuevos = new HSis.UI.Controls.IndicadorControl();
        ucUrgentes = new HSis.UI.Controls.IndicadorControl();
        ucEnProceso = new HSis.UI.Controls.IndicadorControl();
        ucCerrados = new HSis.UI.Controls.IndicadorControl();
        dgvTickets = new DataGridView();
        btnRecargar = new HSis.UI.Controls.BotonModerno();
        ucReabiertos = new HSis.UI.Controls.IndicadorControl();
        tabMain = new TabControl();
        tabTickets = new TabPage();
        pnlFiltros = new Panel();
        lblFiltrosTitle = new Label();
        filtroGenerico = new HSis.UI.Controls.FiltroGenericoControl();
        btnLimpiarFiltros = new HSis.UI.Controls.BotonModerno();
        btnAbrirReportes = new HSis.UI.Controls.BotonModerno();

        ((System.ComponentModel.ISupportInitialize)dgvTickets).BeginInit();
        tabMain.SuspendLayout();
        tabTickets.SuspendLayout();
        pnlFiltros.SuspendLayout();
        pnlContenedorPrincipal.SuspendLayout();
        SuspendLayout();
        // 
        // sidebarAdmin
        // 
        sidebarAdmin.Dock = DockStyle.Left;
        sidebarAdmin.Location = new Point(0, 0);
        sidebarAdmin.Name = "sidebarAdmin";
        sidebarAdmin.Size = new Size(240, 720);
        sidebarAdmin.TabIndex = 0;
        // 
        // topBarAdmin
        // 
        topBarAdmin.Dock = DockStyle.Top;
        topBarAdmin.Location = new Point(0, 0);
        topBarAdmin.Name = "topBarAdmin";
        topBarAdmin.Size = new Size(960, 64);
        topBarAdmin.TabIndex = 0;
        topBarAdmin.Titulo = "Panel de Control";
        topBarAdmin.Subtitulo = "Mesa de Servicio y Gestión Global";
        // 
        // pnlContenedorPrincipal
        // 
        pnlContenedorPrincipal.Controls.Add(tabMain);
        pnlContenedorPrincipal.Controls.Add(topBarAdmin);
        pnlContenedorPrincipal.Dock = DockStyle.Fill;
        pnlContenedorPrincipal.Location = new Point(240, 0);
        pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
        pnlContenedorPrincipal.Size = new Size(960, 720);
        pnlContenedorPrincipal.TabIndex = 1;
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
        dgvTickets.Size = new Size(936, 300);
        dgvTickets.TabIndex = 4;
        dgvTickets.CellDoubleClick += dgvTickets_CellDoubleClick;
        // 
        // lblTitulo
        // 
        lblTitulo.AutoSize = true;
        lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        lblTitulo.ForeColor = Color.FromArgb(15, 23, 42);
        lblTitulo.Location = new Point(12, 10);
        lblTitulo.Name = "lblTitulo";
        lblTitulo.Size = new Size(280, 30);
        lblTitulo.TabIndex = 9;
        lblTitulo.Text = "Resumen de Mesa de Ayuda";
        // 
        // btnNuevoTicket
        // 
        btnNuevoTicket.Estilo = EstiloBotonModerno.Exito;
        btnNuevoTicket.Icono = FontAwesome.Sharp.IconChar.Plus;
        btnNuevoTicket.IconoTamano = 14;
        btnNuevoTicket.Location = new Point(410, 75);
        btnNuevoTicket.Name = "btnNuevoTicket";
        btnNuevoTicket.Size = new Size(150, 34);
        btnNuevoTicket.TabIndex = 4;
        btnNuevoTicket.Text = "Nuevo Ticket";
        btnNuevoTicket.Click += btnNuevoTicket_Click;
        // 
        // btnRecargar
        // 
        btnRecargar.Estilo = EstiloBotonModerno.Secundario;
        btnRecargar.Icono = FontAwesome.Sharp.IconChar.RotateRight;
        btnRecargar.IconoTamano = 14;
        btnRecargar.Location = new Point(570, 75);
        btnRecargar.Name = "btnRecargar";
        btnRecargar.Size = new Size(130, 34);
        btnRecargar.TabIndex = 5;
        btnRecargar.Text = "Recargar";
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
        tabMain.Appearance = TabAppearance.FlatButtons;
        tabMain.ItemSize = new Size(0, 1);
        tabMain.SizeMode = TabSizeMode.Fixed;
        tabMain.Controls.Add(tabTickets);
        tabMain.Dock = DockStyle.Fill;
        tabMain.Location = new Point(0, 64);
        tabMain.Name = "tabMain";
        tabMain.SelectedIndex = 0;
        tabMain.Size = new Size(960, 656);
        tabMain.TabIndex = 7;
        // 
        // tabTickets
        // 
        tabTickets.BackColor = Color.FromArgb(248, 250, 252);
        tabTickets.Controls.Add(lblTitulo);
        tabTickets.Controls.Add(pnlFiltros);
        tabTickets.Controls.Add(ucReabiertos);
        tabTickets.Controls.Add(dgvTickets);
        tabTickets.Controls.Add(ucCerrados);
        tabTickets.Controls.Add(ucEnProceso);
        tabTickets.Controls.Add(ucUrgentes);
        tabTickets.Controls.Add(ucNuevos);
        tabTickets.Location = new Point(4, 5);
        tabTickets.Name = "tabTickets";
        tabTickets.Padding = new Padding(3);
        tabTickets.Size = new Size(952, 647);
        tabTickets.TabIndex = 0;
        tabTickets.Text = "Tickets";
        tabTickets.AutoScroll = true;
        // 
        // pnlFiltros
        // 
        pnlFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        pnlFiltros.BackColor = Color.White;
        pnlFiltros.Controls.Add(lblFiltrosTitle);
        pnlFiltros.Controls.Add(filtroGenerico);
        pnlFiltros.Controls.Add(btnNuevoTicket);
        pnlFiltros.Controls.Add(btnLimpiarFiltros);
        pnlFiltros.Controls.Add(btnAbrirReportes);
        pnlFiltros.Controls.Add(btnRecargar);
        pnlFiltros.Location = new Point(12, 145);
        pnlFiltros.Name = "pnlFiltros";
        pnlFiltros.Size = new Size(936, 120);
        pnlFiltros.TabIndex = 8;
        // 
        // filtroGenerico
        // 
        filtroGenerico.Location = new Point(10, 25);
        filtroGenerico.Name = "filtroGenerico";
        filtroGenerico.Size = new Size(916, 48);
        filtroGenerico.TabIndex = 6;
        filtroGenerico.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        // 
        // lblFiltrosTitle
        // 
        lblFiltrosTitle.AutoSize = true;
        lblFiltrosTitle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblFiltrosTitle.ForeColor = Color.FromArgb(71, 85, 105);
        lblFiltrosTitle.Location = new Point(10, 5);
        lblFiltrosTitle.Name = "lblFiltrosTitle";
        lblFiltrosTitle.Size = new Size(165, 17);
        lblFiltrosTitle.Text = "Filtros de Búsqueda Rápida";
        // 
        // btnLimpiarFiltros
        // 
        btnLimpiarFiltros.Estilo = EstiloBotonModerno.Ghost;
        btnLimpiarFiltros.Icono = FontAwesome.Sharp.IconChar.Eraser;
        btnLimpiarFiltros.IconoTamano = 14;
        btnLimpiarFiltros.Location = new Point(710, 75);
        btnLimpiarFiltros.Name = "btnLimpiarFiltros";
        btnLimpiarFiltros.Size = new Size(110, 34);
        btnLimpiarFiltros.Text = "Limpiar";
        btnLimpiarFiltros.Click += btnLimpiarFiltros_Click;
        // 
        // btnAbrirReportes
        // 
        btnAbrirReportes.Estilo = EstiloBotonModerno.Primario;
        btnAbrirReportes.Icono = FontAwesome.Sharp.IconChar.ChartBar;
        btnAbrirReportes.IconoTamano = 14;
        btnAbrirReportes.Location = new Point(830, 75);
        btnAbrirReportes.Name = "btnAbrirReportes";
        btnAbrirReportes.Size = new Size(140, 34);
        btnAbrirReportes.Text = "Reportes";
        btnAbrirReportes.Click += btnAbrirReportes_Click;
        // 
        // DashboardAdminForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(1200, 720);
        Controls.Add(pnlContenedorPrincipal);
        Controls.Add(sidebarAdmin);
        MinimumSize = new Size(1100, 700);
        Name = "DashboardAdminForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "HSis Support - Administración";
        Load += DashboardAdmin_Load;
        ((System.ComponentModel.ISupportInitialize)dgvTickets).EndInit();
        tabMain.ResumeLayout(false);
        tabTickets.ResumeLayout(false);
        pnlFiltros.ResumeLayout(false);
        pnlContenedorPrincipal.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion
    private HSis.UI.Controls.SidebarControl sidebarAdmin;
    private HSis.UI.Controls.TopBarControl topBarAdmin;
    private Panel pnlContenedorPrincipal;
    private Label lblTitulo;
    private HSis.UI.Controls.IndicadorControl ucNuevos;
    private HSis.UI.Controls.IndicadorControl ucUrgentes;
    private HSis.UI.Controls.IndicadorControl ucEnProceso;
    private HSis.UI.Controls.IndicadorControl ucCerrados;
    private DataGridView dgvTickets;
    private HSis.UI.Controls.BotonModerno btnNuevoTicket;
    private HSis.UI.Controls.BotonModerno btnRecargar;
    private HSis.UI.Controls.IndicadorControl ucReabiertos;
    private TabControl tabMain;
    private TabPage tabTickets;
    private Panel pnlFiltros;
    private Label lblFiltrosTitle;
    private HSis.UI.Controls.FiltroGenericoControl filtroGenerico;
    private HSis.UI.Controls.BotonModerno btnLimpiarFiltros;
    private HSis.UI.Controls.BotonModerno btnAbrirReportes;

    private void InicializarLayoutDashboard()
    {
        // Instanciar control de paginación reutilizable
        PaginacionControl = new PaginacionControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        _ucCalificacion = new IndicadorControl();
        _ucCalificacion.IndicadorClic += UcCalificacion_Click;
        var tblIndicadores = AyudanteDisenoPanel.CrearPanelIndicadores(
            "tblIndicadoresAdmin",
            6,
            ucNuevos, ucUrgentes, ucEnProceso, ucCerrados, ucReabiertos, _ucCalificacion
        );

        filtroGenerico.Dock = DockStyle.Fill;
        filtroGenerico.Margin = new Padding(0, 4, 0, 4);
        filtroGenerico.RecargarClic += async (s, e) =>
        {
            await CargarGridCompletoAsync();
            await _presenter.CargarKPIsAsync(SesionSistema.IdUsuario);
        };
        filtroGenerico.LimpiarClic += async (s, e) =>
        {
            _controladorPaginacion.ReiniciarAPrimeraPagina();
            await CargarGridCompletoAsync();
        };

        dgvTickets.Dock = DockStyle.Fill;
        dgvTickets.Margin = new Padding(0, 4, 0, 6);

        var tblPrincipal = AyudanteDisenoPanel.CrearPanelPrincipal(tabTickets.ClientSize, incluirFiltros: true);
        tblPrincipal.Name = "tblPrincipalTickets";

        // Ensamblar el layout en el grid principal
        tblPrincipal.Controls.Add(tblIndicadores, 0, 0);
        tblPrincipal.Controls.Add(filtroGenerico, 0, 1);
        tblPrincipal.Controls.Add(dgvTickets, 0, 2);
        tblPrincipal.Controls.Add(PaginacionControl, 0, 3);

        AyudanteDisenoPanel.ReubicarControles(tabTickets, tblPrincipal, lblTitulo, ucNuevos, ucUrgentes, ucEnProceso, ucCerrados, ucReabiertos, pnlFiltros, dgvTickets);
    }
}
