using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Dashboards;

partial class DashboardTecnicoForm
{
    private IContainer components = null;

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
        this.sidebarTecnico = new HSis.UI.Controls.SidebarControl();
        this.topBarTecnico = new HSis.UI.Controls.TopBarControl();
        this.pnlContenedorPrincipal = new Panel();
        this.lblTitulo = new Label();
        this.btnNuevoTicket = new HSis.UI.Controls.BotonModerno();
        this.ucMisAsignados = new HSis.UI.Controls.IndicadorControl();
        this.ucDisponibles = new HSis.UI.Controls.IndicadorControl();
        this.ucCerrados = new HSis.UI.Controls.IndicadorControl();
        this.ucCalificacion = new HSis.UI.Controls.IndicadorControl();
        this.dgvTicketsOperativos = new DataGridView();
        this.pnlFiltros = new Panel();
        this.lblFiltrosTitle = new Label();
        this.filtroGenerico = new HSis.UI.Controls.FiltroGenericoControl();
        this.btnLimpiarFiltros = new HSis.UI.Controls.BotonModerno();
        this.btnRecargar = new HSis.UI.Controls.BotonModerno();
        ((System.ComponentModel.ISupportInitialize)(this.dgvTicketsOperativos)).BeginInit();
        this.pnlFiltros.SuspendLayout();
        this.pnlContenedorPrincipal.SuspendLayout();
        this.SuspendLayout();

        // sidebarTecnico
        this.sidebarTecnico.Dock = DockStyle.Left;
        this.sidebarTecnico.Location = new Point(0, 0);
        this.sidebarTecnico.Name = "sidebarTecnico";
        this.sidebarTecnico.Size = new Size(240, 720);
        this.sidebarTecnico.TabIndex = 0;

        // topBarTecnico
        this.topBarTecnico.Dock = DockStyle.Top;
        this.topBarTecnico.Location = new Point(0, 0);
        this.topBarTecnico.Name = "topBarTecnico";
        this.topBarTecnico.Size = new Size(960, 64);
        this.topBarTecnico.TabIndex = 0;
        this.topBarTecnico.Titulo = "Panel de Control - Técnico";
        this.topBarTecnico.Subtitulo = "Gestión de Tickets y Soporte Técnico";

        // pnlContenedorPrincipal
        this.pnlContenedorPrincipal.Controls.Add(this.topBarTecnico);
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
        this.lblTitulo.Size = new Size(320, 30);
        this.lblTitulo.TabIndex = 0;
        this.lblTitulo.Text = "Bandeja Operativa de Tickets";

        // ucMisAsignados
        this.ucMisAsignados.Location = new Point(12, 50);
        this.ucMisAsignados.Name = "ucMisAsignados";
        this.ucMisAsignados.Size = new Size(200, 100);
        this.ucMisAsignados.TabIndex = 1;

        // ucDisponibles
        this.ucDisponibles.Location = new Point(220, 50);
        this.ucDisponibles.Name = "ucDisponibles";
        this.ucDisponibles.Size = new Size(200, 100);
        this.ucDisponibles.TabIndex = 2;

        // ucCerrados
        this.ucCerrados.Location = new Point(428, 50);
        this.ucCerrados.Name = "ucCerrados";
        this.ucCerrados.Size = new Size(200, 100);
        this.ucCerrados.TabIndex = 4;

        // ucCalificacion
        this.ucCalificacion.Location = new Point(636, 50);
        this.ucCalificacion.Name = "ucCalificacion";
        this.ucCalificacion.Size = new Size(200, 100);
        this.ucCalificacion.TabIndex = 5;

