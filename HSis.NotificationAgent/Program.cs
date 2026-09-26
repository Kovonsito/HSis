using System.Runtime.Versioning;
using HSis.Contracts.Services;
using HSis.Desktop.Infrastructure;
using HSis.NotificationAgent.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace HSis.NotificationAgent;

[SupportedOSPlatform("windows")]
internal static class Program
{
    private const string AgentMutexName = ConfiguracionEscritorio.MutexAgenteNotificaciones;

    [STAThread]
    private static void Main()
    {
        using var mutex = new Mutex(initiallyOwned: true, AgentMutexName, out var createdNew);
        if (!createdNew)
        {
            return;
        }

        ApplicationConfiguration.Initialize();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        using var services = CrearProveedorServicios(configuration);
        RegistrarInicioAutomatico(configuration);
        var host = services.GetRequiredService<NotificationAgentHost>();

        try
        {
            var iniciado = host.StartAsync().GetAwaiter().GetResult();
            if (!iniciado)
            {
                return;
            }

            Application.Run(new ApplicationContext());
        }
        catch (Exception ex)
        {
            var logger = services.GetService<ILoggerFactory>()?.CreateLogger("HSis.NotificationAgent");
            logger?.LogCritical(ex, "El agente de notificaciones terminó inesperadamente.");
        }
        finally
        {
            host.DisposeAsync().AsTask().GetAwaiter().GetResult();
        }
    }

    private static ServiceProvider CrearProveedorServicios(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSingleton(configuration);
        services.AddLogging(builder =>
        {
            builder.ClearProviders();
            builder.AddSimpleConsole(options => options.SingleLine = true);
            builder.SetMinimumLevel(LogLevel.Information);
        });

        var baseUrl = configuration["ApiSettings:BaseUrl"] ?? "http://localhost:5000";
        var apiBaseAddress = new Uri($"{baseUrl.TrimEnd('/')}/");

        services.AddHttpClient<TokenApiClient>(client =>
        {
            client.BaseAddress = apiBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddHttpClient<NotificationAgentHost>(client =>
        {
            client.BaseAddress = apiBaseAddress;
            client.Timeout = TimeSpan.FromSeconds(30);
        });
        services.AddSingleton<IAlmacenamientoCredencialesLocal, AlmacenamientoCredencialesLocal>();
        services.AddSingleton<PresenciaAplicacion>(serviceProvider =>
            new PresenciaAplicacion(configuration["NotificationAgent:PresenceFileName"]));
        services.AddSingleton<NativeNotificationService>();
        services.AddTransient<NotificationAgentHost>();

        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
    }

    private static void RegistrarInicioAutomatico(IConfiguration configuration)
    {
        if (!configuration.GetValue("NotificationAgent:RegisterStartup", true) ||
            string.IsNullOrWhiteSpace(Environment.ProcessPath))
        {
            return;
        }

        var registro = new RegistroInicioAutomatico();
        if (!registro.Registrar(
                ConfiguracionEscritorio.NombreInicioAgente,
                Environment.ProcessPath!))
        {
            Console.Error.WriteLine("No se pudo registrar el inicio automático del agente.");
        }
    }
}
