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
        sidebarCliente = new SidebarControl();
        topBarCliente = new TopBarControl();
        pnlContenedorPrincipal = new Panel();
        vistaTickets = new VistaTicketsDashboardControl();
        btnNuevoReporte = new BotonModerno();
        pnlContenedorPrincipal.SuspendLayout();
        SuspendLayout();
        // 
        // sidebarCliente
        // 
        sidebarCliente.BackColor = Color.FromArgb(15, 23, 42);
        sidebarCliente.Dock = DockStyle.Left;
        sidebarCliente.Location = new Point(0, 0);
        sidebarCliente.Name = "sidebarCliente";
        sidebarCliente.Size = new Size(240, 720);
        sidebarCliente.TabIndex = 0;
        // 
        // topBarCliente
        // 
        topBarCliente.BackColor = Color.White;
        topBarCliente.Dock = DockStyle.Top;
        topBarCliente.Location = new Point(0, 0);
        topBarCliente.Name = "topBarCliente";
        topBarCliente.Size = new Size(960, 64);
        topBarCliente.TabIndex = 0;
        topBarCliente.Load += topBarCliente_Load;
        // 
        // pnlContenedorPrincipal
        // 
        pnlContenedorPrincipal.BackColor = Color.FromArgb(248, 250, 252);
        pnlContenedorPrincipal.Controls.Add(vistaTickets);
        pnlContenedorPrincipal.Controls.Add(topBarCliente);
        pnlContenedorPrincipal.Dock = DockStyle.Fill;
        pnlContenedorPrincipal.Location = new Point(240, 0);
        pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
        pnlContenedorPrincipal.Size = new Size(960, 720);
        pnlContenedorPrincipal.TabIndex = 1;
        // 
        // vistaTickets
        // 
        vistaTickets.BackColor = Color.FromArgb(248, 250, 252);
        vistaTickets.Dock = DockStyle.Fill;
        vistaTickets.Location = new Point(0, 64);
        vistaTickets.Name = "vistaTickets";
        vistaTickets.Size = new Size(960, 656);
        vistaTickets.TabIndex = 2;
        // 
        // btnNuevoReporte
        // 
        btnNuevoReporte.BackColor = Color.Transparent;
        btnNuevoReporte.FlatStyle = FlatStyle.Flat;
        btnNuevoReporte.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
        btnNuevoReporte.ForeColor = Color.White;
        btnNuevoReporte.Icono = FontAwesome.Sharp.IconChar.Add;
        btnNuevoReporte.Location = new Point(0, 0);
        btnNuevoReporte.Name = "btnNuevoReporte";
        btnNuevoReporte.Size = new Size(180, 48);
        btnNuevoReporte.TabIndex = 2;
        btnNuevoReporte.Text = "Nuevo Reporte";
        btnNuevoReporte.UseVisualStyleBackColor = false;
        // 
        // DashboardClienteForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(1200, 720);
        Controls.Add(pnlContenedorPrincipal);
        Controls.Add(sidebarCliente);
        MinimumSize = new Size(800, 500);
        Name = "DashboardClienteForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "HSis Support - Mi Portal";
        Load += frmDashboardCliente_Load;
        pnlContenedorPrincipal.ResumeLayout(false);
        ResumeLayout(false);
    }

    private HSis.UI.Controls.SidebarControl sidebarCliente;
    private HSis.UI.Controls.TopBarControl topBarCliente;
    private Panel pnlContenedorPrincipal;
    private HSis.UI.Controls.BotonModerno btnNuevoReporte;
    private HSis.UI.Controls.VistaTicketsDashboardControl vistaTickets;
}
