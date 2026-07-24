#nullable enable
using System.Runtime.Versioning;
using FluentValidation;
using AutoUpdaterDotNET;
using HSis.Data.Models;
using HSis.Logic.Interceptors;
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
using Microsoft.EntityFrameworkCore;
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
                    Log.Fatal(e.Exception, "Excepción no manejada en el hilo principal de la UI.");
                    MessageBox.Show("Ha ocurrido un error inesperado. El sistema ha guardado los detalles para su revisión.", "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    if (e.ExceptionObject is Exception ex)
                    {
                        Log.Fatal(ex, "Excepción no manejada en AppDomain.");
                    }
                    else
                    {
                        Log.Fatal("Excepción no manejada desconocida en AppDomain: {0}", e.ExceptionObject);
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
                services.AddHttpClient<ICatalogoService, ApiClients.CatalogoApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<ITicketDetalleService, ApiClients.TicketDetalleApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IMaterialService, ApiClients.MaterialApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IReportExportService, ApiClients.ReportExportApiClientService>(c => c.BaseAddress = new Uri(baseUrl))
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();

                // Mocks temporales para servicios que requerían EF local
                services.AddSingleton<INotificacionStorageService, MockNotificacionStorageService>();

                services.AddSingleton<INotificationClientService, NotificationClientService>();
                services.AddSingleton<ISessionCacheService, SessionCacheService>();
                services.AddSingleton<IFabricaFormularios, FabricaFormularios>();
                services.AddTransient<AdministradorUINotificaciones>();

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
                                1 => "Admin",
                                2 => "Tecnico",
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

    // Mock temporal para INotificacionStorageService (ya que usaba EF)
    public class MockNotificacionStorageService : INotificacionStorageService
    {
        public Task<List<NotificacionLocal>> ObtenerNotificacionesAsync(int userId) => Task.FromResult(new List<NotificacionLocal>());
        public Task GuardarNotificacionAsync(int userId, int ticketId, string mensaje) => Task.CompletedTask;
        public Task MarcarComoLeidaAsync(int userId, Guid id) => Task.CompletedTask;
        public Task LimpiarTodasAsync(int userId) => Task.CompletedTask;
        public Task SincronizarDesdeBDAsync(int userId) => Task.CompletedTask;
    }
}