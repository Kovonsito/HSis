#nullable enable
using System.Runtime.Versioning;
using HSis.Logic.Services;
using HSis.UI.Factories;
using HSis.UI.Presenters;

namespace HSis.UI.Controls
{
    [SupportedOSPlatform("windows")]
    public partial class NotificacionesControl : UserControl, INotificacionesView
    {
        private NotificacionesPresenter? _presenter;
        private IFabricaFormularios? _fabricaFormularios;
        private IContextoSesion? _contextoSesion;
        private Func<Task>? _callbackRecargaDatos;

        public NotificacionesControl()
        {
            InitializeComponent();
        }

        public void Configurar(
            NotificacionesPresenter presenter,
            IFabricaFormularios fabricaFormularios,
            IContextoSesion contextoSesion,
            Func<Task>? callbackRecargaDatos = null)
        {
            _presenter = presenter;
            _fabricaFormularios = fabricaFormularios;
            _contextoSesion = contextoSesion;
            _callbackRecargaDatos = callbackRecargaDatos;

            _presenter.SetView(this);
            _ = _presenter.CargarHistorialAsync();
        }

        public void ActualizarInsigniaCampana(int noLeidas)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => ActualizarInsigniaCampana(noLeidas)));
                return;
            }
            btnCampana.Text = noLeidas > 0 ? $"🔔 ({noLeidas}) 🔴" : $"🔔 ({noLeidas})";
            btnCampana.ForeColor = noLeidas > 0 ? Color.Red : Color.Black;
            btnCampana.Font = new Font("Segoe UI", 10F, noLeidas > 0 ? FontStyle.Bold : FontStyle.Regular);
        }

        public void MostrarNotificaciones(IEnumerable<NotificacionLocal> notificaciones)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new Action(() => MostrarNotificaciones(notificaciones)));
                return;
            }

            flpNotificaciones.Controls.Clear();
            foreach (var notif in notificaciones)
            {
                var pnlItem = CrearItemNotificacion(notif);
                flpNotificaciones.Controls.Add(pnlItem);
            }
        }

        public void ActualizarEstadoConexion(bool conectado, string mensaje, Color colorFondo)
        {
            // Opcionalmente propagar al banner si está incrustado o delegate
        }

        public async Task RecargarDatosHostAsync()
        {
            if (_callbackRecargaDatos != null)
            {
                await _callbackRecargaDatos();
            }
        }

        public void AbrirDetalleTicket(int ticketId)
        {
            if (_fabricaFormularios == null || _contextoSesion == null) return;

            Form detailForm = _contextoSesion.IdRolUsuario == 3
                ? _fabricaFormularios.CrearDetalleCliente(ticketId)
                : _fabricaFormularios.CrearTicketDetalle(ticketId);

            using (detailForm)
            {
                detailForm.ShowDialog();
            }
        }

        private Panel CrearItemNotificacion(NotificacionLocal notif)
        {
            var pnlItem = new Panel
            {
                Width = flpNotificaciones.ClientSize.Width - 10,
                Height = 85,
                BackColor = notif.Leido ? Color.White : Color.FromArgb(235, 245, 251),
                Padding = new Padding(8),
                Margin = new Padding(0, 0, 0, 8),
                Cursor = Cursors.Hand
            };

            pnlItem.Paint += (s, e) =>
            {
                ControlPaint.DrawBorder(e.Graphics, pnlItem.ClientRectangle, Color.FromArgb(220, 224, 230), ButtonBorderStyle.Solid);
                if (!notif.Leido)
                {
                    using var brush = new SolidBrush(Color.FromArgb(52, 152, 219));
                    e.Graphics.FillEllipse(brush, pnlItem.Width - 15, 8, 8, 8);
                }
            };

            var lblMsg = new Label
            {
                Text = notif.Mensaje,
                Font = new Font("Segoe UI", 9.5F, notif.Leido ? FontStyle.Regular : FontStyle.Bold),
                ForeColor = Color.FromArgb(44, 62, 80),
                Location = new Point(8, 8),
                Size = new Size(pnlItem.Width - 25, 50),
                AutoEllipsis = true
            };

            var lblFecha = new Label
            {
                Text = notif.Fecha.ToString("g"),
                Font = new Font("Segoe UI", 8F, FontStyle.Italic),
                ForeColor = Color.Gray,
                Location = new Point(8, 60),
                Size = new Size(pnlItem.Width - 20, 18)
            };

            lblMsg.Click += async (s, e) => await _presenter?.MarcarComoLeidaAsync(notif)!;
            lblFecha.Click += async (s, e) => await _presenter?.MarcarComoLeidaAsync(notif)!;
            pnlItem.Click += async (s, e) => await _presenter?.MarcarComoLeidaAsync(notif)!;

            pnlItem.Controls.Add(lblMsg);
            pnlItem.Controls.Add(lblFecha);
            return pnlItem;
        }

        private void BtnCampana_Click(object sender, EventArgs e)
        {
            pnlHistorial.Visible = !pnlHistorial.Visible;
            if (pnlHistorial.Visible)
            {
                pnlHistorial.BringToFront();
            }
        }

        private async void BtnMarcarTodasLeidas_Click(object sender, EventArgs e)
        {
            if (_presenter != null) await _presenter.MarcarTodasComoLeidasAsync();
        }

        private async void BtnLimpiar_Click(object sender, EventArgs e)
        {
            if (_presenter != null) await _presenter.LimpiarTodasAsync();
        }
    }
}
