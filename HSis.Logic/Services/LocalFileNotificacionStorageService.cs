using System.Text.Json;

namespace HSis.Logic.Services;

public class LocalFileNotificacionStorageService : INotificacionStorageService
{
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

        if (list.Count > 50)
        {
            list = [.. list.Take(50)];
        }

        await GuardarListaAsync(userId, list);
    }

    public async Task MarcarComoLeidaAsync(int userId, Guid id)
    {
        var list = await ObtenerNotificacionesAsync(userId);
        var target = list.FirstOrDefault(n => n.Id == id);
        if (target != null)
        {
            target.Leido = true;
            await GuardarListaAsync(userId, list);
        }
    }

    public async Task MarcarTodasComoLeidasAsync(int userId)
    {
        var list = await ObtenerNotificacionesAsync(userId);
        if (list.Count == 0) return;

        foreach (var item in list)
        {
            item.Leido = true;
        }
        await GuardarListaAsync(userId, list);
    }

    public async Task LimpiarTodasAsync(int userId)
    {
        var path = GetFilePath(userId);
        if (File.Exists(path))
        {
            try
            {
                File.Delete(path);
            }
            catch
            {
                // Ignorar si no se puede eliminar el archivo
            }
        }
        await Task.CompletedTask;
    }

    public Task SincronizarDesdeBDAsync(int userId)
    {
        return Task.CompletedTask;
    }


    private static async Task GuardarListaAsync(int userId, List<NotificacionLocal> list)
    {
        try
        {
            var path = GetFilePath(userId);
            var json = JsonSerializer.Serialize(list, SerializerOptions);
            await File.WriteAllTextAsync(path, json);
        }
        catch
        {
            // Ignorar errores de escritura local
        }
    }
}
