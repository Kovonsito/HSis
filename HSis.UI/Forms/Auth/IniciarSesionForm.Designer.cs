#nullable enable
using System;
using System.Drawing;
using System.Windows.Forms;
using FontAwesome.Sharp;
using HSis.UI.Controls;

namespace HSis.UI.Forms.Auth;

partial class IniciarSesionForm
{
    /// <summary>
    /// Required designer variable.
    private System.ComponentModel.IContainer? components = null;

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
        this.pnlBranding = new Panel();
        this.pnlFormulario = new Panel();
        this.lblBienvenida = new Label();
        this.lblSubBienvenida = new Label();
        this.lblUsuario = new Label();
        this.txtUsuario = new TextBox();
        this.lblContraseña = new Label();
        this.txtContraseña = new TextBox();
        this.btnIniciarSesion = new HSis.UI.Controls.BotonModerno();
        this.pnlFormulario.SuspendLayout();
        this.SuspendLayout();

        // 
        // pnlBranding
        // 
        this.pnlBranding.BackColor = Color.FromArgb(15, 23, 42);
        this.pnlBranding.Dock = DockStyle.Left;
        this.pnlBranding.Location = new Point(0, 0);
        this.pnlBranding.Name = "pnlBranding";
        this.pnlBranding.Size = new Size(250, 420);
        this.pnlBranding.TabIndex = 0;
        this.pnlBranding.Paint += PnlBranding_Paint;

        // 
        // pnlFormulario
        // 
        this.pnlFormulario.BackColor = Color.White;
        this.pnlFormulario.Controls.Add(this.lblBienvenida);
        this.pnlFormulario.Controls.Add(this.lblSubBienvenida);
        this.pnlFormulario.Controls.Add(this.lblUsuario);
        this.pnlFormulario.Controls.Add(this.txtUsuario);
        this.pnlFormulario.Controls.Add(this.lblContraseña);
        this.pnlFormulario.Controls.Add(this.txtContraseña);
        this.pnlFormulario.Controls.Add(this.btnIniciarSesion);
        this.pnlFormulario.Dock = DockStyle.Fill;
        this.pnlFormulario.Location = new Point(250, 0);
        this.pnlFormulario.Name = "pnlFormulario";
        this.pnlFormulario.Padding = new Padding(35, 30, 35, 30);
        this.pnlFormulario.Size = new Size(400, 420);
        this.pnlFormulario.TabIndex = 1;

        // 
        // lblBienvenida
        // 
        this.lblBienvenida.AutoSize = true;
        this.lblBienvenida.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
        this.lblBienvenida.ForeColor = Color.FromArgb(15, 23, 42);
        this.lblBienvenida.Location = new Point(35, 35);
        this.lblBienvenida.Name = "lblBienvenida";
        this.lblBienvenida.Size = new Size(185, 30);
        this.lblBienvenida.TabIndex = 0;
        this.lblBienvenida.Text = "¡Hola de nuevo! 👋";

        // 
        // lblSubBienvenida
        // 
        this.lblSubBienvenida.AutoSize = true;
        this.lblSubBienvenida.Font = new Font("Segoe UI", 9F);
        this.lblSubBienvenida.ForeColor = Color.FromArgb(100, 116, 139);
        this.lblSubBienvenida.Location = new Point(37, 68);
        this.lblSubBienvenida.Name = "lblSubBienvenida";
        this.lblSubBienvenida.Size = new Size(245, 15);
        this.lblSubBienvenida.TabIndex = 1;
        this.lblSubBienvenida.Text = "Ingresa tus datos de acceso al sistema HSis.";

        // 
        // lblUsuario
        // 
        this.lblUsuario.AutoSize = true;
        this.lblUsuario.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        this.lblUsuario.ForeColor = Color.FromArgb(51, 65, 85);
        this.lblUsuario.Location = new Point(37, 110);
        this.lblUsuario.Name = "lblUsuario";
        this.lblUsuario.Size = new Size(122, 17);
        this.lblUsuario.TabIndex = 2;
        this.lblUsuario.Text = "Usuario o Matrícula";

        // 
        // txtUsuario
        // 
        this.txtUsuario.Font = new Font("Segoe UI", 10.5F);
        this.txtUsuario.Location = new Point(37, 132);
        this.txtUsuario.Name = "txtUsuario";
        this.txtUsuario.Size = new Size(320, 26);
        this.txtUsuario.TabIndex = 3;

