#nullable enable
using System.Runtime.Versioning;
using AutoUpdaterDotNET;
using FluentValidation;
using HSis.Logic.Services;
using HSis.UI.Factories;
using HSis.UI.Forms.Auth;
using HSis.UI.Forms.Dashboards;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;
using HSis.UI.Services;
using Mapster;
using MapsterMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace HSis.UI
{
    [SupportedOSPlatform("windows")]
    internal static class Program
    {
        public static IServiceProvider ServiceProvider { get; private set; } = null!;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // Configurar la fuente por defecto a un tamaño mayor (11 puntos) para mejor legibilidad en todo el sistema
            Application.SetDefaultFont(new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 1. Configurar Serilog
            Log.Logger = new Serilog.LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.File("Logs/hsis_log_.txt", rollingInterval: RollingInterval.Day)
                .CreateLogger();

            try
            {
                Log.Information("Iniciando la aplicación HSis...");

                // 2. Configurar Manejadores Globales de Excepciones
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (sender, e) =>
                {
                    var ex = e.Exception;

                    // A) Errores de Red / Conexión con la Web API
                    if (ex is System.Net.Http.HttpRequestException || ex is System.Net.Sockets.SocketException)
                    {
                        Log.Warning(ex, "Fallo de comunicación con la Web API.");
                        MessageBox.Show(
                            "No se pudo establecer comunicación con el servidor de la aplicación.\n\n" +
                            "• Categoría: ERR-NET-101 (Conexión de Red)\n" +
                            "• Sugerencia: Verifique su conexión de red o contacte al administrador del servidor.",
                            "Error de Conexión",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Warning
                        );
                        return;
                    }

                    // B) Errores Inesperados del Sistema / Crashes
                    string correlationId = Guid.NewGuid().ToString("N")[..6].ToUpper();
                    Log.Fatal(ex, "[Ref: {CorrelationId}] Excepción no manejada en el hilo principal de la UI.", correlationId);

                    MessageBox.Show(
                        $"Ocurrió un problema inesperado al procesar la información.\n\n" +
                        $"• Categoría: ERR-SYS-999 (Error Inesperado)\n" +
                        $"• Código de rastreo: #{correlationId}\n\n" +
                        $"Proporcione este código al equipo de soporte técnico para su revisión.",
                        "Error del Sistema",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error
                    );
                };

                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    string correlationId = Guid.NewGuid().ToString("N")[..6].ToUpper();
                    if (e.ExceptionObject is Exception ex)
                    {
                        Log.Fatal(ex, "[Ref: {CorrelationId}] Excepción no manejada en AppDomain.", correlationId);
                    }
                    else
                    {
                        Log.Fatal("[Ref: {CorrelationId}] Excepción no manejada desconocida en AppDomain: {0}", correlationId, e.ExceptionObject);
                    }
                };

                var services = new ServiceCollection();

                // Registrar IConfiguration en el contenedor DI
                services.AddSingleton<IConfiguration>(configuration);

                // Registrar Logging
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.AddSerilog(dispose: true);
                });

                // Registrar Mapster
                var config = TypeAdapterConfig.GlobalSettings;
                config.Scan(AppDomain.CurrentDomain.GetAssemblies());
                services.AddSingleton(config);
                services.AddScoped<IMapper, ServiceMapper>();

                // Registrar FluentValidation
                services.AddValidatorsFromAssemblyContaining<HSis.Logic.Validators.TicketCreateValidator>();

                // Configurar ApiClients basados en HttpClient
                var baseUrl = configuration.GetSection("ApiSettings")["BaseUrl"] ?? "http://localhost:5000";

                // Registrar JwtAuthHeaderHandler para inyectar token JWT automáticamente
                services.AddTransient<ApiClients.JwtAuthHeaderHandler>();

                services.AddHttpClient<IUsuarioService, ApiClients.UsuarioApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<ITicketService, ApiClients.TicketApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddTransient<ITicketQueryService>(sp => sp.GetRequiredService<ITicketService>());
                services.AddTransient<ITicketCommandService>(sp => sp.GetRequiredService<ITicketService>());
                services.AddTransient<ITicketKpiService>(sp => sp.GetRequiredService<ITicketService>());
                services.AddHttpClient<ICatalogoService, ApiClients.CatalogoApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<ITicketDetalleService, ApiClients.TicketDetalleApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IMaterialService, ApiClients.MaterialApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IReportExportService, ApiClients.ReportExportApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();

                // Almacenamiento local persistente para notificaciones de la UI
                services.AddSingleton<INotificacionStorageService, LocalFileNotificacionStorageService>();
                services.AddSingleton<INotificationEventBus, NotificationEventBus>();
                services.AddSingleton<INotificationClientService, NotificationClientService>();
                services.AddSingleton<ISessionCacheService, SessionCacheService>();
                services.AddSingleton<IFabricaFormularios, FabricaFormularios>();
                services.AddTransient<AdministradorUINotificaciones>();
                services.AddTransient<Presenters.DashboardAdminPresenter>();
                services.AddTransient<Presenters.DashboardTecnicoPresenter>();
                services.AddTransient<Presenters.DashboardClientePresenter>();

                // Registrar Formularios
                services.AddTransient<IniciarSesionForm>();
                services.AddTransient<DashboardAdminForm>();
                services.AddTransient<DashboardClienteForm>();
                services.AddTransient<DashboardTecnicoForm>();
                services.AddTransient<GeneradorReportesForm>();
                services.AddTransient<KardexForm>();
                services.AddTransient<NuevoTicketForm>();

                ServiceProvider = services.BuildServiceProvider();

                // Comprobar actualizaciones automáticas desde el servidor API
                try
                {
                    var updateUrl = $"{baseUrl.TrimEnd('/')}/updates/update.xml";
                    AutoUpdater.Start(updateUrl);
                }
                catch (Exception ex)
                {
                    Log.Warning(ex, "No se pudo verificar la actualización automática.");
                }

                Form? startForm = null;
                var sessionCache = ServiceProvider.GetRequiredService<ISessionCacheService>();
                var cached = sessionCache.GetCredentials();

                if (cached.HasValue)
                {
                    try
                    {
                        var usuarioService = ServiceProvider.GetRequiredService<IUsuarioService>();
                        var usuario = usuarioService.AutenticarAsync(cached.Value.Username, cached.Value.Password).GetAwaiter().GetResult();

                        if (usuario != null)
                        {
                            SesionSistema.UsuarioActual = usuario;

                            // Iniciar SignalR
                            var notificationClient = ServiceProvider.GetRequiredService<INotificationClientService>();
                            string roleName = SesionSistema.IdRolUsuario switch
                            {
                                1 => "Administrador",
                                2 => "Técnico",
                                3 => "Cliente",
                                _ => "Usuario"
                            };
                            notificationClient.IniciarAsync(SesionSistema.IdUsuario, roleName).GetAwaiter().GetResult();

                            startForm = SesionSistema.IdRolUsuario switch
                            {
                                1 => (Form)ServiceProvider.GetRequiredService<DashboardAdminForm>(),
                                2 => (Form)ServiceProvider.GetRequiredService<DashboardTecnicoForm>(),
                                3 => (Form)ServiceProvider.GetRequiredService<DashboardClienteForm>(),
                                _ => (Form)ServiceProvider.GetRequiredService<DashboardAdminForm>()
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error durante el inicio de sesión automático.");
                    }
                }

                startForm ??= ServiceProvider.GetRequiredService<IniciarSesionForm>();

                Application.Run(startForm);
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación terminó inesperadamente debido a una excepción fatal.");
            }
            finally
            {
                Log.Information("Cerrando la aplicación HSis...");
                Log.CloseAndFlush();
            }
        }
    }
}