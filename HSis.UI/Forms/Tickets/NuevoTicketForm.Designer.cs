using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FontAwesome.Sharp;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Tickets;

partial class NuevoTicketForm
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

    #region Windows Form Designer generated code

    private void InitializeComponent()
    {
        lblSolicitante = new Label();
        cmbSolicitante = new ComboBox();
        lblPrioridad = new Label();
        cmbPrioridad = new ComboBox();
        lblTecnico = new Label();
        cmbTecnico = new ComboBox();
        lblDescripcion = new Label();
        chkSolicitanteEnRepresentacion = new CheckBox();
        txtNombreSolicitante = new TextBox();
        btnGuardar = new BotonModerno();
        btnCancelar = new BotonModerno();
        SuspendLayout();
        // 
        // lblSolicitante
        // 
        lblSolicitante.AutoSize = true;
        lblSolicitante.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblSolicitante.ForeColor = Color.FromArgb(51, 65, 85);
        lblSolicitante.Location = new Point(0, 0);
        lblSolicitante.Name = "lblSolicitante";
        lblSolicitante.Size = new Size(130, 17);
        lblSolicitante.TabIndex = 0;
        lblSolicitante.Text = "Cliente / Solicitante:";
        // 
        // cmbSolicitante
        // 
        cmbSolicitante.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbSolicitante.Font = new Font("Segoe UI", 9.5F);
        cmbSolicitante.Location = new Point(0, 0);
        cmbSolicitante.Name = "cmbSolicitante";
        cmbSolicitante.Size = new Size(200, 25);
        cmbSolicitante.TabIndex = 1;
        // 
        // lblPrioridad
        // 
        lblPrioridad.AutoSize = true;
        lblPrioridad.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblPrioridad.ForeColor = Color.FromArgb(51, 65, 85);
        lblPrioridad.Location = new Point(0, 0);
        lblPrioridad.Name = "lblPrioridad";
        lblPrioridad.Size = new Size(68, 17);
        lblPrioridad.TabIndex = 2;
        lblPrioridad.Text = "Prioridad:";
        // 
        // cmbPrioridad
        // 
        cmbPrioridad.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPrioridad.Font = new Font("Segoe UI", 9.5F);
        cmbPrioridad.Location = new Point(0, 0);
        cmbPrioridad.Name = "cmbPrioridad";
        cmbPrioridad.Size = new Size(130, 25);
        cmbPrioridad.TabIndex = 3;
        // 
        // lblTecnico
        // 
        lblTecnico.AutoSize = true;
        lblTecnico.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        lblTecnico.ForeColor = Color.FromArgb(51, 65, 85);
        lblTecnico.Location = new Point(0, 0);
        lblTecnico.Name = "lblTecnico";
        lblTecnico.Size = new Size(106, 17);
        lblTecnico.TabIndex = 4;
        lblTecnico.Text = "Asignar Técnico:";
        // 
        // cmbTecnico
        // 
        cmbTecnico.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbTecnico.Font = new Font("Segoe UI", 9.5F);
        cmbTecnico.Location = new Point(0, 0);
        cmbTecnico.Name = "cmbTecnico";
        cmbTecnico.Size = new Size(180, 25);
        cmbTecnico.TabIndex = 5;
        // 
        // lblDescripcion
        // 
        lblDescripcion.AutoSize = true;
        lblDescripcion.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        lblDescripcion.ForeColor = Color.FromArgb(15, 23, 42);
        lblDescripcion.Location = new Point(0, 0);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(185, 19);
        lblDescripcion.TabIndex = 6;
        lblDescripcion.Text = "Descripción del Problema *";
        // 
        // chkSolicitanteEnRepresentacion
        // 
        chkSolicitanteEnRepresentacion.AutoSize = true;
        chkSolicitanteEnRepresentacion.Font = new Font("Segoe UI", 9F);
        chkSolicitanteEnRepresentacion.ForeColor = Color.FromArgb(71, 85, 105);
        chkSolicitanteEnRepresentacion.Location = new Point(0, 0);
        chkSolicitanteEnRepresentacion.Name = "chkSolicitanteEnRepresentacion";
        chkSolicitanteEnRepresentacion.Size = new Size(230, 19);
        chkSolicitanteEnRepresentacion.TabIndex = 7;
        chkSolicitanteEnRepresentacion.Text = "Solicitante no registrado en el sistema";
        chkSolicitanteEnRepresentacion.CheckedChanged += chkSolicitanteEnRepresentacion_CheckedChanged;
        // 
        // txtNombreSolicitante
        // 
        txtNombreSolicitante.Enabled = false;
        txtNombreSolicitante.Font = new Font("Segoe UI", 9.5F);
        txtNombreSolicitante.Location = new Point(0, 0);
        txtNombreSolicitante.Name = "txtNombreSolicitante";
        txtNombreSolicitante.PlaceholderText = "Nombre del solicitante no registrado...";
        txtNombreSolicitante.Size = new Size(200, 24);
        txtNombreSolicitante.TabIndex = 8;
        // 
        // btnGuardar
        // 
        btnGuardar.Estilo = EstiloBotonModerno.Primario;
        btnGuardar.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnGuardar.Icono = IconChar.PaperPlane;
        btnGuardar.IconoTamano = 15;
        btnGuardar.Location = new Point(0, 0);
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(145, 38);
        btnGuardar.TabIndex = 9;
        btnGuardar.Text = "Crear Ticket";
        btnGuardar.Click += btnGuardar_Click;
        // 
        // btnCancelar
        // 
        btnCancelar.Estilo = EstiloBotonModerno.Secundario;
        btnCancelar.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
        btnCancelar.Icono = IconChar.Xmark;
        btnCancelar.IconoTamano = 14;
        btnCancelar.Location = new Point(0, 0);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(110, 38);
        btnCancelar.TabIndex = 10;
        btnCancelar.Text = "Cancelar";
        btnCancelar.Click += btnCancelar_Click;
        // 
        // NuevoTicketForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(248, 250, 252);
        ClientSize = new Size(620, 520);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        Name = "NuevoTicketForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "HSis Support - Nuevo Ticket";
        Load += frmNuevoTicket_Load;
        ResumeLayout(false);
    }

    #endregion

    private Label lblSolicitante;
    private ComboBox cmbSolicitante;
    private Label lblPrioridad;
    private ComboBox cmbPrioridad;
    private Label lblTecnico;
    private ComboBox cmbTecnico;
    private Label lblDescripcion;
    private CheckBox chkSolicitanteEnRepresentacion;
    private TextBox txtNombreSolicitante;
    private BotonModerno btnGuardar;
    private BotonModerno btnCancelar;

    private void InicializarLayoutNuevoTicket()
    {
        rtbDescripcion = new CajaTextoOrtograficaWpf();
        bool esPerfilElevado = SesionSistema.EsAdmin || SesionSistema.EsTecnico;

        this.ClientSize = new Size(620, esPerfilElevado ? 560 : 460);

        // 1. Header Card con branding
        var pnlHeader = new Panel
        {
            Dock = DockStyle.Top,
            Height = 64,
            BackColor = Color.White
        };
        pnlHeader.Paint += (s, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;

            // Icono en círculo azul
            using (var brushCircle = new SolidBrush(Color.FromArgb(37, 99, 235)))
            {
                g.FillEllipse(brushCircle, 18, 14, 36, 36);
            }

            using (var bmpIcon = IconChar.Ticket.ToBitmap(Color.White, 18))
            {
                g.DrawImage(bmpIcon, 27, 23);
            }

            using (var brushTitle = new SolidBrush(Color.FromArgb(15, 23, 42)))
            using (var fontTitle = new Font("Segoe UI", 12.5f, FontStyle.Bold))
            {
                g.DrawString("Crear Nuevo Ticket de Servicio", fontTitle, brushTitle, new PointF(62, 12));
            }

            using (var brushSub = new SolidBrush(Color.FromArgb(100, 116, 139)))
            using (var fontSub = new Font("Segoe UI", 8.5f, FontStyle.Regular))
            {
                g.DrawString("Describe tu solicitud o incidente para que el equipo de soporte pueda atenderlo.", fontSub, brushSub, new PointF(62, 34));
            }

            using var penDiv = new Pen(Color.FromArgb(226, 232, 240), 1f);
            g.DrawLine(penDiv, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        };

        // 2. Footer Action Bar
        var pnlFooter = new Panel
        {
            Dock = DockStyle.Bottom,
            Height = 58,
            BackColor = Color.White
        };
        pnlFooter.Paint += (s, e) =>
        {
            using var penDiv = new Pen(Color.FromArgb(226, 232, 240), 1f);
            e.Graphics.DrawLine(penDiv, 0, 0, pnlFooter.Width, 0);
        };

        var flpBotones = new FlowLayoutPanel
        {
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Padding = new Padding(0, 10, 16, 10)
        };
        btnCancelar.Margin = new Padding(10, 0, 0, 0);
        btnGuardar.Margin = new Padding(0);
        flpBotones.Controls.Add(btnCancelar);
        flpBotones.Controls.Add(btnGuardar);
        pnlFooter.Controls.Add(flpBotones);

        // 3. Body Content Container
        var pnlBody = new Panel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(18, 14, 18, 10),
            BackColor = Color.FromArgb(248, 250, 252)
        };

        var tblPrincipal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = esPerfilElevado ? 5 : 3,
            ColumnCount = 1
        };

        if (esPerfilElevado)
        {
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Solicitante
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Representación
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Prioridad / Técnico
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Label Descripción
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Caja de texto

            // Solicitante
            var pnlSolicitante = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                Margin = new Padding(0, 0, 0, 8)
            };
            pnlSolicitante.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlSolicitante.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            lblSolicitante.Margin = new Padding(0, 4, 10, 0);
            cmbSolicitante.Dock = DockStyle.Fill;
            pnlSolicitante.Controls.Add(lblSolicitante, 0, 0);
            pnlSolicitante.Controls.Add(cmbSolicitante, 1, 0);

            // Representación
            var pnlRepresentacion = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 2,
                Margin = new Padding(0, 0, 0, 8)
            };
            pnlRepresentacion.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlRepresentacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            chkSolicitanteEnRepresentacion.Margin = new Padding(0, 4, 10, 0);
            txtNombreSolicitante.Dock = DockStyle.Fill;
            pnlRepresentacion.Controls.Add(chkSolicitanteEnRepresentacion, 0, 0);
            pnlRepresentacion.Controls.Add(txtNombreSolicitante, 1, 0);

            // Prioridad & Técnico
            var pnlCamposElevados = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                RowCount = 1,
                ColumnCount = 4,
                Margin = new Padding(0, 0, 0, 10)
            };
            pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 40F));
            pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            pnlCamposElevados.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 60F));

            lblPrioridad.Margin = new Padding(0, 4, 8, 0);
            cmbPrioridad.Dock = DockStyle.Fill;
            lblTecnico.Margin = new Padding(14, 4, 8, 0);
            cmbTecnico.Dock = DockStyle.Fill;

            pnlCamposElevados.Controls.Add(lblPrioridad, 0, 0);
            pnlCamposElevados.Controls.Add(cmbPrioridad, 1, 0);
            pnlCamposElevados.Controls.Add(lblTecnico, 2, 0);
            pnlCamposElevados.Controls.Add(cmbTecnico, 3, 0);

            lblDescripcion.Dock = DockStyle.Fill;
            lblDescripcion.Margin = new Padding(0, 0, 0, 6);
            rtbDescripcion.Dock = DockStyle.Fill;
            rtbDescripcion.Margin = new Padding(0, 0, 0, 4);

            tblPrincipal.Controls.Add(pnlSolicitante, 0, 0);
            tblPrincipal.Controls.Add(pnlRepresentacion, 0, 1);
            tblPrincipal.Controls.Add(pnlCamposElevados, 0, 2);
            tblPrincipal.Controls.Add(lblDescripcion, 0, 3);
            tblPrincipal.Controls.Add(rtbDescripcion, 0, 4);
        }
        else
        {
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Label Descripción
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F)); // Caja de texto
            tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize)); // Nota informativa

            lblDescripcion.Dock = DockStyle.Fill;
            lblDescripcion.Margin = new Padding(0, 0, 0, 6);
            rtbDescripcion.Dock = DockStyle.Fill;
            rtbDescripcion.Margin = new Padding(0, 0, 0, 8);

            var lblNota = new Label
            {
                Text = "ℹ️ Tu ticket será asignado automáticamente y el equipo de soporte te dará seguimiento.",
                Font = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = Color.FromArgb(100, 116, 139),
                Dock = DockStyle.Fill,
                AutoSize = true,
                Margin = new Padding(0, 2, 0, 0)
            };

            tblPrincipal.Controls.Add(lblDescripcion, 0, 0);
            tblPrincipal.Controls.Add(rtbDescripcion, 0, 1);
            tblPrincipal.Controls.Add(lblNota, 0, 2);
        }

        pnlBody.Controls.Add(tblPrincipal);

        this.Controls.Clear();
        this.Controls.Add(pnlBody);
        this.Controls.Add(pnlFooter);
        this.Controls.Add(pnlHeader);
    }
}
