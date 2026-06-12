#nullable enable
using Mapster;
using MapsterMapper;
using FluentValidation;
using HSis.Data.Models;
using HSis.Logic.Interceptors;
using HSis.Logic.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System.Runtime.Versioning;

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

                // Configurar Sesión de Usuario e Interceptor
                services.AddSingleton<ICurrentUserService, CurrentUserService>();
                services.AddSingleton<TicketAuditInterceptor>();

                // Configurar DbContextFactory con Interceptor
                services.AddDbContextFactory<HSisDbContext>((sp, options) =>
                    options.UseSqlServer(configuration.GetConnectionString("CadenaSQL"))
                           .AddInterceptors(sp.GetRequiredService<TicketAuditInterceptor>()));

                // Registrar Servicios de Lógica mediante Interfaces
                services.AddTransient<ITicketService, TicketService>();
                services.AddTransient<IUsuarioService, UsuarioService>();
                services.AddTransient<ICatalogoService, CatalogoService>();
                services.AddTransient<ITicketDetalleService, TicketDetalleService>();
                services.AddTransient<IMaterialService, MaterialService>();
                services.AddTransient<IReportExportService, ReportExportService>();
                services.AddSingleton<INotificationClientService, NotificationClientService>();
                services.AddSingleton<ISessionCacheService, SessionCacheService>();
                services.AddSingleton<INotificacionStorageService, NotificacionStorageService>();
                services.AddSingleton<IFormFactory, FormFactory>();
                services.AddTransient<NotificationUIManager>();

                // Registrar Formularios
                services.AddTransient<IniciarSesionForm>();
                services.AddTransient<DashboardAdminForm>();
                services.AddTransient<DashboardClienteForm>();
                services.AddTransient<DashboardTecnicoForm>();
                services.AddTransient<GeneradorReportesForm>();
                services.AddTransient<KardexForm>();
                services.AddTransient<NuevoTicketForm>();

                ServiceProvider = services.BuildServiceProvider();

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
}