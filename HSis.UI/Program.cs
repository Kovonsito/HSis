#nullable enable
using System.Net.Http;
using System.Runtime.Versioning;
using AutoUpdaterDotNET;
using FluentValidation;
using HSis.Contracts.Constants;
using HSis.Contracts.Services;
using HSis.Contracts.Validators;
using HSis.Desktop.Infrastructure;
using HSis.UI.Factories;
using HSis.UI.Services;
using HSis.UI.Forms.Auth;
using HSis.UI.Forms.Catalogos;
using HSis.UI.Forms.Dashboards;
using HSis.UI.Forms.Otros;
using HSis.UI.Forms.Tickets;
using HSis.UI.Helpers;
using HSis.UI.ApiClients;
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
        static void Main(string[] args)
        {
            using var uiInstance = new InstanciaAplicacion(ConfiguracionEscritorio.MutexUi);
            if (!uiInstance.EsPrimeraInstancia)
            {
                if (args.Length > 0)
                {
                    using var activationClient = new CanalActivacionAplicacion();
                    activationClient.IntentarEnviar(args);
                }

                return;
            }

            ApplicationConfiguration.Initialize();

            // Configurar la fuente por defecto a un tamaño mayor (11 puntos) para mejor legibilidad en todo el sistema
            Application.SetDefaultFont(new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point));

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 1. Configurar Serilog
            Log.Logger = new LoggerConfiguration()
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
                    HSis.UI.Helpers.ManejadorErroresUI.Manejar(
                        e.Exception,
                        "una operación de la interfaz",
                        Form.ActiveForm,
                        mostrarDialogo: true,
                        esFatal: true);
                };

                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    if (e.ExceptionObject is Exception ex)
                    {
                        HSis.UI.Helpers.ManejadorErroresUI.Manejar(
                            ex,
                            "el dominio de aplicación",
                            mostrarDialogo: false,
                            esFatal: true);
                    }
                    else
                    {
                        Log.Fatal("Excepción no manejada desconocida en AppDomain: {ExceptionObject}", e.ExceptionObject);
                    }
                };

                var services = new ServiceCollection();

                // Registrar IConfiguration en el contenedor DI
                services.AddSingleton<IConfiguration>(configuration);
                services.AddSingleton(new SolicitudAperturaTicket(args));

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
                services.AddValidatorsFromAssemblyContaining<TicketCreateValidator>();

                // Configurar ApiClients basados en HttpClient
                var baseUrl = configuration.GetSection("ApiSettings")["BaseUrl"] ?? "http://localhost:5000";

                // Registrar JwtAuthHeaderHandler para inyectar token JWT automáticamente
                services.AddTransient<ApiClients.JwtAuthHeaderHandler>();

                void ConfigurarHttpClient(HttpClient c)
                {
                    c.BaseAddress = new Uri(baseUrl);
                    c.Timeout = TimeSpan.FromSeconds(30);
                }

                services.AddHttpClient<IUsuarioService, ApiClients.UsuarioApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<ITicketService, ApiClients.TicketApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<ITicketDetalleService, ApiClients.TicketDetalleApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IMaterialService, ApiClients.MaterialApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IDepartamentoService, ApiClients.CatalogosAdministracionApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<ISucursalService, ApiClients.CatalogosAdministracionApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IEmpresaService, ApiClients.CatalogosAdministracionApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IPuestoService, ApiClients.CatalogosAdministracionApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IRolUsuarioService, ApiClients.CatalogosAdministracionApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<IReportExportService, ApiClients.ReportExportApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();
                services.AddHttpClient<INotificacionesApiClient, ApiClients.NotificacionesApiClientService>(ConfigurarHttpClient)
                        .AddHttpMessageHandler<ApiClients.JwtAuthHeaderHandler>();

                // Servicios de UI centralizados
                services.AddSingleton<IBusEventosNotificaciones, BusEventosNotificaciones>();
                services.AddSingleton<IClienteSignalRNotificaciones, ClienteSignalRNotificaciones>();
                services.AddSingleton<IAlmacenamientoCredencialesLocal, AlmacenamientoCredencialesLocal>();
                services.AddSingleton<PresenciaAplicacion>();
                services.AddSingleton<AdministradorSesionUsuario>();
                services.AddSingleton<IAdministradorSesionUsuario>(sp => sp.GetRequiredService<AdministradorSesionUsuario>());
                services.AddSingleton<ICurrentUserService>(sp => sp.GetRequiredService<AdministradorSesionUsuario>());
                services.AddSingleton<IFabricaFormularios, FabricaFormularios>();
                services.AddSingleton<AperturaTicketService>();
                services.AddSingleton<AgenteNotificacionesLauncher>();

                // Registrar Formularios con inyección directa de dependencias
                services.AddTransient<IniciarSesionForm>();
                services.AddTransient<DashboardAdminForm>();
                services.AddTransient<DashboardClienteForm>();
                services.AddTransient<DashboardTecnicoForm>();
                services.AddTransient<GeneradorReportesForm>();
                services.AddTransient<KardexForm>();
                services.AddTransient<NuevoTicketForm>();
                services.AddTransient<DetalleClienteForm>();
                services.AddTransient<TicketDetalleForm>();
                services.AddTransient<MaterialCatalogoForm>();
                services.AddTransient<UsuarioCatalogoForm>();
                services.AddTransient<DepartamentoCatalogoForm>();
                services.AddTransient<SucursalCatalogoForm>();
                services.AddTransient<EmpresaCatalogoForm>();
                services.AddTransient<PuestoCatalogoForm>();
                services.AddTransient<RolUsuarioCatalogoForm>();

                ServiceProvider = services.BuildServiceProvider();
                using var applicationPresence = ServiceProvider.GetRequiredService<PresenciaAplicacion>();
                using var activationChannel = new CanalActivacionAplicacion();

                var notificationClient = ServiceProvider.GetRequiredService<IClienteSignalRNotificaciones>();
                Application.ApplicationExit += (_, _) =>
                    notificationClient.DetenerAsync().GetAwaiter().GetResult();

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
                var sessionCache = ServiceProvider.GetRequiredService<IAlmacenamientoCredencialesLocal>();
                var notificationAgentLauncher = ServiceProvider.GetRequiredService<AgenteNotificacionesLauncher>();
                var aperturaTicketService = ServiceProvider.GetRequiredService<AperturaTicketService>();
                var cached = sessionCache.GetCredentials();

                if (cached.HasValue)
                {
                    try
                    {
                        var usuarioService = ServiceProvider.GetRequiredService<IUsuarioService>();
                        var usuario = usuarioService.AutenticarAsync(cached.Value.Username, cached.Value.Password).GetAwaiter().GetResult();

                        if (usuario != null)
                        {
                            var contextoSesion = ServiceProvider.GetRequiredService<IAdministradorSesionUsuario>();
                            contextoSesion.UsuarioActual = usuario;

                            // Iniciar SignalR
                            notificationClient.IniciarAsync(contextoSesion.TokenJWT).GetAwaiter().GetResult();
                            notificationAgentLauncher.Iniciar();

                            startForm = (RolUsuarioEnum)contextoSesion.IdRolUsuario switch
                            {
                                RolUsuarioEnum.Administrador => ServiceProvider.GetRequiredService<DashboardAdminForm>(),
                                RolUsuarioEnum.Tecnico => ServiceProvider.GetRequiredService<DashboardTecnicoForm>(),
                                RolUsuarioEnum.Cliente => ServiceProvider.GetRequiredService<DashboardClienteForm>(),
                                _ => ServiceProvider.GetRequiredService<DashboardAdminForm>()
                            };
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error durante el inicio de sesión automático.");
                    }
                }

                startForm ??= ServiceProvider.GetRequiredService<IniciarSesionForm>();

                activationChannel.Iniciar(argumentos =>
                {
                    ServiceProvider.GetRequiredService<SolicitudAperturaTicket>().Agregar(argumentos);
                    if (startForm is not null && !startForm.IsDisposed && startForm.IsHandleCreated)
                    {
                        startForm.BeginInvoke(new Action(() =>
                            aperturaTicketService.AbrirPendiente(startForm)));
                    }
                });

                startForm.Shown += (_, _) =>
                    aperturaTicketService.AbrirPendiente(startForm);

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