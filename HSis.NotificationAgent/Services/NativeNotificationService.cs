using HSis.Contracts.DTOs;
using HSis.Desktop.Infrastructure;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace HSis.NotificationAgent.Services;

internal sealed class NativeNotificationService(
    ILogger<NativeNotificationService> logger,
    IConfiguration configuration) : IDisposable
{
    private readonly ILogger<NativeNotificationService> _logger = logger;
    private readonly IConfiguration _configuration = configuration;
    private int _registered;
    private int _disposed;

    public void Register()
    {
        if (Interlocked.Exchange(ref _registered, 1) != 0)
        {
            return;
        }

        try
        {
            var manager = AppNotificationManager.Default;
            manager.NotificationInvoked += OnNotificationInvoked;
            manager.Register();
        }
        catch
        {
            Interlocked.Exchange(ref _registered, 0);
            throw;
        }
    }

    public void Show(NotificacionDto notification)
    {
        ArgumentNullException.ThrowIfNull(notification);

        if (Volatile.Read(ref _registered) == 0 || Volatile.Read(ref _disposed) != 0)
        {
            return;
        }

        try
        {
            var builder = new AppNotificationBuilder()
                .AddArgument("action", "open-ticket")
                .AddArgument("notificationId", notification.IdNotificacion.ToString())
                .AddArgument("ticketId", notification.TicketId?.ToString() ?? string.Empty)
                .AddArgument("materialId", notification.MaterialId?.ToString() ?? string.Empty)
                .AddText("HSis")
                .AddText(notification.Mensaje);

            AppNotificationManager.Default.Show(builder.BuildNotification());
        }
        catch (Exception ex)
        {
            _logger.LogWarning(
                ex,
                "No se pudo mostrar la notificación nativa {NotificationId}.",
                notification.IdNotificacion);
        }
    }

    private void OnNotificationInvoked(
        AppNotificationManager sender,
        AppNotificationActivatedEventArgs args)
    {
        var ticketId = ObtenerArgumento(args.Argument, "ticketId");
        if (int.TryParse(ticketId, out var idTicket) && idTicket > 0)
        {
            var executablePath = ObtenerRutaUi();
            if (string.IsNullOrWhiteSpace(executablePath))
            {
                _logger.LogWarning("No se encontró el ejecutable de la UI; no se puede abrir el ticket {TicketId}.", idTicket);
                return;
            }

            try
            {
                using var activationClient = new CanalActivacionAplicacion();
                if (!activationClient.IntentarEnviar(["--open-ticket", idTicket.ToString()]))
                {
                    LanzadorProceso.Iniciar(executablePath, $"--open-ticket {idTicket}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "No se pudo activar la UI para abrir el ticket {TicketId}.", idTicket);
            }
        }

        _logger.LogInformation(
            "Notificación nativa activada con argumentos {Arguments}.",
            args.Argument);
    }

    private string? ObtenerRutaUi()
    {
        var configuredPath = _configuration["NotificationAgent:UIExecutablePath"];
        if (!string.IsNullOrWhiteSpace(configuredPath))
        {
            var fullPath = Path.GetFullPath(configuredPath, AppContext.BaseDirectory);
            return File.Exists(fullPath) ? fullPath : null;
        }

        var defaultPath = Path.Combine(AppContext.BaseDirectory, "HSis.UI.exe");
        return File.Exists(defaultPath) ? defaultPath : null;
    }

    private static string? ObtenerArgumento(string argumentos, string nombre)
        => argumentos
            .Split('&', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(argumento => argumento.Split('=', 2))
            .Where(partes => partes.Length == 2)
            .FirstOrDefault(partes => string.Equals(partes[0], nombre, StringComparison.OrdinalIgnoreCase))?[1];

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        if (Interlocked.Exchange(ref _registered, 0) == 0)
        {
            return;
        }

        try
        {
            var manager = AppNotificationManager.Default;
            manager.NotificationInvoked -= OnNotificationInvoked;
            manager.Unregister();
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "No se pudo liberar el registro de notificaciones nativas.");
        }
    }
}
