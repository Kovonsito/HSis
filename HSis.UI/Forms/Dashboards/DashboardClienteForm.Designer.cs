using System;
using System.Drawing;
using System.Windows.Forms;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Dashboards;

partial class DashboardClienteForm
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
        this.sidebarCliente = new HSis.UI.Controls.SidebarControl();
        this.topBarCliente = new HSis.UI.Controls.TopBarControl();
        this.pnlContenedorPrincipal = new Panel();
        this.lblTitulo = new Label();
        this.ucMisActivos = new HSis.UI.Controls.IndicadorControl();
        this.dgvMisTickets = new DataGridView();
        this.btnNuevoReporte = new HSis.UI.Controls.BotonModerno();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMisTickets)).BeginInit();
        this.pnlContenedorPrincipal.SuspendLayout();
        this.SuspendLayout();

        // sidebarCliente
        this.sidebarCliente.Dock = DockStyle.Left;
        this.sidebarCliente.Location = new Point(0, 0);
        this.sidebarCliente.Name = "sidebarCliente";
        this.sidebarCliente.Size = new Size(240, 720);
        this.sidebarCliente.TabIndex = 0;

        // topBarCliente
        this.topBarCliente.Dock = DockStyle.Top;
        this.topBarCliente.Location = new Point(0, 0);
        this.topBarCliente.Name = "topBarCliente";
        this.topBarCliente.Size = new Size(960, 64);
        this.topBarCliente.TabIndex = 0;
        this.topBarCliente.Titulo = "Mi Portal de Soporte";
        this.topBarCliente.Subtitulo = "Seguimiento y Registro de Solicitudes";

        // pnlContenedorPrincipal
        this.pnlContenedorPrincipal.Controls.Add(this.topBarCliente);
        this.pnlContenedorPrincipal.Dock = DockStyle.Fill;
        this.pnlContenedorPrincipal.Location = new Point(240, 0);
        this.pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
        this.pnlContenedorPrincipal.Size = new Size(960, 720);
        this.pnlContenedorPrincipal.TabIndex = 1;

        // lblTitulo
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        this.lblTitulo.ForeColor = Color.FromArgb(15, 23, 42);
        this.lblTitulo.Location = new Point(12, 10);
        this.lblTitulo.Name = "lblTitulo";
        this.lblTitulo.Size = new Size(270, 30);
        this.lblTitulo.TabIndex = 0;
        this.lblTitulo.Text = "Mis Solicitudes y Tickets";

        // ucMisActivos
        this.ucMisActivos.Location = new Point(12, 50);
        this.ucMisActivos.Name = "ucMisActivos";
        this.ucMisActivos.Size = new Size(200, 100);
        this.ucMisActivos.TabIndex = 1;

        // btnNuevoReporte
        this.btnNuevoReporte.Estilo = EstiloBotonModerno.Primario;
        this.btnNuevoReporte.Icono = FontAwesome.Sharp.IconChar.Plus;
        this.btnNuevoReporte.IconoTamano = 16;
        this.btnNuevoReporte.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
        this.btnNuevoReporte.Location = new Point(10, 25);
        this.btnNuevoReporte.Name = "btnNuevoReporte";
        this.btnNuevoReporte.Size = new Size(180, 48);
        this.btnNuevoReporte.TabIndex = 2;
        this.btnNuevoReporte.Text = "Nuevo Reporte";
        this.btnNuevoReporte.Click += new EventHandler(this.btnNuevoReporte_Click);

        // dgvMisTickets
        this.dgvMisTickets.AllowUserToAddRows = false;
        this.dgvMisTickets.AllowUserToDeleteRows = false;
        this.dgvMisTickets.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvMisTickets.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvMisTickets.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dgvMisTickets.Location = new Point(12, 170);
        this.dgvMisTickets.Name = "dgvMisTickets";
        this.dgvMisTickets.ReadOnly = true;
        this.dgvMisTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvMisTickets.Size = new Size(936, 400);
        this.dgvMisTickets.TabIndex = 3;
        this.dgvMisTickets.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvMisTickets_CellDoubleClick);

        // DashboardClienteForm
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.FromArgb(248, 250, 252);
        this.ClientSize = new Size(1200, 720);
        this.MinimumSize = new Size(1100, 700);
        this.Controls.Add(this.pnlContenedorPrincipal);
        this.Controls.Add(this.sidebarCliente);
        this.Name = "DashboardClienteForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "HSis Support - Mi Portal";
        this.Load += new EventHandler(this.frmDashboardCliente_Load);
        ((System.ComponentModel.ISupportInitialize)(this.dgvMisTickets)).EndInit();
        this.pnlContenedorPrincipal.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private HSis.UI.Controls.SidebarControl sidebarCliente;
    private HSis.UI.Controls.TopBarControl topBarCliente;
    private Panel pnlContenedorPrincipal;
    private Label lblTitulo;
    private HSis.UI.Controls.IndicadorControl ucMisActivos;
    private DataGridView dgvMisTickets;
    private HSis.UI.Controls.BotonModerno btnNuevoReporte;
    private HSis.UI.Controls.FiltroGenericoControl filtroCliente = null!;

    private void InicializarLayoutDashboard()
    {
        PaginacionControl = new PaginacionControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        ucMisCerrados = new IndicadorControl();
        ucMisActivos.IndicadorClic += UcMisActivos_Click;
        ucMisCerrados.IndicadorClic += UcMisCerrados_Click;

        btnNuevoReporte.Dock = DockStyle.Fill;
        btnNuevoReporte.Margin = new Padding(5, 0, 0, 0);

        var tblIndicadores = AyudanteDisenoPanel.CrearPanelIndicadores("tblCabeceraCliente", 3, ucMisActivos, ucMisCerrados, btnNuevoReporte);

        filtroCliente = new FiltroGenericoControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0, 4, 0, 4)
        };
        filtroCliente.RecargarClic += (s, e) => _ = CargarDatosDashboardAsync();
        filtroCliente.LimpiarClic += (s, e) =>
        {
            _controladorPaginacion.ReiniciarAPrimeraPagina();
            MostrarPaginaActual();
        };

        dgvMisTickets.Dock = DockStyle.Fill;
        dgvMisTickets.Margin = new Padding(0, 4, 0, 6);

        var tblPrincipal = AyudanteDisenoPanel.CrearPanelPrincipal(pnlContenedorPrincipal.ClientSize, true);
        tblPrincipal.Controls.Add(tblIndicadores, 0, 0);
        tblPrincipal.Controls.Add(filtroCliente, 0, 1);
        tblPrincipal.Controls.Add(dgvMisTickets, 0, 2);
        tblPrincipal.Controls.Add(PaginacionControl, 0, 3);

        AyudanteDisenoPanel.ReubicarControles(pnlContenedorPrincipal, tblPrincipal, lblTitulo, ucMisActivos, btnNuevoReporte, dgvMisTickets);
    }
}
