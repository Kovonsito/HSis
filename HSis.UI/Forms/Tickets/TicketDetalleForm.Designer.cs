using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Tickets;

partial class TicketDetalleForm
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
        lblFolio = new Label();
        cmbEstatus = new HSis.UI.Controls.ComboModerno();
        lblUsuario = new Label();
        lblEstatus = new Label();
        txtUsuario = new HSis.UI.Controls.CajaTextoModerna();
        lblAlta = new Label();
        lblDescripcion = new Label();
        txtAlta = new HSis.UI.Controls.CajaTextoModerna();
        lblSolucion = new Label();
        lblAtendido = new Label();
        lblAtencion = new Label();
        txtAtencion = new HSis.UI.Controls.CajaTextoModerna();
        lblCierre = new Label();
        txtCierre = new HSis.UI.Controls.CajaTextoModerna();
        cmbAtendido = new HSis.UI.Controls.ComboModerno();
        btnGuardar = new HSis.UI.Controls.BotonModerno();
        btnCancelar = new HSis.UI.Controls.BotonModerno();
        dgvHistorial = new DataGridView();
        lblPrioridad = new Label();
        cmbPrioridad = new HSis.UI.Controls.ComboModerno();
        lblDepartamento = new Label();
        txtDepartamento = new HSis.UI.Controls.CajaTextoModerna();
        grpFeedback = new HSis.UI.Controls.PanelCardModerno();
        lblEstrellas = new Label();
        cmbEstrellas = new HSis.UI.Controls.ComboModerno();
        lblComentario = new Label();
        txtComentario = new HSis.UI.Controls.CajaTextoModerna();
        btnEnviar = new HSis.UI.Controls.BotonModerno();
        lblResumen = new Label();
        lblComentarioLectura = new Label();
        tabControlTicket = new TabControl();
        tabInfoGeneral = new TabPage();
        tabDescripcionSolucion = new TabPage();
        tbpHistorial = new TabPage();
        tbpFeedback = new TabPage();
        ((System.ComponentModel.ISupportInitialize)dgvHistorial).BeginInit();
        grpFeedback.SuspendLayout();
        tabControlTicket.SuspendLayout();
        tabInfoGeneral.SuspendLayout();
        tabDescripcionSolucion.SuspendLayout();
        tbpHistorial.SuspendLayout();
        tbpFeedback.SuspendLayout();
        SuspendLayout();
        // 
        // lblFolio
        // 
        lblFolio.AutoSize = true;
        lblFolio.Location = new Point(12, 15);
        lblFolio.Name = "lblFolio";
        lblFolio.Size = new Size(39, 15);
        lblFolio.TabIndex = 0;
        lblFolio.Text = "Folio: ";
        // 
        // cmbEstatus
        // 
        cmbEstatus.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmbEstatus.FormattingEnabled = true;
        cmbEstatus.Items.AddRange(new object[] { "Abierto", "En proceso", "Cerrado", "Reabierto", "Abierto", "En proceso", "Cerrado", "Reabierto" });
        cmbEstatus.Location = new Point(134, 93);
        cmbEstatus.Name = "cmbEstatus";
        cmbEstatus.Size = new Size(540, 23);
        cmbEstatus.TabIndex = 2;
        cmbEstatus.SelectedIndexChanged += CmbEstatus_SelectedIndexChanged;
        // 
        // lblUsuario
        // 
        lblUsuario.AutoSize = true;
        lblUsuario.Location = new Point(12, 43);
        lblUsuario.Name = "lblUsuario";
        lblUsuario.Size = new Size(47, 15);
        lblUsuario.TabIndex = 3;
        lblUsuario.Text = "Usuario";
        // 
        // lblEstatus
        // 
        lblEstatus.AutoSize = true;
        lblEstatus.Location = new Point(12, 101);
        lblEstatus.Name = "lblEstatus";
        lblEstatus.Size = new Size(44, 15);
        lblEstatus.TabIndex = 4;
        lblEstatus.Text = "Estatus";
        // 
        // txtUsuario
        // 
        txtUsuario.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtUsuario.Enabled = false;
        txtUsuario.Location = new Point(134, 35);
        txtUsuario.Name = "txtUsuario";
        txtUsuario.Size = new Size(540, 23);
        txtUsuario.TabIndex = 7;
        // 
        // lblAlta
        // 
        lblAlta.AutoSize = true;
        lblAlta.Location = new Point(12, 157);
        lblAlta.Name = "lblAlta";
        lblAlta.Size = new Size(65, 15);
        lblAlta.TabIndex = 9;
        lblAlta.Text = "Fecha Alta:";
        // 
        // lblDescripcion
        // 
        lblDescripcion.AutoSize = true;
        lblDescripcion.Location = new Point(12, 15);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(69, 15);
        lblDescripcion.TabIndex = 1;
        lblDescripcion.Text = "Descripción";
        // 
        // txtAlta
        // 
        txtAlta.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtAlta.Location = new Point(134, 151);
        txtAlta.Name = "txtAlta";
        txtAlta.Size = new Size(540, 23);
        txtAlta.TabIndex = 10;
        txtAlta.ReadOnly = true;
        // 
        // lblSolucion
        // 
        lblSolucion.AutoSize = true;
        lblSolucion.Location = new Point(12, 230);
        lblSolucion.Name = "lblSolucion";
        lblSolucion.Size = new Size(53, 15);
        lblSolucion.TabIndex = 11;
        lblSolucion.Text = "Solución";
        // 
        // lblAtendido
        // 
        lblAtendido.AutoSize = true;
        lblAtendido.Location = new Point(12, 241);
        lblAtendido.Name = "lblAtendido";
        lblAtendido.Size = new Size(80, 15);
        lblAtendido.TabIndex = 24;
        lblAtendido.Text = "Atendido por:";
        // 
        // lblAtencion
        // 
        lblAtencion.AutoSize = true;
        lblAtencion.Location = new Point(12, 188);
        lblAtencion.Name = "lblAtencion";
        lblAtencion.Size = new Size(92, 15);
        lblAtencion.TabIndex = 14;
        lblAtencion.Text = "Fecha Atención:";
        // 
        // txtAtencion
        // 
        txtAtencion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtAtencion.Location = new Point(134, 180);
        txtAtencion.Name = "txtAtencion";
        txtAtencion.Size = new Size(540, 23);
        txtAtencion.TabIndex = 15;
        txtAtencion.ReadOnly = true;
        // 
        // lblCierre
        // 
        lblCierre.AutoSize = true;
        lblCierre.Location = new Point(12, 217);
        lblCierre.Name = "lblCierre";
        lblCierre.Size = new Size(75, 15);
        lblCierre.TabIndex = 16;
        lblCierre.Text = "Fecha Cierre:";
        // 
        // txtCierre
        // 
        txtCierre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtCierre.Location = new Point(134, 209);
        txtCierre.Name = "txtCierre";
        txtCierre.Size = new Size(540, 23);
        txtCierre.TabIndex = 17;
        txtCierre.ReadOnly = true;
        // 
        // cmbAtendido
        // 
        cmbAtendido.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmbAtendido.Location = new Point(134, 238);
        cmbAtendido.Name = "cmbAtendido";
        cmbAtendido.Size = new Size(540, 23);
        cmbAtendido.TabIndex = 23;
        // 
        // btnGuardar
        // 
        btnGuardar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnGuardar.Estilo = EstiloBotonModerno.Exito;
        btnGuardar.Icono = FontAwesome.Sharp.IconChar.FloppyDisk;
        btnGuardar.IconoTamano = 14;
        btnGuardar.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        btnGuardar.Location = new Point(470, 488);
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Size = new Size(140, 36);
        btnGuardar.TabIndex = 19;
        btnGuardar.Text = "Guardar";
        btnGuardar.Click += btnGuardar_Click;
        // 
        // btnCancelar
        // 
        btnCancelar.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        btnCancelar.Estilo = EstiloBotonModerno.Secundario;
        btnCancelar.Icono = FontAwesome.Sharp.IconChar.Xmark;
        btnCancelar.IconoTamano = 14;
        btnCancelar.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        btnCancelar.Location = new Point(620, 488);
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Size = new Size(110, 36);
        btnCancelar.TabIndex = 20;
        btnCancelar.Text = "Cancelar";
        btnCancelar.Click += btnCancelar_Click;
        // 
        // dgvHistorial
        // 
        dgvHistorial.AllowUserToAddRows = false;
        dgvHistorial.AllowUserToDeleteRows = false;
        dgvHistorial.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvHistorial.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
        dgvHistorial.Dock = DockStyle.Fill;
        dgvHistorial.Location = new Point(3, 3);
        dgvHistorial.Name = "dgvHistorial";
        dgvHistorial.ReadOnly = true;
        dgvHistorial.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvHistorial.Size = new Size(693, 441);
        dgvHistorial.TabIndex = 21;
        // 
        // lblPrioridad
        // 
        lblPrioridad.AutoSize = true;
        lblPrioridad.Location = new Point(12, 130);
        lblPrioridad.Name = "lblPrioridad";
        lblPrioridad.Size = new Size(58, 15);
        lblPrioridad.TabIndex = 25;
        lblPrioridad.Text = "Prioridad:";
        // 
        // cmbPrioridad
        // 
        cmbPrioridad.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        cmbPrioridad.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbPrioridad.FormattingEnabled = true;
        cmbPrioridad.Items.AddRange(new object[] { "Alta", "Media", "Baja" });
        cmbPrioridad.Location = new Point(134, 122);
        cmbPrioridad.Name = "cmbPrioridad";
        cmbPrioridad.Size = new Size(540, 23);
        cmbPrioridad.TabIndex = 26;
        // 
        // lblDepartamento
        // 
        lblDepartamento.AutoSize = true;
        lblDepartamento.Location = new Point(12, 72);
        lblDepartamento.Name = "lblDepartamento";
        lblDepartamento.Size = new Size(83, 15);
        lblDepartamento.TabIndex = 27;
        lblDepartamento.Text = "Departamento";
        // 
        // txtDepartamento
        // 
        txtDepartamento.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtDepartamento.Enabled = false;
        txtDepartamento.Location = new Point(134, 64);
        txtDepartamento.Name = "txtDepartamento";
        txtDepartamento.Size = new Size(540, 23);
        txtDepartamento.TabIndex = 28;
        // 
        // grpFeedback
        // 
        grpFeedback.Controls.Add(lblEstrellas);
        grpFeedback.Controls.Add(cmbEstrellas);
        grpFeedback.Controls.Add(lblComentario);
        grpFeedback.Controls.Add(txtComentario);
        grpFeedback.Controls.Add(btnEnviar);
        grpFeedback.Controls.Add(lblResumen);
        grpFeedback.Controls.Add(lblComentarioLectura);
        grpFeedback.Dock = DockStyle.Fill;
        grpFeedback.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpFeedback.Location = new Point(3, 3);
        grpFeedback.Name = "grpFeedback";
        grpFeedback.Size = new Size(693, 438);
        grpFeedback.TabIndex = 0;
        grpFeedback.Titulo = "Retroalimentación de la Atención";
        grpFeedback.Icono = FontAwesome.Sharp.IconChar.Star;
        // 
        // lblEstrellas
        // 
        lblEstrellas.AutoSize = true;
        lblEstrellas.Font = new Font("Segoe UI", 9F);
        lblEstrellas.Location = new Point(15, 52);
        lblEstrellas.Name = "lblEstrellas";
        lblEstrellas.Size = new Size(110, 15);
        lblEstrellas.TabIndex = 0;
        lblEstrellas.Text = "Calificación (1 al 5):";
        // 
        // cmbEstrellas
        // 
        cmbEstrellas.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbEstrellas.Font = new Font("Segoe UI", 9F);
        cmbEstrellas.Items.AddRange(new object[] { "1 - Muy Malo", "2 - Malo", "3 - Regular", "4 - Bueno", "5 - Excelente" });
        cmbEstrellas.Location = new Point(140, 46);
        cmbEstrellas.Name = "cmbEstrellas";
        cmbEstrellas.Size = new Size(140, 32);
        cmbEstrellas.TabIndex = 1;
        // 
        // lblComentario
        // 
        lblComentario.AutoSize = true;
        lblComentario.Font = new Font("Segoe UI", 9F);
        lblComentario.Location = new Point(15, 88);
        lblComentario.Name = "lblComentario";
        lblComentario.Size = new Size(132, 15);
        lblComentario.TabIndex = 2;
        lblComentario.Text = "Comentario (Opcional):";
        // 
        // txtComentario
        // 
        txtComentario.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        txtComentario.Font = new Font("Segoe UI", 9F);
        txtComentario.Location = new Point(15, 108);
        txtComentario.Name = "txtComentario";
        txtComentario.Placeholder = "Escribe un comentario sobre la atención recibida...";
        txtComentario.Size = new Size(500, 34);
        txtComentario.TabIndex = 3;
        // 
        // btnEnviar
        // 
        btnEnviar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnEnviar.Estilo = EstiloBotonModerno.Primario;
        btnEnviar.Icono = FontAwesome.Sharp.IconChar.Star;
        btnEnviar.IconoTamano = 14;
        btnEnviar.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        btnEnviar.Location = new Point(525, 70);
        btnEnviar.Name = "btnEnviar";
        btnEnviar.Size = new Size(155, 36);
        btnEnviar.TabIndex = 4;
        btnEnviar.Text = "Enviar Feedback";
        btnEnviar.Click += btnEnviarFeedback_Click;
        // 
        // lblResumen
        // 
        lblResumen.AutoSize = true;
        lblResumen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblResumen.ForeColor = Color.FromArgb(41, 128, 185);
        lblResumen.Location = new Point(15, 25);
        lblResumen.Name = "lblResumen";
        lblResumen.Size = new Size(0, 19);
        lblResumen.TabIndex = 5;
        // 
        // lblComentarioLectura
        // 
        lblComentarioLectura.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        lblComentarioLectura.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblComentarioLectura.Location = new Point(15, 55);
        lblComentarioLectura.Name = "lblComentarioLectura";
        lblComentarioLectura.Size = new Size(650, 45);
        lblComentarioLectura.TabIndex = 6;
        // 
        // tabControlTicket
        // 
        tabControlTicket.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        tabControlTicket.Controls.Add(tabInfoGeneral);
        tabControlTicket.Controls.Add(tabDescripcionSolucion);
        tabControlTicket.Controls.Add(tbpHistorial);
        tabControlTicket.Controls.Add(tbpFeedback);
        tabControlTicket.Location = new Point(1, 12);
        tabControlTicket.Name = "tabControlTicket";
        tabControlTicket.SelectedIndex = 0;
        tabControlTicket.Size = new Size(707, 472);
        tabControlTicket.TabIndex = 0;
        // 
        // tabInfoGeneral
        // 
        tabInfoGeneral.AutoScroll = true;
        tabInfoGeneral.Controls.Add(lblFolio);
        tabInfoGeneral.Controls.Add(lblUsuario);
        tabInfoGeneral.Controls.Add(txtUsuario);
        tabInfoGeneral.Controls.Add(lblDepartamento);
        tabInfoGeneral.Controls.Add(txtDepartamento);
        tabInfoGeneral.Controls.Add(lblEstatus);
        tabInfoGeneral.Controls.Add(cmbEstatus);
        tabInfoGeneral.Controls.Add(lblPrioridad);
        tabInfoGeneral.Controls.Add(cmbPrioridad);
        tabInfoGeneral.Controls.Add(lblAlta);
        tabInfoGeneral.Controls.Add(txtAlta);
        tabInfoGeneral.Controls.Add(lblAtencion);
        tabInfoGeneral.Controls.Add(txtAtencion);
        tabInfoGeneral.Controls.Add(lblCierre);
        tabInfoGeneral.Controls.Add(txtCierre);
        tabInfoGeneral.Controls.Add(lblAtendido);
        tabInfoGeneral.Controls.Add(cmbAtendido);
        tabInfoGeneral.Location = new Point(4, 24);
        tabInfoGeneral.Name = "tabInfoGeneral";
        tabInfoGeneral.Padding = new Padding(3);
        tabInfoGeneral.Size = new Size(699, 444);
        tabInfoGeneral.TabIndex = 0;
        tabInfoGeneral.Text = "Información General";
        tabInfoGeneral.UseVisualStyleBackColor = true;
        // 
        // tabDescripcionSolucion
        // 
        tabDescripcionSolucion.AutoScroll = true;
        tabDescripcionSolucion.Controls.Add(lblDescripcion);
        tabDescripcionSolucion.Controls.Add(lblSolucion);
        tabDescripcionSolucion.Location = new Point(4, 24);
        tabDescripcionSolucion.Name = "tabDescripcionSolucion";
        tabDescripcionSolucion.Padding = new Padding(3);
        tabDescripcionSolucion.Size = new Size(699, 444);
        tabDescripcionSolucion.TabIndex = 1;
        tabDescripcionSolucion.Text = "Descripción y Solución";
        tabDescripcionSolucion.UseVisualStyleBackColor = true;
        // 
        // tbpHistorial
        // 
        tbpHistorial.AutoScroll = true;
        tbpHistorial.Controls.Add(dgvHistorial);
        tbpHistorial.Location = new Point(4, 24);
        tbpHistorial.Name = "tbpHistorial";
        tbpHistorial.Padding = new Padding(3);
        tbpHistorial.Size = new Size(699, 447);
        tbpHistorial.TabIndex = 2;
        tbpHistorial.Text = "Historial de cambios";
        tbpHistorial.UseVisualStyleBackColor = true;
        // 
        // tbpFeedback
        // 
        tbpFeedback.AutoScroll = true;
        tbpFeedback.Controls.Add(grpFeedback);
        tbpFeedback.Location = new Point(4, 24);
        tbpFeedback.Name = "tbpFeedback";
        tbpFeedback.Padding = new Padding(3);
        tbpFeedback.Size = new Size(699, 444);
        tbpFeedback.TabIndex = 3;
        tbpFeedback.Text = "Retroalimentación";
        tbpFeedback.UseVisualStyleBackColor = true;
        // 
        // TicketDetalleForm
        // 
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = TemaVisual.FondoApp;
        ClientSize = new Size(709, 534);
        Controls.Add(tabControlTicket);
        Controls.Add(btnCancelar);
        Controls.Add(btnGuardar);
        MinimumSize = new Size(600, 500);
        Name = "TicketDetalleForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "FormularioTicket";
        Load += FormularioTicket_Load;
        ((System.ComponentModel.ISupportInitialize)dgvHistorial).EndInit();
        grpFeedback.ResumeLayout(false);
        grpFeedback.PerformLayout();
        tabControlTicket.ResumeLayout(false);
        tabInfoGeneral.ResumeLayout(false);
        tabInfoGeneral.PerformLayout();
        tabDescripcionSolucion.ResumeLayout(false);
        tabDescripcionSolucion.PerformLayout();
        tbpHistorial.ResumeLayout(false);
        tbpFeedback.ResumeLayout(false);
        ResumeLayout(false);
    }

    #endregion

    private Label lblFolio;
    private HSis.UI.Controls.ComboModerno cmbEstatus;
    private Label lblUsuario;
    private HSis.UI.Controls.CajaTextoModerna txtUsuario;
    private Label lblDescripcion;
    private Label lblAlta;
    private Label lblEstatus;
    private HSis.UI.Controls.CajaTextoModerna txtAlta;
    private Label lblSolucion;
    private Label lblAtendido;
    private Label lblAtencion;
    private HSis.UI.Controls.CajaTextoModerna txtAtencion;
    private Label lblCierre;
    private HSis.UI.Controls.CajaTextoModerna txtCierre;
    private HSis.UI.Controls.ComboModerno cmbAtendido;
    private HSis.UI.Controls.BotonModerno btnGuardar;
    private HSis.UI.Controls.BotonModerno btnCancelar;
    private DataGridView dgvHistorial;
    private Label lblPrioridad;
    private HSis.UI.Controls.ComboModerno cmbPrioridad;
    private Label lblDepartamento;
    private HSis.UI.Controls.CajaTextoModerna txtDepartamento;
    private HSis.UI.Controls.PanelCardModerno grpFeedback;
    private Label lblEstrellas;
    private HSis.UI.Controls.ComboModerno cmbEstrellas;
    private Label lblComentario;
    private HSis.UI.Controls.CajaTextoModerna txtComentario;
    private HSis.UI.Controls.BotonModerno btnEnviar;
    private Label lblResumen;
    private Label lblComentarioLectura;
    private TabControl tabControlTicket;
    private TabPage tabInfoGeneral;
    private TabPage tabDescripcionSolucion;
    private TabPage tbpHistorial;
    private TabPage tbpFeedback;
    private TabPage tbpMateriales = null!;
    private PanelCardModerno pnlFeedbackEstado = null!;
    private Label lblFeedbackEstado = null!;
    // rtbSolucion y rtbDescripcion serán creados dinámicamente en el código del formulario

    private void InicializarLayoutDetalle()
    {
        BackColor = TemaVisual.FondoApp;
        Font = TemaVisual.FuenteNormal;
        ClientSize = new Size(920, 680);
        MinimumSize = new Size(760, 600);

        rtbDescripcion = new CajaTextoOrtograficaWpf
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            BackColor = Color.White
        };
        rtbSolucion = new CajaTextoOrtograficaWpf
        {
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            BackColor = Color.White
        };

        dgvMateriales = new DataGridView
        {
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };
        dgvMateriales.AplicarTemaModerno();
        dgvHistorial.AplicarTemaModerno();

        ConfigurarPagina(tabInfoGeneral);
        ConfigurarPagina(tabDescripcionSolucion);
        ConfigurarPagina(tbpHistorial);
        ConfigurarPagina(tbpFeedback);

        tabInfoGeneral.Controls.Clear();
        tabDescripcionSolucion.Controls.Clear();
        tbpHistorial.Controls.Clear();
        tbpFeedback.Controls.Clear();

        txtUsuario.Enabled = true;
        txtUsuario.ReadOnly = true;
        txtDepartamento.Enabled = true;
        txtDepartamento.ReadOnly = true;
        txtAlta.ReadOnly = true;
        txtAtencion.ReadOnly = true;
        txtCierre.ReadOnly = true;

        // Pestaña de información: separar los datos de contexto de la gestión operativa.
        var tablaInfo = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0),
            RowCount = 1
        };
        tablaInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48F));
        tablaInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 52F));
        tablaInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var tarjetaSolicitante = CrearTarjeta("Información del solicitante", IconChar.Ticket);
        tarjetaSolicitante.Margin = new Padding(0, 0, 8, 0);
        var tablaSolicitante = CrearTablaCampos(3);
        lblFolio.AutoSize = false;
        lblFolio.BackColor = TemaVisual.PrimarioSuave;
        lblFolio.Dock = DockStyle.Fill;
        lblFolio.Font = TemaVisual.FuenteSubtitulo;
        lblFolio.ForeColor = TemaVisual.Primario;
        lblFolio.Margin = new Padding(0, 0, 0, 8);
        lblFolio.Padding = new Padding(12, 0, 12, 0);
        lblFolio.TextAlign = ContentAlignment.MiddleLeft;
        tablaSolicitante.Controls.Add(lblFolio, 0, 0);
        tablaSolicitante.SetColumnSpan(lblFolio, 2);
        AgregarCampo(tablaSolicitante, 1, lblUsuario, txtUsuario);
        AgregarCampo(tablaSolicitante, 2, lblDepartamento, txtDepartamento);
        tarjetaSolicitante.Controls.Add(tablaSolicitante);

        var tarjetaGestion = CrearTarjeta("Gestión y seguimiento", IconChar.ClipboardList);
        tarjetaGestion.Margin = new Padding(8, 0, 0, 0);
        var tablaGestion = CrearTablaCampos(6);
        AgregarCampo(tablaGestion, 0, lblEstatus, cmbEstatus);
        AgregarCampo(tablaGestion, 1, lblPrioridad, cmbPrioridad);
        AgregarCampo(tablaGestion, 2, lblAtendido, cmbAtendido);
        AgregarCampo(tablaGestion, 3, lblAlta, txtAlta);
        AgregarCampo(tablaGestion, 4, lblAtencion, txtAtencion);
        AgregarCampo(tablaGestion, 5, lblCierre, txtCierre);
        tarjetaGestion.Controls.Add(tablaGestion);

        tablaInfo.Controls.Add(tarjetaSolicitante, 0, 0);
        tablaInfo.Controls.Add(tarjetaGestion, 1, 0);
        tabInfoGeneral.Controls.Add(tablaInfo);

        // Pestaña de descripción: dos editores independientes para facilitar la lectura.
        var tablaDescripcion = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0),
            RowCount = 2
        };
        tablaDescripcion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tablaDescripcion.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
        tablaDescripcion.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));

        var tarjetaDescripcion = CrearTarjeta("Descripción del problema", IconChar.ClipboardList);
        tarjetaDescripcion.Margin = new Padding(0, 0, 0, 8);
        var contenidoDescripcion = CrearTablaEditor();
        ConfigurarEtiquetaEditor(lblDescripcion, "Detalle reportado por el usuario");
        contenidoDescripcion.Controls.Add(lblDescripcion, 0, 0);
        contenidoDescripcion.Controls.Add(rtbDescripcion, 0, 1);
        tarjetaDescripcion.Controls.Add(contenidoDescripcion);

        var tarjetaSolucion = CrearTarjeta("Solución aplicada", IconChar.Check);
        tarjetaSolucion.Margin = new Padding(0, 8, 0, 0);
        var contenidoSolucion = CrearTablaEditor();
        ConfigurarEtiquetaEditor(lblSolucion, "Respuesta y acciones realizadas por soporte");
        contenidoSolucion.Controls.Add(lblSolucion, 0, 0);
        contenidoSolucion.Controls.Add(rtbSolucion, 0, 1);
        tarjetaSolucion.Controls.Add(contenidoSolucion);

        tablaDescripcion.Controls.Add(tarjetaDescripcion, 0, 0);
        tablaDescripcion.Controls.Add(tarjetaSolucion, 0, 1);
        tabDescripcionSolucion.Controls.Add(tablaDescripcion);

        // Pestaña de historial: el grid se presenta dentro de una tarjeta con encabezado propio.
        var tarjetaHistorial = CrearTarjeta("Línea de tiempo de cambios", IconChar.ClipboardList);
        tarjetaHistorial.Controls.Add(dgvHistorial);
        tbpHistorial.Controls.Add(tarjetaHistorial);

        // Pestaña de retroalimentación: conservar los controles existentes, pero distribuirlos con una retícula flexible.
        grpFeedback.Controls.Clear();
        grpFeedback.BackColor = TemaVisual.FondoTarjeta;
        grpFeedback.Dock = DockStyle.Fill;
        grpFeedback.Font = TemaVisual.FuenteNormal;
        grpFeedback.Margin = new Padding(0);
        grpFeedback.Titulo = "Retroalimentación de la atención";
        grpFeedback.Icono = IconChar.Star;

        var tablaFeedback = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 3,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0),
            RowCount = 5
        };
        tablaFeedback.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 165F));
        tablaFeedback.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tablaFeedback.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 145F));
        tablaFeedback.RowStyles.Add(new RowStyle(SizeType.Absolute, 32F));
        tablaFeedback.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        tablaFeedback.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
        tablaFeedback.RowStyles.Add(new RowStyle(SizeType.Absolute, 44F));
        tablaFeedback.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        lblResumen.AutoSize = false;
        lblResumen.Dock = DockStyle.Fill;
        lblResumen.Font = TemaVisual.FuenteSubtitulo;
        lblResumen.ForeColor = TemaVisual.Primario;
        lblResumen.TextAlign = ContentAlignment.MiddleLeft;
        tablaFeedback.Controls.Add(lblResumen, 0, 0);
        tablaFeedback.SetColumnSpan(lblResumen, 3);

        ConfigurarEtiquetaCampo(lblEstrellas);
        cmbEstrellas.Dock = DockStyle.Fill;
        cmbEstrellas.Margin = new Padding(0, 3, 10, 3);
        tablaFeedback.Controls.Add(lblEstrellas, 0, 1);
        tablaFeedback.Controls.Add(cmbEstrellas, 1, 1);

        ConfigurarEtiquetaCampo(lblComentario);
        tablaFeedback.Controls.Add(lblComentario, 0, 2);
        tablaFeedback.SetColumnSpan(lblComentario, 3);

        txtComentario.Dock = DockStyle.Fill;
        txtComentario.Margin = new Padding(0, 3, 10, 3);
        btnEnviar.Dock = DockStyle.Fill;
        btnEnviar.Margin = new Padding(0, 3, 0, 3);
        tablaFeedback.Controls.Add(txtComentario, 0, 3);
        tablaFeedback.SetColumnSpan(txtComentario, 2);
        tablaFeedback.Controls.Add(btnEnviar, 2, 3);

        lblComentarioLectura.AutoSize = false;
        lblComentarioLectura.Dock = DockStyle.Fill;
        lblComentarioLectura.Font = TemaVisual.FuentePequena;
        lblComentarioLectura.ForeColor = TemaVisual.TextoSecundario;
        lblComentarioLectura.Padding = new Padding(0, 8, 0, 0);
        tablaFeedback.Controls.Add(lblComentarioLectura, 0, 4);
        tablaFeedback.SetColumnSpan(lblComentarioLectura, 3);
        grpFeedback.Controls.Add(tablaFeedback);
        grpFeedback.Visible = false;

        pnlFeedbackEstado = CrearTarjeta("Retroalimentación pendiente", IconChar.Star);
        lblFeedbackEstado = new Label
        {
            AutoSize = false,
            Dock = DockStyle.Fill,
            Font = TemaVisual.FuenteNormal,
            ForeColor = TemaVisual.TextoSecundario,
            Text = "La retroalimentación estará disponible cuando el ticket se cierre.",
            TextAlign = ContentAlignment.MiddleCenter
        };
        pnlFeedbackEstado.Controls.Add(lblFeedbackEstado);

        var panelFeedbackContenido = new Panel
        {
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0)
        };
        panelFeedbackContenido.Controls.Add(pnlFeedbackEstado);
        panelFeedbackContenido.Controls.Add(grpFeedback);
        tbpFeedback.Controls.Add(panelFeedbackContenido);

        // Pestaña de materiales.
        tbpMateriales = new TabPage("Materiales utilizados")
        {
            BackColor = TemaVisual.FondoApp,
            Name = "tbpMateriales",
            Padding = new Padding(16),
            UseVisualStyleBackColor = false
        };
        var tarjetaMateriales = CrearTarjeta("Materiales utilizados", IconChar.BoxesStacked);
        tarjetaMateriales.Controls.Add(dgvMateriales);
        tbpMateriales.Controls.Add(tarjetaMateriales);
        tabControlTicket.TabPages.Add(tbpMateriales);

        // Navegación de pestañas y envoltura general del formulario.
        tabControlTicket.BackColor = TemaVisual.FondoApp;
        tabControlTicket.DrawMode = TabDrawMode.OwnerDrawFixed;
        tabControlTicket.Font = TemaVisual.FuentePequena;
        tabControlTicket.ItemSize = new Size(145, 42);
        tabControlTicket.Padding = new Point(14, 0);
        tabControlTicket.SizeMode = TabSizeMode.Fixed;
        tabControlTicket.DrawItem -= TabControlTicket_DrawItem;
        tabControlTicket.DrawItem += TabControlTicket_DrawItem;
        tabControlTicket.SelectedIndexChanged -= TabControlTicket_SelectedIndexChanged;
        tabControlTicket.SelectedIndexChanged += TabControlTicket_SelectedIndexChanged;

        var pnlHeader = new Panel
        {
            BackColor = Color.White,
            Dock = DockStyle.Top,
            Height = 72,
            Name = "pnlHeaderDetalle"
        };
        pnlHeader.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            using (var brushCirculo = new SolidBrush(TemaVisual.Primario))
            {
                g.FillEllipse(brushCirculo, 20, 18, 36, 36);
            }

            using (var bmpIcono = IconChar.Ticket.ToBitmap(Color.White, 18))
            {
                g.DrawImageUnscaled(bmpIcono, 29, 27);
            }

            using (var brushTitulo = new SolidBrush(TemaVisual.TextoPrincipal))
            using (var fuenteTitulo = new Font("Segoe UI", 13F, FontStyle.Bold))
            {
                g.DrawString("Detalle del ticket", fuenteTitulo, brushTitulo, new PointF(70, 13));
            }

            using (var brushSubtitulo = new SolidBrush(TemaVisual.TextoSecundario))
            using (var fuenteSubtitulo = new Font("Segoe UI", 8.5F, FontStyle.Regular))
            {
                g.DrawString("Consulta y actualiza la información, gestión y seguimiento de la solicitud.", fuenteSubtitulo, brushSubtitulo, new PointF(70, 39));
            }

            using var penDivision = new Pen(TemaVisual.BordeSutil, 1F);
            g.DrawLine(penDivision, 0, pnlHeader.Height - 1, pnlHeader.Width, pnlHeader.Height - 1);
        };
        pnlHeader.Resize += (_, _) => pnlHeader.Invalidate();

        var pnlFooter = new Panel
        {
            BackColor = Color.White,
            Dock = DockStyle.Bottom,
            Height = 64,
            Name = "pnlFooterDetalle"
        };
        pnlFooter.Paint += (_, e) =>
        {
            using var penDivision = new Pen(TemaVisual.BordeSutil, 1F);
            e.Graphics.DrawLine(penDivision, 0, 0, pnlFooter.Width, 0);
        };
        pnlFooter.Resize += (_, _) => pnlFooter.Invalidate();

        var botonesFooter = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 12, 20, 12),
            WrapContents = false
        };
        btnCancelar.Anchor = AnchorStyles.None;
        btnCancelar.Margin = new Padding(10, 0, 0, 0);
        btnCancelar.Size = new Size(116, 40);
        btnGuardar.Anchor = AnchorStyles.None;
        btnGuardar.Margin = new Padding(0);
        btnGuardar.Size = new Size(145, 40);
        botonesFooter.Controls.Add(btnCancelar);
        botonesFooter.Controls.Add(btnGuardar);
        pnlFooter.Controls.Add(botonesFooter);

        var pnlBody = new Panel
        {
            BackColor = TemaVisual.FondoApp,
            Dock = DockStyle.Fill,
            Name = "pnlBodyDetalle",
            Padding = new Padding(16, 14, 16, 12)
        };
        tabControlTicket.Dock = DockStyle.Fill;
        pnlBody.Controls.Add(tabControlTicket);

        Controls.Clear();
        Controls.Add(pnlBody);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
        Text = "HSis Support - Detalle del ticket";
    }

    private static PanelCardModerno CrearTarjeta(string titulo, IconChar icono)
    {
        return new PanelCardModerno
        {
            BackColor = TemaVisual.FondoTarjeta,
            Dock = DockStyle.Fill,
            Icono = icono,
            Margin = new Padding(0),
            Titulo = titulo
        };
    }

    private static TableLayoutPanel CrearTablaCampos(int filas)
    {
        var tabla = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0),
            RowCount = filas + 1
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        for (int i = 0; i < filas; i++)
        {
            tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        }

        // La última fila absorbe el espacio sobrante; los campos mantienen una
        // altura uniforme y no convierten el último control en una zona enorme.
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        return tabla;
    }

    private static void AgregarCampo(TableLayoutPanel tabla, int fila, Label etiqueta, Control control)
    {
        ConfigurarEtiquetaCampo(etiqueta);
        control.Dock = DockStyle.Fill;
        control.Margin = new Padding(0, 3, 0, 3);
        tabla.Controls.Add(etiqueta, 0, fila);
        tabla.Controls.Add(control, 1, fila);
    }

    private static void ConfigurarEtiquetaCampo(Label etiqueta)
    {
        etiqueta.AutoSize = false;
        etiqueta.Dock = DockStyle.Fill;
        etiqueta.Font = TemaVisual.FuentePequena;
        etiqueta.ForeColor = TemaVisual.TextoMedio;
        etiqueta.Margin = new Padding(0, 0, 10, 0);
        etiqueta.TextAlign = ContentAlignment.MiddleLeft;
    }

    private static TableLayoutPanel CrearTablaEditor()
    {
        var tabla = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0),
            RowCount = 2
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        return tabla;
    }

    private static void ConfigurarEtiquetaEditor(Label etiqueta, string texto)
    {
        etiqueta.AutoSize = false;
        etiqueta.Dock = DockStyle.Fill;
        etiqueta.Font = TemaVisual.FuentePequena;
        etiqueta.ForeColor = TemaVisual.TextoSecundario;
        etiqueta.Margin = new Padding(0, 0, 0, 4);
        etiqueta.Text = texto;
        etiqueta.TextAlign = ContentAlignment.MiddleLeft;
    }

    private static void ConfigurarPagina(TabPage pagina)
    {
        pagina.AutoScroll = false;
        pagina.BackColor = TemaVisual.FondoApp;
        pagina.Padding = new Padding(16);
        pagina.UseVisualStyleBackColor = false;
    }

    private void TabControlTicket_SelectedIndexChanged(object sender, EventArgs e)
    {
        tabControlTicket.Invalidate();
    }

    private void TabControlTicket_DrawItem(object sender, DrawItemEventArgs e)
    {
        if (sender is not TabControl tabControl || e.Index < 0 || e.Index >= tabControl.TabPages.Count)
        {
            return;
        }

        bool seleccionado = e.Index == tabControl.SelectedIndex;
        var rect = e.Bounds;
        rect.Inflate(-2, -2);

        using var brushFondo = new SolidBrush(seleccionado ? TemaVisual.FondoTarjeta : TemaVisual.FondoTenue);
        using var penBorde = new Pen(seleccionado ? TemaVisual.Primario : TemaVisual.BordeSutil, seleccionado ? 1.5F : 1F);
        e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        e.Graphics.FillRectangle(brushFondo, rect);
        e.Graphics.DrawRectangle(penBorde, rect);

        Color colorTexto = seleccionado ? TemaVisual.Primario : TemaVisual.TextoSecundario;
        TextRenderer.DrawText(
            e.Graphics,
            tabControl.TabPages[e.Index].Text,
            seleccionado ? TemaVisual.FuenteSubtitulo : TemaVisual.FuentePequena,
            rect,
            colorTexto,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis);
    }
}