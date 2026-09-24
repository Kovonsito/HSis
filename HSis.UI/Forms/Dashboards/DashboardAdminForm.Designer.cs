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
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        sidebarAdmin = new HSis.UI.Controls.SidebarControl();
        topBarAdmin = new HSis.UI.Controls.TopBarControl();
        pnlContenedorPrincipal = new Panel();
        btnNuevoTicket = new HSis.UI.Controls.BotonModerno();
        btnAbrirReportes = new HSis.UI.Controls.BotonModerno();
        tabMain = new TabControl();
        tabTickets = new TabPage();
        vistaTickets = new HSis.UI.Controls.VistaTicketsDashboardControl();

        tabMain.SuspendLayout();
        tabTickets.SuspendLayout();
        pnlContenedorPrincipal.SuspendLayout();
        SuspendLayout();

        // sidebarAdmin
        sidebarAdmin.Dock = DockStyle.Left;
        sidebarAdmin.Location = new Point(0, 0);
        sidebarAdmin.Name = "sidebarAdmin";
        sidebarAdmin.Size = new Size(240, 720);
        sidebarAdmin.TabIndex = 0;

        // topBarAdmin
        topBarAdmin.Dock = DockStyle.Top;
        topBarAdmin.Location = new Point(0, 0);
        topBarAdmin.Name = "topBarAdmin";
        topBarAdmin.Size = new Size(960, 64);
        topBarAdmin.TabIndex = 0;
        topBarAdmin.Titulo = "Panel de Control";
        topBarAdmin.Subtitulo = "Mesa de Servicio y Gestión Global";

        // pnlContenedorPrincipal
        pnlContenedorPrincipal.BackColor = Color.FromArgb(248, 250, 252);
        pnlContenedorPrincipal.Controls.Add(tabMain);
        pnlContenedorPrincipal.Controls.Add(topBarAdmin);
        pnlContenedorPrincipal.Dock = DockStyle.Fill;
        pnlContenedorPrincipal.Location = new Point(240, 0);
        pnlContenedorPrincipal.Name = "pnlContenedorPrincipal";
        pnlContenedorPrincipal.Size = new Size(960, 720);
        pnlContenedorPrincipal.TabIndex = 1;

        // btnNuevoTicket
        btnNuevoTicket.Estilo = EstiloBotonModerno.Exito;
        btnNuevoTicket.Icono = FontAwesome.Sharp.IconChar.Plus;
        btnNuevoTicket.IconoTamano = 14;
        btnNuevoTicket.Name = "btnNuevoTicket";
        btnNuevoTicket.Size = new Size(150, 36);
        btnNuevoTicket.TabIndex = 5;
        btnNuevoTicket.Text = "Nuevo Ticket";

        // btnAbrirReportes
        btnAbrirReportes.Estilo = EstiloBotonModerno.Secundario;
        btnAbrirReportes.Icono = FontAwesome.Sharp.IconChar.ChartBar;
        btnAbrirReportes.IconoTamano = 14;
        btnAbrirReportes.Name = "btnAbrirReportes";
        btnAbrirReportes.Size = new Size(140, 36);
        btnAbrirReportes.TabIndex = 6;
        btnAbrirReportes.Text = "Reportes";

        // vistaTickets
        vistaTickets.Dock = DockStyle.Fill;
        vistaTickets.Name = "vistaTickets";
        vistaTickets.TabIndex = 0;

        // tabTickets
        tabTickets.BackColor = Color.FromArgb(248, 250, 252);
        tabTickets.Controls.Add(vistaTickets);
        tabTickets.Location = new Point(4, 5);
        tabTickets.Name = "tabTickets";
        tabTickets.Padding = new Padding(3);
        tabTickets.Size = new Size(952, 647);
        tabTickets.TabIndex = 0;
        tabTickets.Text = "Tickets";

        // tabMain
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

        // DashboardAdminForm
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(1200, 720);
        Controls.Add(pnlContenedorPrincipal);
        Controls.Add(sidebarAdmin);
        MinimumSize = new Size(800, 500);
        Name = "DashboardAdminForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "HSis Support - Administración";
        Load += DashboardAdmin_Load;
        tabMain.ResumeLayout(false);
        tabTickets.ResumeLayout(false);
        pnlContenedorPrincipal.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private HSis.UI.Controls.SidebarControl sidebarAdmin;
    private HSis.UI.Controls.TopBarControl topBarAdmin;
    private Panel pnlContenedorPrincipal;
    private HSis.UI.Controls.BotonModerno btnNuevoTicket;
    private HSis.UI.Controls.BotonModerno btnAbrirReportes;
    private TabControl tabMain;
    private TabPage tabTickets;
    private HSis.UI.Controls.VistaTicketsDashboardControl vistaTickets;
}
