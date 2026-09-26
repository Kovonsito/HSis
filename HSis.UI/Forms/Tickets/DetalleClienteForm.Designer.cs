using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using FontAwesome.Sharp;
using HSis.UI.Controls;
using HSis.UI.Helpers;

namespace HSis.UI.Forms.Tickets;

partial class DetalleClienteForm
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
        lblFolioValor = new Label();
        lblFechaAlta = new Label();
        lblFechaAltaValor = new Label();
        lblEstatus = new Label();
        lblEstatusValor = new Label();
        lblTecnico = new Label();
        lblTecnicoValor = new Label();
        lblDescripcion = new Label();
        txtDescripcion = new HSis.UI.Controls.CajaTextoModerna();
        lblSolucion = new Label();
        txtSolucion = new HSis.UI.Controls.CajaTextoModerna();
        btnCerrar = new HSis.UI.Controls.BotonModerno();
        lblFechaCierre = new Label();
        lblFechaCierreValor = new Label();
        grpFeedback = new HSis.UI.Controls.PanelCardModerno();
        lblEstrellas = new Label();
        lblStar1 = new Label();
        lblStar2 = new Label();
        lblStar3 = new Label();
        lblStar4 = new Label();
        lblStar5 = new Label();
        lblComentario = new Label();
        txtComentario = new HSis.UI.Controls.CajaTextoModerna();
        btnEnviar = new HSis.UI.Controls.BotonModerno();
        lblResumen = new Label();
        lblComentarioLectura = new Label();
        grpFeedback.SuspendLayout();
        SuspendLayout();
        // 
        // lblFolio
        // 
        lblFolio.AutoSize = true;
        lblFolio.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblFolio.Location = new Point(12, 15);
        lblFolio.Name = "lblFolio";
        lblFolio.Size = new Size(47, 20);
        lblFolio.TabIndex = 0;
        lblFolio.Text = "Folio:";
        // 
        // lblFolioValor
        // 
        lblFolioValor.AutoSize = true;
        lblFolioValor.Font = new Font("Segoe UI", 11F);
        lblFolioValor.Location = new Point(60, 15);
        lblFolioValor.Name = "lblFolioValor";
        lblFolioValor.Size = new Size(36, 20);
        lblFolioValor.TabIndex = 1;
        lblFolioValor.Text = "N/A";
        // 
        // lblFechaAlta
        // 
        lblFechaAlta.AutoSize = true;
        lblFechaAlta.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblFechaAlta.Location = new Point(12, 40);
        lblFechaAlta.Name = "lblFechaAlta";
        lblFechaAlta.Size = new Size(86, 20);
        lblFechaAlta.TabIndex = 2;
        lblFechaAlta.Text = "Fecha Alta:";
        // 
        // lblFechaAltaValor
        // 
        lblFechaAltaValor.AutoSize = true;
        lblFechaAltaValor.Font = new Font("Segoe UI", 11F);
        lblFechaAltaValor.Location = new Point(101, 40);
        lblFechaAltaValor.Name = "lblFechaAltaValor";
        lblFechaAltaValor.Size = new Size(36, 20);
        lblFechaAltaValor.TabIndex = 3;
        lblFechaAltaValor.Text = "N/A";
        // 
        // lblEstatus
        // 
        lblEstatus.AutoSize = true;
        lblEstatus.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblEstatus.Location = new Point(12, 65);
        lblEstatus.Name = "lblEstatus";
        lblEstatus.Size = new Size(64, 20);
        lblEstatus.TabIndex = 4;
        lblEstatus.Text = "Estatus:";
        // 
        // lblEstatusValor
        // 
        lblEstatusValor.AutoSize = true;
        lblEstatusValor.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblEstatusValor.Location = new Point(71, 60);
        lblEstatusValor.Name = "lblEstatusValor";
        lblEstatusValor.Padding = new Padding(5);
        lblEstatusValor.Size = new Size(49, 30);
        lblEstatusValor.TabIndex = 5;
        lblEstatusValor.Text = "N/A";
        // 
        // lblTecnico
        // 
        lblTecnico.AutoSize = true;
        lblTecnico.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblTecnico.Location = new Point(12, 100);
        lblTecnico.Name = "lblTecnico";
        lblTecnico.Size = new Size(135, 20);
        lblTecnico.TabIndex = 6;
        lblTecnico.Text = "Técnico Asignado:";
        // 
        // lblTecnicoValor
        // 
        lblTecnicoValor.AutoSize = true;
        lblTecnicoValor.Font = new Font("Segoe UI", 11F);
        lblTecnicoValor.Location = new Point(152, 100);
        lblTecnicoValor.Name = "lblTecnicoValor";
        lblTecnicoValor.Size = new Size(36, 20);
        lblTecnicoValor.TabIndex = 7;
        lblTecnicoValor.Text = "N/A";
        // 
        // lblDescripcion
        // 
        lblDescripcion.AutoSize = true;
        lblDescripcion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblDescripcion.Location = new Point(12, 125);
        lblDescripcion.Name = "lblDescripcion";
        lblDescripcion.Size = new Size(94, 20);
        lblDescripcion.TabIndex = 8;
        lblDescripcion.Text = "Descripción:";
        // 
        // txtDescripcion
        // 
        txtDescripcion.BackColor = SystemColors.Control;
        txtDescripcion.Font = new Font("Segoe UI", 11F);
        txtDescripcion.Location = new Point(12, 145);
        txtDescripcion.Multiline = true;
        txtDescripcion.Name = "txtDescripcion";
        txtDescripcion.ReadOnly = true;
        txtDescripcion.Size = new Size(496, 80);
        txtDescripcion.TabIndex = 9;
        // 
        // lblSolucion
        // 
        lblSolucion.AutoSize = true;
        lblSolucion.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblSolucion.Location = new Point(12, 235);
        lblSolucion.Name = "lblSolucion";
        lblSolucion.Size = new Size(72, 20);
        lblSolucion.TabIndex = 10;
        lblSolucion.Text = "Solución:";
        // 
        // txtSolucion
        // 
        txtSolucion.BackColor = SystemColors.Control;
        txtSolucion.Font = new Font("Segoe UI", 11F);
        txtSolucion.Location = new Point(12, 255);
        txtSolucion.Multiline = true;
        txtSolucion.Name = "txtSolucion";
        txtSolucion.ReadOnly = true;
        txtSolucion.Size = new Size(496, 80);
        txtSolucion.TabIndex = 11;
        // 
        // btnCerrar
        // 
        btnCerrar.Estilo = EstiloBotonModerno.Secundario;
        btnCerrar.Text = "Cerrar";
        btnCerrar.Size = new Size(110, 36);
        btnCerrar.Click += BtnCerrar_Click;
        // 
        // lblFechaCierre
        // 
        lblFechaCierre.AutoSize = true;
        lblFechaCierre.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        lblFechaCierre.Location = new Point(220, 40);
        lblFechaCierre.Name = "lblFechaCierre";
        lblFechaCierre.Size = new Size(98, 20);
        lblFechaCierre.TabIndex = 13;
        lblFechaCierre.Text = "Fecha Cierre:";
        lblFechaCierre.Visible = false;
        // 
        // lblFechaCierreValor
        // 
        lblFechaCierreValor.AutoSize = true;
        lblFechaCierreValor.Font = new Font("Segoe UI", 11F);
        lblFechaCierreValor.Location = new Point(321, 40);
        lblFechaCierreValor.Name = "lblFechaCierreValor";
        lblFechaCierreValor.Size = new Size(36, 20);
        lblFechaCierreValor.TabIndex = 14;
        lblFechaCierreValor.Text = "N/A";
        lblFechaCierreValor.Visible = false;
        // 
        // grpFeedback
        // 
        grpFeedback.Controls.Add(lblEstrellas);
        grpFeedback.Controls.Add(lblStar1);
        grpFeedback.Controls.Add(lblStar2);
        grpFeedback.Controls.Add(lblStar3);
        grpFeedback.Controls.Add(lblStar4);
        grpFeedback.Controls.Add(lblStar5);
        grpFeedback.Controls.Add(lblComentario);
        grpFeedback.Controls.Add(txtComentario);
        grpFeedback.Controls.Add(btnEnviar);
        grpFeedback.Controls.Add(lblResumen);
        grpFeedback.Controls.Add(lblComentarioLectura);
        grpFeedback.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        grpFeedback.Location = new Point(12, 350);
        grpFeedback.Name = "grpFeedback";
        grpFeedback.Size = new Size(496, 130);
        grpFeedback.TabIndex = 0;
        grpFeedback.Titulo = "Retroalimentación de la Atención";
        grpFeedback.Icono = FontAwesome.Sharp.IconChar.Star;
        grpFeedback.Visible = false;
        // 
        // lblEstrellas
        // 
        lblEstrellas.AutoSize = true;
        lblEstrellas.Font = new Font("Segoe UI", 9F);
        lblEstrellas.Location = new Point(15, 23);
        lblEstrellas.Name = "lblEstrellas";
        lblEstrellas.Size = new Size(110, 15);
        lblEstrellas.TabIndex = 0;
        lblEstrellas.Text = "Calificación (1 al 5):";
        // 
        // lblStar1
        // 
        lblStar1.Cursor = Cursors.Hand;
        lblStar1.Font = new Font("Segoe UI", 16F);
        lblStar1.ForeColor = Color.Gray;
        lblStar1.Location = new Point(140, 18);
        lblStar1.Name = "lblStar1";
        lblStar1.Size = new Size(30, 30);
        lblStar1.TabIndex = 1;
        lblStar1.Tag = "1";
        lblStar1.Text = "☆";
        lblStar1.Click += LblStar_Click;
        lblStar1.MouseEnter += LblStar_MouseEnter;
        lblStar1.MouseLeave += LblStar_MouseLeave;
        // 
        // lblStar2
        // 
        lblStar2.Cursor = Cursors.Hand;
        lblStar2.Font = new Font("Segoe UI", 16F);
        lblStar2.ForeColor = Color.Gray;
        lblStar2.Location = new Point(175, 18);
        lblStar2.Name = "lblStar2";
        lblStar2.Size = new Size(30, 30);
        lblStar2.TabIndex = 2;
        lblStar2.Tag = "2";
        lblStar2.Text = "☆";
        lblStar2.Click += LblStar_Click;
        lblStar2.MouseEnter += LblStar_MouseEnter;
        lblStar2.MouseLeave += LblStar_MouseLeave;
        // 
        // lblStar3
        // 
        lblStar3.Cursor = Cursors.Hand;
        lblStar3.Font = new Font("Segoe UI", 16F);
        lblStar3.ForeColor = Color.Gray;
        lblStar3.Location = new Point(210, 18);
        lblStar3.Name = "lblStar3";
        lblStar3.Size = new Size(30, 30);
        lblStar3.TabIndex = 3;
        lblStar3.Tag = "3";
        lblStar3.Text = "☆";
        lblStar3.Click += LblStar_Click;
        lblStar3.MouseEnter += LblStar_MouseEnter;
        lblStar3.MouseLeave += LblStar_MouseLeave;
        // 
        // lblStar4
        // 
        lblStar4.Cursor = Cursors.Hand;
        lblStar4.Font = new Font("Segoe UI", 16F);
        lblStar4.ForeColor = Color.Gray;
        lblStar4.Location = new Point(245, 18);
        lblStar4.Name = "lblStar4";
        lblStar4.Size = new Size(30, 30);
        lblStar4.TabIndex = 4;
        lblStar4.Tag = "4";
        lblStar4.Text = "☆";
        lblStar4.Click += LblStar_Click;
        lblStar4.MouseEnter += LblStar_MouseEnter;
        lblStar4.MouseLeave += LblStar_MouseLeave;
        // 
        // lblStar5
        // 
        lblStar5.Cursor = Cursors.Hand;
        lblStar5.Font = new Font("Segoe UI", 16F);
        lblStar5.ForeColor = Color.Gray;
        lblStar5.Location = new Point(280, 18);
        lblStar5.Name = "lblStar5";
        lblStar5.Size = new Size(30, 30);
        lblStar5.TabIndex = 5;
        lblStar5.Tag = "5";
        lblStar5.Text = "☆";
        lblStar5.Click += LblStar_Click;
        lblStar5.MouseEnter += LblStar_MouseEnter;
        lblStar5.MouseLeave += LblStar_MouseLeave;
        // 
        // lblComentario
        // 
        lblComentario.AutoSize = true;
        lblComentario.Font = new Font("Segoe UI", 9F);
        lblComentario.Location = new Point(15, 50);
        lblComentario.Name = "lblComentario";
        lblComentario.Size = new Size(132, 15);
        lblComentario.TabIndex = 6;
        lblComentario.Text = "Comentario (Opcional):";
        // 
        // txtComentario
        // 
        txtComentario.Font = new Font("Segoe UI", 9F);
        txtComentario.Location = new Point(15, 70);
        txtComentario.Name = "txtComentario";
        txtComentario.Placeholder = "Escribe un comentario sobre la atención recibida...";
        txtComentario.Size = new Size(330, 34);
        txtComentario.TabIndex = 7;
        // 
        // btnEnviar
        // 
        btnEnviar.Estilo = EstiloBotonModerno.Primario;
        btnEnviar.Text = "Enviar Calificación";
        btnEnviar.Size = new Size(140, 32);
        btnEnviar.Click += BtnEnviarFeedback_Click;
        // 
        // lblResumen
        // 
        lblResumen.AutoSize = true;
        lblResumen.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblResumen.ForeColor = Color.FromArgb(41, 128, 185);
        lblResumen.Location = new Point(15, 25);
        lblResumen.Name = "lblResumen";
        lblResumen.Size = new Size(0, 19);
        lblResumen.TabIndex = 9;
        // 
        // lblComentarioLectura
        // 
        lblComentarioLectura.Font = new Font("Segoe UI", 9F, FontStyle.Italic);
        lblComentarioLectura.Location = new Point(15, 50);
        lblComentarioLectura.Name = "lblComentarioLectura";
        lblComentarioLectura.Size = new Size(430, 50);
        lblComentarioLectura.TabIndex = 10;
        // 
        // DetalleClienteForm
        // 
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        ClientSize = new Size(559, 482);
        Controls.Add(grpFeedback);
        Controls.Add(lblFechaCierre);
        Controls.Add(lblFechaCierreValor);
        Controls.Add(btnCerrar);
        Controls.Add(txtSolucion);
        Controls.Add(lblSolucion);
        Controls.Add(txtDescripcion);
        Controls.Add(lblDescripcion);
        Controls.Add(lblTecnicoValor);
        Controls.Add(lblTecnico);
        Controls.Add(lblEstatusValor);
        Controls.Add(lblEstatus);
        Controls.Add(lblFechaAltaValor);
        Controls.Add(lblFechaAlta);
        Controls.Add(lblFolioValor);
        Controls.Add(lblFolio);
        Font = new Font("Segoe UI", 11F);
        FormBorderStyle = FormBorderStyle.Sizable;
        MaximizeBox = true;
        MinimizeBox = true;
        MinimumSize = new Size(600, 520);
        Name = "DetalleClienteForm";
        StartPosition = FormStartPosition.CenterParent;
        Text = "Detalle del Ticket - Solo Lectura";
        Load += FrmDetalleCliente_Load;
        grpFeedback.ResumeLayout(false);
        grpFeedback.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    #endregion

    private System.Windows.Forms.Label lblFolio;
    private System.Windows.Forms.Label lblFolioValor;
    private System.Windows.Forms.Label lblFechaAlta;
    private System.Windows.Forms.Label lblFechaAltaValor;
    private System.Windows.Forms.Label lblEstatus;
    private System.Windows.Forms.Label lblEstatusValor;
    private System.Windows.Forms.Label lblTecnico;
    private System.Windows.Forms.Label lblTecnicoValor;
    private System.Windows.Forms.Label lblDescripcion;
    private HSis.UI.Controls.CajaTextoModerna txtDescripcion;
    private System.Windows.Forms.Label lblSolucion;
    private HSis.UI.Controls.CajaTextoModerna txtSolucion;
    private HSis.UI.Controls.BotonModerno btnCerrar;
    private System.Windows.Forms.Label lblFechaCierre;
    private System.Windows.Forms.Label lblFechaCierreValor;
    private HSis.UI.Controls.PanelCardModerno grpFeedback;
    private System.Windows.Forms.Label lblEstrellas;
    private System.Windows.Forms.Label lblStar1;
    private System.Windows.Forms.Label lblStar2;
    private System.Windows.Forms.Label lblStar3;
    private System.Windows.Forms.Label lblStar4;
    private System.Windows.Forms.Label lblStar5;
    private System.Windows.Forms.Label lblComentario;
    private HSis.UI.Controls.CajaTextoModerna txtComentario;
    private HSis.UI.Controls.BotonModerno btnEnviar;
    private System.Windows.Forms.Label lblResumen;
    private System.Windows.Forms.Label lblComentarioLectura;

    private void InicializarLayoutDetalleCliente()
    {
        this.BackColor = TemaVisual.FondoApp;
        this.Font = TemaVisual.FuenteNormal;
        this.ClientSize = new Size(720, 700);
        this.MinimumSize = new Size(620, 600);

        var pnlHeader = new Panel
        {
            BackColor = Color.White,
            Dock = DockStyle.Top,
            Height = 72,
            Name = "pnlHeaderDetalleCliente"
        };
        pnlHeader.Paint += (_, e) =>
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
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
                g.DrawString("Consulta la información de tu solicitud y comparte tu experiencia.", fuenteSubtitulo, brushSubtitulo, new PointF(70, 39));
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
            Name = "pnlFooterDetalleCliente"
        };
        pnlFooter.Paint += (_, e) =>
        {
            using var penDivision = new Pen(TemaVisual.BordeSutil, 1F);
            e.Graphics.DrawLine(penDivision, 0, 0, pnlFooter.Width, 0);
        };
        pnlFooter.Resize += (_, _) => pnlFooter.Invalidate();

        var flpCerrar = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Right,
            FlowDirection = FlowDirection.RightToLeft,
            Padding = new Padding(0, 12, 20, 12),
            WrapContents = false
        };
        btnCerrar.Margin = new Padding(0);
        btnCerrar.Size = new Size(120, 40);
        flpCerrar.Controls.Add(btnCerrar);
        pnlFooter.Controls.Add(flpCerrar);

        var pnlBody = new Panel
        {
            BackColor = TemaVisual.FondoApp,
            Dock = DockStyle.Fill,
            Name = "pnlBodyDetalleCliente",
            Padding = new Padding(24, 18, 24, 16)
        };

        var tblPrincipal = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 3
        };
        tblPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 150F));
        tblPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        tblPrincipal.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var tarjetaInfo = new PanelCardModerno
        {
            BackColor = TemaVisual.FondoTarjeta,
            Dock = DockStyle.Fill,
            Icono = IconChar.CircleInfo,
            Margin = new Padding(0, 0, 0, 12),
            Titulo = "Información del ticket"
        };

        var tblInfo = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 4,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 3
        };
        tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 92F));
        tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 112F));
        tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        for (int i = 0; i < 3; i++)
        {
            tblInfo.RowStyles.Add(new RowStyle(SizeType.Percent, 33.333F));
        }

        ConfigurarEtiquetaInfo(lblFolio);
        ConfigurarEtiquetaInfo(lblFechaAlta);
        ConfigurarEtiquetaInfo(lblFechaCierre);
        ConfigurarEtiquetaInfo(lblEstatus);
        ConfigurarEtiquetaInfo(lblTecnico);
        ConfigurarValorInfo(lblFolioValor);
        ConfigurarValorInfo(lblFechaAltaValor);
        ConfigurarValorInfo(lblFechaCierreValor);
        ConfigurarValorInfo(lblEstatusValor);
        ConfigurarValorInfo(lblTecnicoValor);

        tblInfo.Controls.Add(lblFolio, 0, 0);
        tblInfo.Controls.Add(lblFolioValor, 1, 0);
        tblInfo.Controls.Add(lblFechaAlta, 2, 0);
        tblInfo.Controls.Add(lblFechaAltaValor, 3, 0);
        tblInfo.Controls.Add(lblEstatus, 0, 1);
        tblInfo.Controls.Add(lblEstatusValor, 1, 1);
        tblInfo.Controls.Add(lblTecnico, 2, 1);
        tblInfo.Controls.Add(lblTecnicoValor, 3, 1);
        tblInfo.Controls.Add(lblFechaCierre, 0, 2);
        tblInfo.Controls.Add(lblFechaCierreValor, 1, 2);
        tarjetaInfo.Controls.Add(tblInfo);

        var tblContenido = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 1
        };
        tblContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tblContenido.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
        tblContenido.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var tarjetaDescripcion = CrearTarjetaContenido("Descripción del problema", IconChar.ClipboardList, lblDescripcion, txtDescripcion);
        tarjetaDescripcion.Margin = new Padding(0, 0, 6, 0);
        var tarjetaSolucion = CrearTarjetaContenido("Solución aplicada", IconChar.Check, lblSolucion, txtSolucion);
        tarjetaSolucion.Margin = new Padding(6, 0, 0, 0);
        tblContenido.Controls.Add(tarjetaDescripcion, 0, 0);
        tblContenido.Controls.Add(tarjetaSolucion, 1, 0);

        var tblFeedback = CrearLayoutFeedbackCliente();
        grpFeedback.AutoSize = true;
        grpFeedback.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        grpFeedback.MinimumSize = new Size(0, 156);
        grpFeedback.Dock = DockStyle.Fill;
        grpFeedback.Margin = new Padding(0, 12, 0, 0);
        grpFeedback.Controls.Clear();
        grpFeedback.Controls.Add(tblFeedback);

        tblPrincipal.Controls.Add(tarjetaInfo, 0, 0);
        tblPrincipal.Controls.Add(tblContenido, 0, 1);
        tblPrincipal.Controls.Add(grpFeedback, 0, 2);

        pnlBody.Controls.Add(tblPrincipal);
        Controls.Clear();
        Controls.Add(pnlBody);
        Controls.Add(pnlFooter);
        Controls.Add(pnlHeader);
    }

    private static PanelCardModerno CrearTarjetaContenido(string titulo, IconChar icono, Label etiqueta, Control contenido)
    {
        var tarjeta = new PanelCardModerno
        {
            BackColor = TemaVisual.FondoTarjeta,
            Dock = DockStyle.Fill,
            Icono = icono,
            Padding = new Padding(14, 44, 14, 14),
            Titulo = titulo
        };

        var tabla = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 2
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        etiqueta.AutoSize = false;
        etiqueta.Dock = DockStyle.Fill;
        etiqueta.Font = TemaVisual.FuentePequena;
        etiqueta.ForeColor = TemaVisual.TextoSecundario;
        etiqueta.Margin = new Padding(0, 0, 0, 4);
        etiqueta.TextAlign = ContentAlignment.MiddleLeft;

        contenido.Dock = DockStyle.Fill;
        contenido.Margin = new Padding(0);
        if (contenido is CajaTextoModerna caja)
        {
            caja.BackColor = TemaVisual.FondoTenue;
        }

        tabla.Controls.Add(etiqueta, 0, 0);
        tabla.Controls.Add(contenido, 0, 1);
        tarjeta.Controls.Add(tabla);
        return tarjeta;
    }

    private static void ConfigurarEtiquetaInfo(Label etiqueta)
    {
        etiqueta.AutoSize = false;
        etiqueta.Dock = DockStyle.Fill;
        etiqueta.Font = TemaVisual.FuentePequena;
        etiqueta.ForeColor = TemaVisual.TextoSecundario;
        etiqueta.Margin = new Padding(0, 0, 10, 0);
        etiqueta.TextAlign = ContentAlignment.MiddleLeft;
    }

    private static void ConfigurarValorInfo(Label valor)
    {
        valor.AutoSize = false;
        valor.Dock = DockStyle.Fill;
        valor.Font = TemaVisual.FuenteNormal;
        valor.ForeColor = TemaVisual.TextoPrincipal;
        valor.Margin = new Padding(0, 0, 16, 0);
        valor.TextAlign = ContentAlignment.MiddleLeft;
    }

    private TableLayoutPanel CrearLayoutFeedbackCliente()
    {
        var tabla = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 1,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            Padding = new Padding(0),
            RowCount = 3
        };
        tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        tabla.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
        tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var flpEstrellas = new FlowLayoutPanel
        {
            AutoSize = false,
            BackColor = Color.Transparent,
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            Margin = new Padding(0),
            WrapContents = false
        };
        lblEstrellas.AutoSize = true;
        lblEstrellas.Font = TemaVisual.FuenteNormal;
        lblEstrellas.ForeColor = TemaVisual.TextoMedio;
        lblEstrellas.Margin = new Padding(0, 7, 10, 0);
        flpEstrellas.Controls.Add(lblEstrellas);

        foreach (var estrella in new[] { lblStar1, lblStar2, lblStar3, lblStar4, lblStar5 })
        {
            estrella.Margin = new Padding(0, 1, 4, 0);
            flpEstrellas.Controls.Add(estrella);
        }

        lblResumen.AutoSize = true;
        lblResumen.Font = TemaVisual.FuenteNormal;
        lblResumen.ForeColor = TemaVisual.PrioridadMedia;
        lblResumen.Margin = new Padding(12, 8, 0, 0);
        flpEstrellas.Controls.Add(lblResumen);

        lblComentario.AutoSize = false;
        lblComentario.Dock = DockStyle.Fill;
        lblComentario.Font = TemaVisual.FuentePequena;
        lblComentario.ForeColor = TemaVisual.TextoSecundario;
        lblComentario.Margin = new Padding(0, 0, 0, 4);
        lblComentario.TextAlign = ContentAlignment.MiddleLeft;

        var tblComentarioInput = new TableLayoutPanel
        {
            BackColor = Color.Transparent,
            ColumnCount = 2,
            Dock = DockStyle.Fill,
            Margin = new Padding(0),
            RowCount = 1
        };
        tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        tblComentarioInput.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        var pnlComentario = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
        txtComentario.Dock = DockStyle.Fill;
        txtComentario.Margin = new Padding(0);
        lblComentarioLectura.Dock = DockStyle.Fill;
        lblComentarioLectura.Font = TemaVisual.FuenteNormal;
        lblComentarioLectura.ForeColor = TemaVisual.TextoMedio;
        lblComentarioLectura.Margin = new Padding(0);
        lblComentarioLectura.TextAlign = ContentAlignment.MiddleLeft;
        pnlComentario.Controls.Add(txtComentario);
        pnlComentario.Controls.Add(lblComentarioLectura);

        btnEnviar.Dock = DockStyle.Fill;
        btnEnviar.Margin = new Padding(12, 0, 0, 0);
        tblComentarioInput.Controls.Add(pnlComentario, 0, 0);
        tblComentarioInput.Controls.Add(btnEnviar, 1, 0);

        tabla.Controls.Add(flpEstrellas, 0, 0);
        tabla.Controls.Add(lblComentario, 0, 1);
        tabla.Controls.Add(tblComentarioInput, 0, 2);
        return tabla;
    }
}
