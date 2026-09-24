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
        this.btnNuevoReporte = new HSis.UI.Controls.BotonModerno();
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
        this.pnlContenedorPrincipal.BackColor = Color.FromArgb(248, 250, 252);
        this.pnlContenedorPrincipal.Controls.Add(this.topBarCliente);
        this.pnlContenedorPrincipal.Dock = DockStyle.Fill;
        this.pnlContenedorPrincipal.Location = new Point(240, 0);
        this.pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
        this.pnlContenedorPrincipal.Size = new Size(960, 720);
        this.pnlContenedorPrincipal.TabIndex = 1;

        // btnNuevoReporte
        this.btnNuevoReporte.Estilo = EstiloBotonModerno.Primario;
        this.btnNuevoReporte.Icono = FontAwesome.Sharp.IconChar.Plus;
        this.btnNuevoReporte.IconoTamano = 16;
        this.btnNuevoReporte.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
        this.btnNuevoReporte.Name = "btnNuevoReporte";
        this.btnNuevoReporte.Size = new Size(180, 48);
        this.btnNuevoReporte.TabIndex = 2;
        this.btnNuevoReporte.Text = "Nuevo Reporte";

        // vistaTickets
        this.vistaTickets = new HSis.UI.Controls.VistaTicketsDashboardControl();
        this.vistaTickets.Dock = DockStyle.Fill;
        this.vistaTickets.Location = new Point(0, 64);
        this.vistaTickets.Name = "vistaTickets";
        this.vistaTickets.Size = new Size(960, 656);
        this.vistaTickets.TabIndex = 2;
        this.pnlContenedorPrincipal.Controls.Add(this.vistaTickets);

        // DashboardClienteForm
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.FromArgb(248, 250, 252);
        this.ClientSize = new Size(1200, 720);
        this.MinimumSize = new Size(800, 500);
        this.Controls.Add(this.pnlContenedorPrincipal);
        this.Controls.Add(this.sidebarCliente);
        this.Name = "DashboardClienteForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "HSis Support - Mi Portal";
        this.Load += new EventHandler(this.frmDashboardCliente_Load);
        this.pnlContenedorPrincipal.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private HSis.UI.Controls.SidebarControl sidebarCliente;
    private HSis.UI.Controls.TopBarControl topBarCliente;
    private Panel pnlContenedorPrincipal;
    private HSis.UI.Controls.BotonModerno btnNuevoReporte;
    private HSis.UI.Controls.VistaTicketsDashboardControl vistaTickets;
}
