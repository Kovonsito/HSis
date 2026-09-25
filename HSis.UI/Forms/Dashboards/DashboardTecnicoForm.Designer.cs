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
        this.btnNuevoTicket = new HSis.UI.Controls.BotonModerno();
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
        this.pnlContenedorPrincipal.BackColor = Color.FromArgb(248, 250, 252);
        this.pnlContenedorPrincipal.Dock = DockStyle.Fill;
        this.pnlContenedorPrincipal.Location = new Point(240, 0);
        this.pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
        this.pnlContenedorPrincipal.Size = new Size(960, 720);
        this.pnlContenedorPrincipal.TabIndex = 1;

        // btnNuevoTicket
        this.btnNuevoTicket.Estilo = EstiloBotonModerno.Exito;
        this.btnNuevoTicket.Icono = FontAwesome.Sharp.IconChar.Plus;
        this.btnNuevoTicket.IconoTamano = 14;
        this.btnNuevoTicket.Size = new Size(140, 36);
        this.btnNuevoTicket.TabIndex = 6;
        this.btnNuevoTicket.Text = "Nuevo Ticket";

        // vistaTickets
        this.vistaTickets = new HSis.UI.Controls.VistaTicketsDashboardControl();
        this.vistaTickets.Dock = DockStyle.Fill;
        this.vistaTickets.Location = new Point(0, 64);
        this.vistaTickets.Name = "vistaTickets";
        this.vistaTickets.TabIndex = 1;
        this.pnlContenedorPrincipal.Controls.Add(this.vistaTickets);
        this.pnlContenedorPrincipal.Controls.Add(this.topBarTecnico);

        // DashboardTecnicoForm
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.FromArgb(248, 250, 252);
        this.ClientSize = new Size(1200, 720);
        this.MinimumSize = new Size(800, 500);
        this.Controls.Add(this.pnlContenedorPrincipal);
        this.Controls.Add(this.sidebarTecnico);
        this.Name = "DashboardTecnicoForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "HSis Support - Técnico";
        this.Load += new EventHandler(this.frmDashboardTecnico_Load);
        this.pnlContenedorPrincipal.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private HSis.UI.Controls.SidebarControl sidebarTecnico;
    private HSis.UI.Controls.TopBarControl topBarTecnico;
    private Panel pnlContenedorPrincipal;
    private HSis.UI.Controls.BotonModerno btnNuevoTicket;
    private HSis.UI.Controls.VistaTicketsDashboardControl vistaTickets;
}