        // 
        // lblContraseña
        // 
        this.lblContraseña.AutoSize = true;
        this.lblContraseña.Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold);
        this.lblContraseña.ForeColor = Color.FromArgb(51, 65, 85);
        this.lblContraseña.Location = new Point(37, 180);
        this.lblContraseña.Name = "lblContraseña";
        this.lblContraseña.Size = new Size(77, 17);
        this.lblContraseña.TabIndex = 4;
        this.lblContraseña.Text = "Contraseña";

        // 
        // txtContraseña
        // 
        this.txtContraseña.Font = new Font("Segoe UI", 10.5F);
        this.txtContraseña.Location = new Point(37, 202);
        this.txtContraseña.Name = "txtContraseña";
        this.txtContraseña.PasswordChar = '●';
        this.txtContraseña.Size = new Size(320, 26);
        this.txtContraseña.TabIndex = 5;

        // 
        // btnIniciarSesion
        // 
        this.btnIniciarSesion.Estilo = EstiloBotonModerno.Primario;
        this.btnIniciarSesion.Icono = FontAwesome.Sharp.IconChar.RightToBracket;
        this.btnIniciarSesion.IconoTamano = 16;
        this.btnIniciarSesion.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        this.btnIniciarSesion.Location = new Point(37, 265);
        this.btnIniciarSesion.Name = "btnIniciarSesion";
        this.btnIniciarSesion.Size = new Size(320, 42);
        this.btnIniciarSesion.TabIndex = 6;
        this.btnIniciarSesion.Text = "Iniciar Sesión";
        this.btnIniciarSesion.Click += BtnIniciarSesion_Click;

        // 
        // IniciarSesionForm
        // 
        this.AcceptButton = this.btnIniciarSesion;
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.BackColor = Color.White;
        this.ClientSize = new Size(650, 400);
        this.Controls.Add(this.pnlFormulario);
        this.Controls.Add(this.pnlBranding);
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.MinimizeBox = false;
        this.Name = "IniciarSesionForm";
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "HSis Support - Iniciar Sesión";
        this.Load += FrmIniciarSesion_Load;
        this.pnlFormulario.ResumeLayout(false);
        this.pnlFormulario.PerformLayout();
        this.ResumeLayout(false);
    }

    #endregion

    private Panel pnlBranding = null!;
    private Panel pnlFormulario = null!;
    private Label lblBienvenida = null!;
    private Label lblSubBienvenida = null!;
    private Label lblUsuario = null!;
    private Label lblContraseña = null!;
    private TextBox txtUsuario = null!;
    private TextBox txtContraseña = null!;
    private HSis.UI.Controls.BotonModerno btnIniciarSesion = null!;

    private void PnlBranding_Paint(object? sender, PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

        // Logo circle
        using (var brushLogo = new SolidBrush(Color.FromArgb(37, 99, 235)))
        {
            g.FillEllipse(brushLogo, 30, 45, 48, 48);
        }

        using (var bmpLogo = FontAwesome.Sharp.IconChar.ShieldHalved.ToBitmap(Color.White, 24))
        {
            g.DrawImage(bmpLogo, 42, 57);
        }

        using (var brushTitle = new SolidBrush(Color.White))
        using (var fontTitle = new Font("Segoe UI", 16f, FontStyle.Bold))
        {
            g.DrawString("HSis", fontTitle, brushTitle, new PointF(88, 43));
        }

        using (var brushSub = new SolidBrush(Color.FromArgb(148, 163, 184)))
        using (var fontSub = new Font("Segoe UI", 9.5f, FontStyle.Regular))
        {
            g.DrawString("Mesa de Servicio", fontSub, brushSub, new PointF(88, 70));
        }

        // Bullets con iconos vectoriales FontAwesome
        var features = new (FontAwesome.Sharp.IconChar Icon, string Text)[]
        {
            (FontAwesome.Sharp.IconChar.Bolt, "Tickets en tiempo real"),
            (FontAwesome.Sharp.IconChar.BoxesStacked, "Control de inventario"),
            (FontAwesome.Sharp.IconChar.ChartLine, "Métricas y exportación"),
            (FontAwesome.Sharp.IconChar.UserShield, "Seguridad y roles")
        };

        int startY = 160;
        using var brushBullet = new SolidBrush(Color.FromArgb(203, 213, 225));
        using var fontBullet = new Font("Segoe UI", 9.5f, FontStyle.Regular);

        for (int i = 0; i < features.Length; i++)
        {
            int rowY = startY + (i * 36);
            using var bmpItem = features[i].Icon.ToBitmap(Color.FromArgb(59, 130, 246), 15);
            g.DrawImage(bmpItem, 30, rowY + 2);
            g.DrawString(features[i].Text, fontBullet, brushBullet, new PointF(54, rowY));
        }

        using var brushFoot = new SolidBrush(Color.FromArgb(100, 116, 139));
        using var fontFoot = new Font("Segoe UI", 8f);
        g.DrawString("HSis v2.0 • 2026", fontFoot, brushFoot, new PointF(30, 350));
    }

    private void InicializarLayoutLogin()
    {
    }
}