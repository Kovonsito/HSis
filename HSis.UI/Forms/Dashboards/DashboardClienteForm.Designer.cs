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
        this.lblTitulo = new Label();
        this.ucMisActivos = new HSis.UI.Controls.IndicadorControl();
        this.dgvMisTickets = new DataGridView();
        this.btnNuevoReporte = new Button();
        ((System.ComponentModel.ISupportInitialize)(this.dgvMisTickets)).BeginInit();
        this.SuspendLayout();

        // lblTitulo
        this.lblTitulo.AutoSize = true;
        this.lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
        this.lblTitulo.Location = new Point(12, 9);
        this.lblTitulo.Name = "lblTitulo";
        this.lblTitulo.Size = new Size(341, 32);
        this.lblTitulo.TabIndex = 0;
        this.lblTitulo.Text = "Mi Panel - Mis Reportes";

        // ucMisActivos
        this.ucMisActivos.Location = new Point(12, 50);
        this.ucMisActivos.Name = "ucMisActivos";
        this.ucMisActivos.Size = new Size(200, 100);
        this.ucMisActivos.TabIndex = 1;

        // btnNuevoReporte
        this.btnNuevoReporte.BackColor = Color.FromArgb(52, 152, 219);
        this.btnNuevoReporte.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        this.btnNuevoReporte.ForeColor = Color.White;
        this.btnNuevoReporte.Location = new Point(220, 70);
        this.btnNuevoReporte.Name = "btnNuevoReporte";
        this.btnNuevoReporte.Size = new Size(150, 60);
        this.btnNuevoReporte.TabIndex = 2;
        this.btnNuevoReporte.Text = "+ Nuevo Reporte";
        this.btnNuevoReporte.UseVisualStyleBackColor = false;
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
        dgvMisTickets.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        this.dgvMisTickets.Size = new Size(760, 280);
        this.dgvMisTickets.TabIndex = 3;
        this.dgvMisTickets.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvMisTickets_CellDoubleClick);

        // DashboardClienteForm
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(1100, 700);
        this.MinimumSize = new Size(1030, 680);
        this.Controls.Add(this.dgvMisTickets);
        this.Controls.Add(this.btnNuevoReporte);
        this.Controls.Add(this.ucMisActivos);
        this.Controls.Add(this.lblTitulo);
        this.Name = "DashboardClienteForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Dashboard - Cliente";
        this.Load += new EventHandler(this.frmDashboardCliente_Load);
        ((System.ComponentModel.ISupportInitialize)(this.dgvMisTickets)).EndInit();
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    private Label lblTitulo;
    private HSis.UI.Controls.IndicadorControl ucMisActivos;
    private DataGridView dgvMisTickets;
    private Button btnNuevoReporte;

    private void InicializarLayoutDashboard()
    {
        // Instanciar control de paginación
        PaginacionControl = new PaginacionControl
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(12, 0, 12, 6)
        };

        // Instanciar indicador de cerrados
        ucMisCerrados = new IndicadorControl();

        // Suscribir eventos de filtrado
        ucMisActivos.IndicadorClic += UcMisActivos_Click;
        ucMisCerrados.IndicadorClic += UcMisCerrados_Click;

        var tblPrincipal = CrearPanelPrincipal();
        var tblIndicadores = CrearPanelIndicadores();

        // Configurar el título y el grid principal para que se estiren
        lblTitulo.Dock = DockStyle.Fill;
        lblTitulo.Margin = new Padding(12, 10, 12, 10);

        // Configurar el grid principal para que se estire
        dgvMisTickets.Dock = DockStyle.Fill;
        dgvMisTickets.Margin = new Padding(12, 10, 12, 12);

        // Agregar componentes al TableLayoutPanel principal
        tblPrincipal.Controls.Add(lblTitulo, 0, 0);
        tblPrincipal.Controls.Add(tblIndicadores, 0, 1);
        tblPrincipal.Controls.Add(dgvMisTickets, 0, 2);
        tblPrincipal.Controls.Add(PaginacionControl, 0, 3);

        // Remover controles del formulario para agregarlos al grid principal
        ReubicarControlesAlPrincipal(tblPrincipal);
    }

    private TableLayoutPanel CrearPanelPrincipal()
    {
        var tbl = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Name = "tblPrincipal",
            RowCount = 4,
            ColumnCount = 1,
            Size = this.ClientSize
        };

        tbl.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Fila 0: Título (lblTitulo)
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 120F)); // Fila 1: Indicador y Botón
        tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Fila 2: Grid de Tickets (dgvMisTickets)
        tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F)); // Fila 3: Paginación

        return tbl;
    }

    private TableLayoutPanel CrearPanelIndicadores()
    {
        var tblCabecera = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Name = "tblCabecera",
            RowCount = 1,
            ColumnCount = 3,
            Margin = new Padding(7, 0, 7, 0)
        };
        tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F)); // Columna 0: ucMisActivos
        tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F)); // Columna 1: ucMisCerrados
        tblCabecera.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F)); // Columna 2: btnNuevoReporte centrado

        // Panel contenedor para centrar verticalmente el botón
        var pnlBoton = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0)
        };

        // Configurar el botón
        btnNuevoReporte.Location = new Point(10, 25); // Posicionarlo de forma prolija en el panel (centrado vertical en altura 110px)
        pnlBoton.Controls.Add(btnNuevoReporte);

        ucMisActivos.Dock = DockStyle.Fill;
        ucMisActivos.Margin = new Padding(5);

        ucMisCerrados.Dock = DockStyle.Fill;
        ucMisCerrados.Margin = new Padding(5);

        tblCabecera.Controls.Add(ucMisActivos, 0, 0);
        tblCabecera.Controls.Add(ucMisCerrados, 1, 0);
        tblCabecera.Controls.Add(pnlBoton, 2, 0);

        return tblCabecera;
    }

    private void ReubicarControlesAlPrincipal(TableLayoutPanel tblPrincipal)
    {
        this.Controls.Remove(lblTitulo);
        this.Controls.Remove(ucMisActivos);
        this.Controls.Remove(btnNuevoReporte);
        this.Controls.Remove(dgvMisTickets);

        this.Controls.Add(tblPrincipal);
    }
}
