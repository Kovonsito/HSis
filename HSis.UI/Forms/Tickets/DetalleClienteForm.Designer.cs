using System;
using System.Drawing;
using System.Windows.Forms;

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
        txtDescripcion = new TextBox();
        lblSolucion = new Label();
        txtSolucion = new TextBox();
        btnCerrar = new Button();
        lblFechaCierre = new Label();
        lblFechaCierreValor = new Label();
        grpFeedback = new GroupBox();
        lblEstrellas = new Label();
        lblStar1 = new Label();
        lblStar2 = new Label();
        lblStar3 = new Label();
        lblStar4 = new Label();
        lblStar5 = new Label();
        lblComentario = new Label();
        txtComentario = new TextBox();
        btnEnviar = new Button();
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
        btnCerrar.BackColor = Color.FromArgb(231, 76, 60);
        btnCerrar.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
        btnCerrar.ForeColor = Color.White;
        btnCerrar.Location = new Point(413, 590);
        btnCerrar.Name = "btnCerrar";
        btnCerrar.Size = new Size(95, 33);
        btnCerrar.TabIndex = 12;
        btnCerrar.Text = "Cerrar";
        btnCerrar.UseVisualStyleBackColor = false;
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
        grpFeedback.Size = new Size(496, 120);
        grpFeedback.TabIndex = 0;
        grpFeedback.TabStop = false;
        grpFeedback.Text = "Retroalimentación de la Atención";
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
        txtComentario.Size = new Size(330, 23);
        txtComentario.TabIndex = 7;
        // 
        // btnEnviar
        // 
        btnEnviar.BackColor = Color.FromArgb(52, 152, 219);
        btnEnviar.FlatAppearance.BorderSize = 0;
        btnEnviar.FlatStyle = FlatStyle.Flat;
        btnEnviar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnEnviar.ForeColor = Color.White;
        btnEnviar.Location = new Point(365, 68);
        btnEnviar.Name = "btnEnviar";
        btnEnviar.Size = new Size(115, 30);
        btnEnviar.TabIndex = 8;
        btnEnviar.Text = "Enviar";
        btnEnviar.UseVisualStyleBackColor = false;
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
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
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
    private System.Windows.Forms.TextBox txtDescripcion;
    private System.Windows.Forms.Label lblSolucion;
    private System.Windows.Forms.TextBox txtSolucion;
    private System.Windows.Forms.Button btnCerrar;
    private System.Windows.Forms.Label lblFechaCierre;
    private System.Windows.Forms.Label lblFechaCierreValor;
    private System.Windows.Forms.GroupBox grpFeedback;
    private System.Windows.Forms.Label lblEstrellas;
    private System.Windows.Forms.Label lblStar1;
    private System.Windows.Forms.Label lblStar2;
    private System.Windows.Forms.Label lblStar3;
    private System.Windows.Forms.Label lblStar4;
    private System.Windows.Forms.Label lblStar5;
    private System.Windows.Forms.Label lblComentario;
    private System.Windows.Forms.TextBox txtComentario;
    private System.Windows.Forms.Button btnEnviar;
    private System.Windows.Forms.Label lblResumen;
    private System.Windows.Forms.Label lblComentarioLectura;

    private void InicializarLayoutDetalleCliente()
    {
        // 1. Crear el TableLayoutPanel principal
        var tblPrincipal = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            RowCount = 7,
            ColumnCount = 1,
            Padding = new Padding(12),
            Name = "tblPrincipal"
        };

        // 2. Grid de Información
        var tblInfo = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            RowCount = 5,
            ColumnCount = 2,
            Margin = new Padding(0, 0, 0, 10)
        };
        tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150F));
        tblInfo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        for (int i = 0; i < 5; i++)
        {
            tblInfo.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        }

        lblFolio.Dock = DockStyle.Fill;
        lblFolioValor.Dock = DockStyle.Fill;
        lblFechaAlta.Dock = DockStyle.Fill;
        lblFechaAltaValor.Dock = DockStyle.Fill;
        lblFechaCierre.Dock = DockStyle.Fill;
        lblFechaCierreValor.Dock = DockStyle.Fill;
        lblEstatus.Dock = DockStyle.Fill;
        lblEstatusValor.Dock = DockStyle.Fill;
        lblTecnico.Dock = DockStyle.Fill;
        lblTecnicoValor.Dock = DockStyle.Fill;

        tblInfo.Controls.Add(lblFolio, 0, 0);
        tblInfo.Controls.Add(lblFolioValor, 1, 0);
        tblInfo.Controls.Add(lblFechaAlta, 0, 1);
        tblInfo.Controls.Add(lblFechaAltaValor, 1, 1);
        tblInfo.Controls.Add(lblFechaCierre, 0, 2);
        tblInfo.Controls.Add(lblFechaCierreValor, 1, 2);
        tblInfo.Controls.Add(lblEstatus, 0, 3);
        tblInfo.Controls.Add(lblEstatusValor, 1, 3);
        tblInfo.Controls.Add(lblTecnico, 0, 4);
        tblInfo.Controls.Add(lblTecnicoValor, 1, 4);

        // 3. Descripciones y soluciones
        lblDescripcion.Dock = DockStyle.Fill;
        lblDescripcion.Margin = new Padding(0, 0, 0, 5);
        txtDescripcion.Dock = DockStyle.Fill;
        txtDescripcion.Height = 80;
        txtDescripcion.Margin = new Padding(0, 0, 0, 10);

        lblSolucion.Dock = DockStyle.Fill;
        lblSolucion.Margin = new Padding(0, 0, 0, 5);
        txtSolucion.Dock = DockStyle.Fill;
        txtSolucion.Height = 80;
        txtSolucion.Margin = new Padding(0, 0, 0, 10);

        // 4. Seccion de feedback
        grpFeedback.Dock = DockStyle.Fill;
        grpFeedback.Margin = new Padding(0, 0, 0, 10);
        grpFeedback.AutoSize = true;

        var tblFeedback = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            RowCount = 3,
            ColumnCount = 1,
            Padding = new Padding(12)
        };
        tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        tblFeedback.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var flpEstrellas = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.LeftToRight,
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 10)
        };
        lblEstrellas.Margin = new Padding(0, 5, 10, 0);
        lblEstrellas.AutoSize = true;

        flpEstrellas.Controls.Add(lblEstrellas);
        flpEstrellas.Controls.Add(lblStar1);
        flpEstrellas.Controls.Add(lblStar2);
        flpEstrellas.Controls.Add(lblStar3);
        flpEstrellas.Controls.Add(lblStar4);
        flpEstrellas.Controls.Add(lblStar5);
        flpEstrellas.Controls.Add(lblResumen);

        lblComentario.Margin = new Padding(0, 0, 0, 5);
        lblComentario.AutoSize = true;

        var tblComentarioInput = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            RowCount = 1,
            ColumnCount = 2,
            Margin = new Padding(0)
        };
        tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        tblComentarioInput.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 120F));

        var pnlComentario = new Panel { Dock = DockStyle.Fill, Margin = new Padding(0) };
        txtComentario.Dock = DockStyle.Fill;
        lblComentarioLectura.Dock = DockStyle.Fill;
        pnlComentario.Controls.Add(txtComentario);
        pnlComentario.Controls.Add(lblComentarioLectura);

        btnEnviar.Dock = DockStyle.Fill;
        btnEnviar.Margin = new Padding(10, 0, 0, 0);

        tblComentarioInput.Controls.Add(pnlComentario, 0, 0);
        tblComentarioInput.Controls.Add(btnEnviar, 1, 0);

        grpFeedback.Controls.Clear();
        tblFeedback.Controls.Add(flpEstrellas, 0, 0);
        tblFeedback.Controls.Add(lblComentario, 0, 1);
        tblFeedback.Controls.Add(tblComentarioInput, 0, 2);
        grpFeedback.Controls.Add(tblFeedback);


        // 5. Botón cerrar
        var flpCerrar = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            AutoSize = true,
            Margin = new Padding(0)
        };
        btnCerrar.Margin = new Padding(0);
        btnCerrar.Dock = DockStyle.None;
        flpCerrar.Controls.Add(btnCerrar);

        // 6. Montar todo en tblPrincipal
        tblPrincipal.Controls.Add(tblInfo, 0, 0);
        tblPrincipal.Controls.Add(lblDescripcion, 0, 1);
        tblPrincipal.Controls.Add(txtDescripcion, 0, 2);
        tblPrincipal.Controls.Add(lblSolucion, 0, 3);
        tblPrincipal.Controls.Add(txtSolucion, 0, 4);
        tblPrincipal.Controls.Add(grpFeedback, 0, 5);
        tblPrincipal.Controls.Add(flpCerrar, 0, 6);

        // Remover de la ventana original
        this.Controls.Clear();
        this.Controls.Add(tblPrincipal);
    }
}
