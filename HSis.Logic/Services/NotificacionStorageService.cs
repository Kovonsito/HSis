using System.Text.Json;
using System.Text.RegularExpressions;
using HSis.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace HSis.Logic.Services
{
    public class NotificacionLocal
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public int? DbId { get; set; }
        public int TicketId { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public DateTime Fecha { get; set; } = DateTime.Now;
        public bool Leido { get; set; }
    }

    public class NotificacionStorageService(IDbContextFactory<HSisDbContext> dbContextFactory) : INotificacionStorageService
    {
        private readonly IDbContextFactory<HSisDbContext> _dbContextFactory = dbContextFactory;
        private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

        private static string GetFilePath(int userId)
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
                return [];
            }

            try
            {
                var json = await File.ReadAllTextAsync(path);
                return JsonSerializer.Deserialize<List<NotificacionLocal>>(json) ?? [];
            }
            catch
            {
                return [];
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
                list = [.. list.Take(50)];
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

                if (notif.DbId.HasValue)
                {
                    try
                    {
                        using var db = _dbContextFactory.CreateDbContext();
                        var dbNotif = await db.Notificaciones.FindAsync(notif.DbId.Value);
                        if (dbNotif != null)
                        {
                            dbNotif.Leido = true;
                            await db.SaveChangesAsync();
                        }
                    }
                    catch
                    {
                        // Ignorar errores de base de datos para asegurar el funcionamiento offline
                    }
                }
            }
        }

        public async Task LimpiarTodasAsync(int userId)
        {
            await GuardarListaAsync(userId, []);

            try
            {
                using var db = _dbContextFactory.CreateDbContext();
                var dbNotifs = await db.Notificaciones.Where(n => n.UsuarioDestinoId == userId).ToListAsync();
                db.Notificaciones.RemoveRange(dbNotifs);
                await db.SaveChangesAsync();
            }
            catch
            {
                // Ignorar errores de base de datos
            }
        }

        public async Task SincronizarDesdeBDAsync(int userId)
        {
            try
            {
                using var db = _dbContextFactory.CreateDbContext();
                var dbNotifications = await db.Notificaciones
                    .Where(n => n.UsuarioDestinoId == userId)
                    .OrderByDescending(n => n.FechaCreacion)
                    .ToListAsync();

                if (dbNotifications.Count == 0) return;

                var localList = await ObtenerNotificacionesAsync(userId);
                bool huboCambios = false;

                foreach (var dbNotif in dbNotifications)
                {
                    // Comprobar si ya existe en la lista local por DbId
                    var localNotif = localList.FirstOrDefault(n => n.DbId == dbNotif.IdNotificacion);

                    if (localNotif == null)
                    {
                        // Extraer el id del ticket del mensaje (ej: TK-000005)
                        int ticketId = 0;
                        var match = Regex.Match(dbNotif.Mensaje, @"TK-(\d+)");
                        if (match.Success && int.TryParse(match.Groups[1].Value, out int parsedId))
                        {
                            ticketId = parsedId;
                        }

                        // Agregar nueva notificación local al principio
                        localList.Insert(0, new NotificacionLocal
                        {
                            DbId = dbNotif.IdNotificacion,
                            TicketId = ticketId,
                            Mensaje = dbNotif.Mensaje,
                            Fecha = dbNotif.FechaCreacion,
                            Leido = dbNotif.Leido
                        });
                        huboCambios = true;
                    }
                    else if (localNotif.Leido != dbNotif.Leido)
                    {
                        // Sincronizar el estado de leído si difiere
                        localNotif.Leido = dbNotif.Leido;
                        huboCambios = true;
                    }
                }

                if (huboCambios)
                {
                    // Ordenar por fecha descendente
                    localList = [.. localList.OrderByDescending(n => n.Fecha)];

                    // Limitar a 50
                    if (localList.Count > 50)
                    {
                        localList = [.. localList.Take(50)];
                    }

                    await GuardarListaAsync(userId, localList);
                }
            }
            catch
            {
                // Prevenir caídas si no hay conexión a la base de datos
            }
        }

        private async Task GuardarListaAsync(int userId, List<NotificacionLocal> list)
        {
            var path = GetFilePath(userId);
            try
            {
                var json = JsonSerializer.Serialize(list, SerializerOptions);
                await File.WriteAllTextAsync(path, json);
            }
            catch
            {
                // Ignorar errores de escritura
            }
        }
    }
}
