#nullable enable
using System.Drawing.Drawing2D;
using System.Media;
using System.Runtime.Versioning;
using FontAwesome.Sharp;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.UI.Factories;
using HSis.UI.Helpers;
using NotificacionLocal = HSis.UI.Models.Notificaciones.NotificacionLocal;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public sealed class PopUpNotificacionResumen : Form
    {
        private readonly System.Windows.Forms.Timer _timerCierre = new();
        private readonly Action _alHacerClic;

        public PopUpNotificacionResumen(NotificacionDto notificacion, Action alHacerClic)
        {
            _alHacerClic = alHacerClic;

            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            TopMost = true;
            Width = 320;
            Height = 72;
            DoubleBuffered = true;
            BackColor = Color.White;

            var pnlContenedor = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.White,
                Cursor = Cursors.Hand
            };

            var picIcon = new PictureBox
            {
                Size = new Size(34, 34),
                Location = new Point(12, 19),
                Image = IconChar.Bell.ToBitmap(TemaVisual.Primario, 20),
                SizeMode = PictureBoxSizeMode.CenterImage,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblTitulo = new Label
            {
                Text = string.IsNullOrWhiteSpace(notificacion.Tipo) ? "Nueva Notificación" : notificacion.Tipo,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TemaVisual.TextoPrincipal,
                Location = new Point(54, 12),
                Size = new Size(230, 18),
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var lblMensaje = new Label
            {
                Text = notificacion.Mensaje,
                Font = new Font("Segoe UI", 8.25F, FontStyle.Regular),
                ForeColor = TemaVisual.TextoMedio,
                Location = new Point(54, 32),
                Size = new Size(230, 30),
                AutoEllipsis = true,
                BackColor = Color.Transparent,
                Cursor = Cursors.Hand
            };

            var btnCerrar = new Label
            {
                Text = "✕",
                Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                ForeColor = TemaVisual.TextoMuted,
                Location = new Point(290, 8),
                Size = new Size(22, 22),
                TextAlign = ContentAlignment.MiddleCenter,
                Cursor = Cursors.Hand,
                BackColor = Color.Transparent
            };

            btnCerrar.MouseEnter += (s, e) => btnCerrar.ForeColor = TemaVisual.TextoOscuro;
            btnCerrar.MouseLeave += (s, e) => btnCerrar.ForeColor = TemaVisual.TextoMuted;
            btnCerrar.Click += (s, e) => CerrarPopUp();

            void EjecutarAccion(object? s, EventArgs e)
            {
                _timerCierre.Stop();
                Close();
                _alHacerClic();
            }

            pnlContenedor.Click += EjecutarAccion;
            picIcon.Click += EjecutarAccion;
            lblTitulo.Click += EjecutarAccion;
            lblMensaje.Click += EjecutarAccion;

            pnlContenedor.Controls.Add(picIcon);
            pnlContenedor.Controls.Add(lblTitulo);
            pnlContenedor.Controls.Add(lblMensaje);
            pnlContenedor.Controls.Add(btnCerrar);
            Controls.Add(pnlContenedor);

            pnlContenedor.Paint += (s, e) =>
            {
                var g = e.Graphics;
                g.SmoothingMode = SmoothingMode.AntiAlias;

                var rect = new Rectangle(0, 0, Width - 1, Height - 1);
                using var path = TemaVisual.CrearRectanguloRedondeado(rect, 8);
                using var penBorder = new Pen(Color.FromArgb(191, 219, 254), 1.5f);
                g.DrawPath(penBorder, path);

                // Barra de color lateral
                using var brushBarra = new SolidBrush(TemaVisual.Primario);
                g.FillRectangle(brushBarra, 0, 8, 4, Height - 16);
            };

            _timerCierre.Interval = 4500;
            _timerCierre.Tick += (s, e) => CerrarPopUp();
        }

        protected override bool ShowWithoutActivation => true;

        protected override CreateParams CreateParams
        {
            get
            {
                var cp = base.CreateParams;
                cp.ExStyle |= 0x08000000; // WS_EX_NOACTIVATE
                return cp;
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);
            _timerCierre.Start();
        }

        private void CerrarPopUp()
        {
            _timerCierre.Stop();
            Close();
            Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _timerCierre.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