        // dgvTicketsOperativos
        this.dgvTicketsOperativos.AllowUserToAddRows = false;
        this.dgvTicketsOperativos.AllowUserToDeleteRows = false;
        this.dgvTicketsOperativos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        this.dgvTicketsOperativos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        this.dgvTicketsOperativos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.dgvTicketsOperativos.Location = new Point(12, 290);
        this.dgvTicketsOperativos.Name = "dgvTicketsOperativos";
        this.dgvTicketsOperativos.ReadOnly = true;
        this.dgvTicketsOperativos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvTicketsOperativos.Size = new Size(936, 300);
        this.dgvTicketsOperativos.TabIndex = 3;
        this.dgvTicketsOperativos.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvTicketsOperativos_CellDoubleClick);

        // pnlFiltros
        this.pnlFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        this.pnlFiltros.BackColor = Color.White;
        this.pnlFiltros.Controls.Add(this.lblFiltrosTitle);
        this.pnlFiltros.Controls.Add(this.filtroGenerico);
        this.pnlFiltros.Controls.Add(this.btnNuevoTicket);
        this.pnlFiltros.Controls.Add(this.btnLimpiarFiltros);
        this.pnlFiltros.Controls.Add(this.btnRecargar);
        this.pnlFiltros.Location = new Point(12, 145);
        this.pnlFiltros.Name = "pnlFiltros";
        this.pnlFiltros.Size = new Size(936, 120);
        this.pnlFiltros.TabIndex = 6;

        // filtroGenerico
        this.filtroGenerico.Location = new Point(10, 25);
        this.filtroGenerico.Name = "filtroGenerico";
        this.filtroGenerico.Size = new Size(916, 48);
        this.filtroGenerico.TabIndex = 0;
        this.filtroGenerico.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

        // lblFiltrosTitle
        this.lblFiltrosTitle.AutoSize = true;
        this.lblFiltrosTitle.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        this.lblFiltrosTitle.ForeColor = Color.FromArgb(71, 85, 105);
        this.lblFiltrosTitle.Location = new Point(10, 5);
        this.lblFiltrosTitle.Name = "lblFiltrosTitle";
        this.lblFiltrosTitle.Size = new Size(165, 17);
        this.lblFiltrosTitle.Text = "Filtros de Búsqueda Rápida";

        // btnNuevoTicket
        this.btnNuevoTicket.Estilo = EstiloBotonModerno.Exito;
        this.btnNuevoTicket.Icono = FontAwesome.Sharp.IconChar.Plus;
        this.btnNuevoTicket.IconoTamano = 14;
        this.btnNuevoTicket.Location = new Point(540, 75);
        this.btnNuevoTicket.Name = "btnNuevoTicket";
        this.btnNuevoTicket.Size = new Size(140, 34);
        this.btnNuevoTicket.TabIndex = 1;
        this.btnNuevoTicket.Text = "Nuevo Ticket";
        this.btnNuevoTicket.Click += new EventHandler(this.btnNuevoTicket_Click);

        // btnRecargar
        this.btnRecargar.Estilo = EstiloBotonModerno.Secundario;
        this.btnRecargar.Icono = FontAwesome.Sharp.IconChar.RotateRight;
        this.btnRecargar.IconoTamano = 14;
        this.btnRecargar.Location = new Point(690, 75);
        this.btnRecargar.Name = "btnRecargar";
        this.btnRecargar.Size = new Size(120, 34);
        this.btnRecargar.TabIndex = 2;
        this.btnRecargar.Text = "Recargar";
        this.btnRecargar.Click += new EventHandler(this.btnRecargar_Click);

        // btnLimpiarFiltros
        this.btnLimpiarFiltros.Estilo = EstiloBotonModerno.Ghost;
        this.btnLimpiarFiltros.Icono = FontAwesome.Sharp.IconChar.Eraser;
        this.btnLimpiarFiltros.IconoTamano = 14;
        this.btnLimpiarFiltros.Location = new Point(820, 75);
        this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
        this.btnLimpiarFiltros.Size = new Size(105, 34);
        this.btnLimpiarFiltros.TabIndex = 3;
        this.btnLimpiarFiltros.Text = "Limpiar";
        this.btnLimpiarFiltros.Click += new EventHandler(this.btnLimpiarFiltros_Click);

        // DashboardTecnicoForm
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.FromArgb(248, 250, 252);
        this.ClientSize = new Size(1200, 720);
        this.MinimumSize = new Size(1100, 700);
        this.Controls.Add(this.pnlContenedorPrincipal);
        this.Controls.Add(this.sidebarTecnico);
        this.Name = "DashboardTecnicoForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "HSis Support - Técnico";
        this.Load += new EventHandler(this.frmDashboardTecnico_Load);
        ((System.ComponentModel.ISupportInitialize)(this.dgvTicketsOperativos)).EndInit();
        this.pnlFiltros.ResumeLayout(false);
        this.pnlContenedorPrincipal.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private HSis.UI.Controls.SidebarControl sidebarTecnico;
    private HSis.UI.Controls.TopBarControl topBarTecnico;
    private Panel pnlContenedorPrincipal;
    private Label lblTitulo;
    private HSis.UI.Controls.IndicadorControl ucMisAsignados;
    private HSis.UI.Controls.IndicadorControl ucDisponibles;
    private HSis.UI.Controls.IndicadorControl ucCerrados;
    private HSis.UI.Controls.IndicadorControl ucCalificacion;
    private DataGridView dgvTicketsOperativos;
    private Panel pnlFiltros;
    private Label lblFiltrosTitle;
    private HSis.UI.Controls.FiltroGenericoControl filtroGenerico;
    private HSis.UI.Controls.BotonModerno btnNuevoTicket;
    private HSis.UI.Controls.BotonModerno btnLimpiarFiltros;
    private HSis.UI.Controls.BotonModerno btnRecargar;

    private void InicializarLayoutDashboard()
    {
        PaginacionControl = new PaginacionControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        ucMisAsignados.IndicadorClic += UcMisAsignados_Click;
        ucDisponibles.IndicadorClic += UcDisponibles_Click;
        ucCerrados.IndicadorClic += UcCerrados_Click;
        ucCalificacion.IndicadorClic += UcCalificacion_Click;

        var tblIndicadores = AyudanteDisenoPanel.CrearPanelIndicadores(
            "tblIndicadoresTecnico",
            4,
            ucMisAsignados, ucDisponibles, ucCerrados, ucCalificacion
        );

        filtroGenerico.Dock = DockStyle.Fill;
        filtroGenerico.Margin = new Padding(0, 4, 0, 4);
        filtroGenerico.RecargarClic += (s, e) => _ = CargarDatosInicialesAsync();
        filtroGenerico.LimpiarClic += (s, e) => AplicarFiltrosMemoria();

        dgvTicketsOperativos.Dock = DockStyle.Fill;
        dgvTicketsOperativos.Margin = new Padding(0, 4, 0, 6);

        var tblPrincipal = AyudanteDisenoPanel.CrearPanelPrincipal(pnlContenedorPrincipal.ClientSize, incluirFiltros: true);
        tblPrincipal.Controls.Add(tblIndicadores, 0, 0);
        tblPrincipal.Controls.Add(filtroGenerico, 0, 1);
        tblPrincipal.Controls.Add(dgvTicketsOperativos, 0, 2);
        tblPrincipal.Controls.Add(PaginacionControl, 0, 3);

        AyudanteDisenoPanel.ReubicarControles(pnlContenedorPrincipal, tblPrincipal, lblTitulo, ucMisAsignados, ucDisponibles, ucCerrados, ucCalificacion, pnlFiltros, dgvTicketsOperativos);
    }
}
