using System;
using System.Drawing;
using System.Windows.Forms;
using System.ComponentModel;

namespace HSis.UI.Forms.Dashboards
{
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
            this.lblTitulo = new Label();
            this.btnNuevoTicket = new Button();
            this.ucMisAsignados = new HSis.UI.Controls.IndicadorControl();
            this.ucDisponibles = new HSis.UI.Controls.IndicadorControl();
            this.ucCerrados = new HSis.UI.Controls.IndicadorControl();
            this.ucCalificacion = new HSis.UI.Controls.IndicadorControl();
            this.dgvTicketsOperativos = new DataGridView();
            this.pnlFiltros = new Panel();
            this.lblFiltrosTitle = new Label();
            this.filtroGenerico = new HSis.UI.Controls.FiltroGenericoControl();
            this.btnLimpiarFiltros = new Button();
            this.btnRecargar = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTicketsOperativos)).BeginInit();
            this.SuspendLayout();

            // lblTitulo
            this.lblTitulo.AutoSize = true;
            this.lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            this.lblTitulo.Location = new Point(12, 9);
            this.lblTitulo.Name = "lblTitulo";
            this.lblTitulo.Size = new Size(362, 32);
            this.lblTitulo.TabIndex = 0;
            this.lblTitulo.Text = "Panel de Control - Técnico";

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
            this.dgvTicketsOperativos.Location = new Point(12, 170);
            this.dgvTicketsOperativos.Name = "dgvTicketsOperativos";
            this.dgvTicketsOperativos.ReadOnly = true;
            this.dgvTicketsOperativos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            this.dgvTicketsOperativos.Size = new Size(824, 280);
            this.dgvTicketsOperativos.TabIndex = 3;
            this.dgvTicketsOperativos.CellDoubleClick += new DataGridViewCellEventHandler(this.dgvTicketsOperativos_CellDoubleClick);

            // 
            // pnlFiltros
            // 
            this.pnlFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            this.pnlFiltros.BorderStyle = BorderStyle.FixedSingle;
            this.pnlFiltros.AutoScroll = true;
            this.pnlFiltros.Controls.Add(this.lblFiltrosTitle);
            this.pnlFiltros.Controls.Add(this.filtroGenerico);
            this.pnlFiltros.Controls.Add(this.btnNuevoTicket);
            this.pnlFiltros.Controls.Add(this.btnLimpiarFiltros);
            this.pnlFiltros.Controls.Add(this.btnRecargar);
            this.pnlFiltros.Location = new Point(12, 145);
            this.pnlFiltros.Name = "pnlFiltros";
            this.pnlFiltros.Size = new Size(1024, 115);
            this.pnlFiltros.TabIndex = 6;
            this.pnlFiltros.BackColor = Color.White;

            // 
            // filtroGenerico
            // 
            this.filtroGenerico.Location = new Point(10, 25);
            this.filtroGenerico.Name = "filtroGenerico";
            this.filtroGenerico.Size = new Size(1004, 52);
            this.filtroGenerico.TabIndex = 0;
            this.filtroGenerico.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

            // 
            // lblFiltrosTitle
            // 
            this.lblFiltrosTitle.AutoSize = true;
            this.lblFiltrosTitle.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.lblFiltrosTitle.ForeColor = Color.FromArgb(31, 41, 55);
            this.lblFiltrosTitle.Location = new Point(10, 5);
            this.lblFiltrosTitle.Name = "lblFiltrosTitle";
            this.lblFiltrosTitle.Size = new Size(180, 19);
            this.lblFiltrosTitle.Text = "Filtros de Búsqueda Rápida";

            // 
            // btnNuevoTicket
            // 
            this.btnNuevoTicket.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.btnNuevoTicket.Location = new Point(430, 80);
            this.btnNuevoTicket.Name = "btnNuevoTicket";
            this.btnNuevoTicket.Size = new Size(130, 26);
            this.btnNuevoTicket.Text = "+ Registrar Ticket";
            this.btnNuevoTicket.UseVisualStyleBackColor = false;
            this.btnNuevoTicket.Click += new EventHandler(this.btnNuevoTicket_Click);

            // 
            // btnLimpiarFiltros
            // 
            this.btnLimpiarFiltros.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            this.btnLimpiarFiltros.Location = new Point(720, 80);
            this.btnLimpiarFiltros.Name = "btnLimpiarFiltros";
            this.btnLimpiarFiltros.Size = new Size(130, 26);
            this.btnLimpiarFiltros.Text = "Limpiar Filtros";
            this.btnLimpiarFiltros.Click += new EventHandler(this.btnLimpiarFiltros_Click);

            // 
            // btnRecargar
            // 
            this.btnRecargar.Location = new Point(580, 80);
            this.btnRecargar.Name = "btnRecargar";
            this.btnRecargar.Size = new Size(130, 26);
            this.btnRecargar.Text = "Recargar tabla";
            this.btnRecargar.UseVisualStyleBackColor = true;
            this.btnRecargar.Click += new EventHandler(this.btnRecargar_Click);

            // DashboardTecnicoForm
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1100, 700);
            this.MinimumSize = new Size(1030, 680);
            this.Controls.Add(this.dgvTicketsOperativos);
            this.Controls.Add(this.ucDisponibles);
            this.Controls.Add(this.ucMisAsignados);
            this.Controls.Add(this.ucCerrados);
            this.Controls.Add(this.ucCalificacion);
            this.Controls.Add(this.lblTitulo);
            this.Controls.Add(this.pnlFiltros);
            this.Name = "DashboardTecnicoForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Dashboard - Técnico";
            this.Load += new EventHandler(this.frmDashboardTecnico_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvTicketsOperativos)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private Label lblTitulo;
        private HSis.UI.Controls.IndicadorControl ucMisAsignados;
        private HSis.UI.Controls.IndicadorControl ucDisponibles;
        private HSis.UI.Controls.IndicadorControl ucCerrados;
        private HSis.UI.Controls.IndicadorControl ucCalificacion;
        private DataGridView dgvTicketsOperativos;
        private Panel pnlFiltros;
        private Label lblFiltrosTitle;
        private HSis.UI.Controls.FiltroGenericoControl filtroGenerico;
        private Button btnNuevoTicket;
        private Button btnLimpiarFiltros;
        private Button btnRecargar;
    }
}
