using System.Collections.Concurrent;
using System.Text.Json;

namespace HSis.Logic.Services
{
    public class NotificacionLocal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int TicketId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Leido { get; set; }
    }

    public class NotificacionStorageService
    {
        private string GetFilePath(int userId)
        {
            var appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var folder = Path.Combine(appData, "HSis");
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            return Path.Combine(folder, $"notificaciones_user_{userId}.json");
        }

        public async Task<List<NotificacionLocal>> ObtenerNotificacionesAsync(int userId)
        {
            var path = GetFilePath(userId);
            if (!File.Exists(path))
            {
                return new List<NotificacionLocal>();
            }

            try
            {
                var json = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<List<NotificacionLocal>>(json) ?? new List<NotificacionLocal>();
            }
            catch
            {
                return new List<NotificacionLocal>();
            }
        }

        public async Task GuardarNotificacionAsync(int userId, int ticketId, string mensaje)
        {
            var list = await ObtenerNotificacionesAsync(userId);
            list.Insert(0, new NotificacionLocal
            {
                TicketId = ticketId,
                Mensaje = mensaje,
                Fecha = DateTime.Now,
                Leido = false
            });

            // Limitar a las últimas 50 notificaciones para no saturar
            if (list.Count > 50)
            {
                list = list.Take(50).ToList();
            }

            await GuardarListaAsync(userId, list);
        }

        public async Task MarcarComoLeidaAsync(int userId, Guid id)
        {
            var list = await ObtenerNotificacionesAsync(userId);
            var notif = list.FirstOrDefault(n => n.Id == id);
            if (notif != null)
            {
                notif.Leido = true;
                await GuardarListaAsync(userId, list);
            }
        }

        public async Task LimpiarTodasAsync(int userId)
        {
            await GuardarListaAsync(userId, new List<NotificacionLocal>());
        }

        private async Task GuardarListaAsync(int userId, List<NotificacionLocal> list)
        {
            var path = GetFilePath(userId);
            try
            {
                var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(path, json);
            }
            catch
            {
                // Ignorar errores de escritura
            }
        }
    }
}
