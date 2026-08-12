#nullable enable
using System;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using HSis.Logic.Services;

namespace HSis.UI.Presenters
{
    public class NotificacionesPresenter
    {
        private readonly INotificationClientService _clienteNotificaciones;
        private readonly INotificacionStorageService _servicioAlmacenamiento;
        private readonly IContextoSesion _contextoSesion;
        private readonly INotificationEventBus? _eventBus;

        private INotificacionesView? _view;

        public NotificacionesPresenter(
            INotificationClientService clienteNotificaciones,
            INotificacionStorageService servicioAlmacenamiento,
            IContextoSesion contextoSesion,
            INotificationEventBus? eventBus = null)
        {
            _clienteNotificaciones = clienteNotificaciones;
            _servicioAlmacenamiento = servicioAlmacenamiento;
            _contextoSesion = contextoSesion;
            _eventBus = eventBus;
        }

        public void SetView(INotificacionesView view)
        {
            _view = view;
            SuscribirEventos();
        }

        public void DesconectarEvents()
        {
            DesuscribirEventos();
        }

        private void SuscribirEventos()
        {
            if (_eventBus != null)
            {
                _eventBus.OnNotificacionPublicada += EnBusNotificacionPublicada;
                _eventBus.OnEstadoConexionCambiado += EnBusEstadoConexionCambiado;
            }
            else
            {
                _clienteNotificaciones.OnNotificationReceived += EnNotificacionRecibida;
                _clienteNotificaciones.OnReconnecting += EnReconectando;
                _clienteNotificaciones.OnConnected += EnConectado;
                _clienteNotificaciones.OnDisconnected += EnDesconectado;
            }
        }

        private void DesuscribirEventos()
        {
            if (_eventBus != null)
            {
                _eventBus.OnNotificacionPublicada -= EnBusNotificacionPublicada;
                _eventBus.OnEstadoConexionCambiado -= EnBusEstadoConexionCambiado;
            }
            else
            {
                _clienteNotificaciones.OnNotificationReceived -= EnNotificacionRecibida;
                _clienteNotificaciones.OnReconnecting -= EnReconectando;
                _clienteNotificaciones.OnConnected -= EnConectado;
                _clienteNotificaciones.OnDisconnected -= EnDesconectado;
            }
        }

        public async Task CargarHistorialAsync()
        {
            if (_view == null) return;
            try
            {
                await _servicioAlmacenamiento.SincronizarDesdeBDAsync(_contextoSesion.IdUsuario);
            }
            catch
            {
                // Ignorar errores durante la sincronización inicial
            }

            var list = (await _servicioAlmacenamiento.ObtenerNotificacionesAsync(_contextoSesion.IdUsuario)).ToList();
            int noLeidas = list.Count(n => !n.Leido);
            _view.ActualizarInsigniaCampana(noLeidas);
            _view.MostrarNotificaciones(list);
        }

        public async Task MarcarComoLeidaAsync(NotificacionLocal notif)
        {
            if (!notif.Leido)
            {
                await _servicioAlmacenamiento.MarcarComoLeidaAsync(_contextoSesion.IdUsuario, notif.Id);
                await CargarHistorialAsync();
            }
            _view?.AbrirDetalleTicket(notif.TicketId);
        }

        public async Task MarcarTodasComoLeidasAsync()
        {
            await _servicioAlmacenamiento.MarcarTodasComoLeidasAsync(_contextoSesion.IdUsuario);
            await CargarHistorialAsync();
        }

        public async Task LimpiarTodasAsync()
        {
            await _servicioAlmacenamiento.LimpiarTodasAsync(_contextoSesion.IdUsuario);
            await CargarHistorialAsync();
        }

        private async void EnNotificacionRecibida(string tipo, int ticketId, string mensaje)
        {
            await _servicioAlmacenamiento.GuardarNotificacionAsync(_contextoSesion.IdUsuario, ticketId, mensaje);
            if (_view != null)
            {
                await _view.RecargarDatosHostAsync();
            }
            await CargarHistorialAsync();
        }

        private void EnReconectando()
        {
            _view?.ActualizarEstadoConexion(false, "⚠️ Intentando reconectar con el servidor de notificaciones...", Color.FromArgb(230, 126, 34));
        }

        private void EnConectado()
        {
            _view?.ActualizarEstadoConexion(true, string.Empty, Color.Empty);
            _ = _view?.RecargarDatosHostAsync();
        }

        private void EnDesconectado()
        {
            _view?.ActualizarEstadoConexion(false, "⚠️ Sin conexión con el servidor de notificaciones. Intentando reconectar...", Color.FromArgb(231, 76, 60));
        }

        private void EnBusNotificacionPublicada(object? sender, NotificacionEventArgs e)
        {
            EnNotificacionRecibida(e.Tipo, e.TicketId, e.Mensaje);
        }

        private void EnBusEstadoConexionCambiado(object? sender, EstadoConexionEventArgs e)
        {
            Color colorFondo = e.Conectado ? Color.Empty : Color.FromArgb(231, 76, 60);
            _view?.ActualizarEstadoConexion(e.Conectado, e.MensajeEstado ?? string.Empty, colorFondo);
            if (e.Conectado && _view != null)
            {
                _ = _view.RecargarDatosHostAsync();
            }
        }
    }
}
