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

namespace HSis.UI
{
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

            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 1. Configurar Serilog
            Serilog.Log.Logger = new Serilog.LoggerConfiguration()
                .ReadFrom.Configuration(configuration)
                .Enrich.FromLogContext()
                .WriteTo.File("Logs/hsis_log_.txt", rollingInterval: Serilog.RollingInterval.Day)
                .CreateLogger();

            try
            {
                Serilog.Log.Information("Iniciando la aplicación HSis...");

                // 2. Configurar Manejadores Globales de Excepciones
                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (sender, e) =>
                {
                    Serilog.Log.Fatal(e.Exception, "Excepción no manejada en el hilo principal de la UI.");
                    MessageBox.Show("Ha ocurrido un error inesperado. El sistema ha guardado los detalles para su revisión.", "Error Fatal", MessageBoxButtons.OK, MessageBoxIcon.Error);
                };

                AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
                {
                    if (e.ExceptionObject is Exception ex)
                    {
                        Serilog.Log.Fatal(ex, "Excepción no manejada en AppDomain.");
                    }
                    else
                    {
                        Serilog.Log.Fatal("Excepción no manejada desconocida en AppDomain: {0}", e.ExceptionObject);
                    }
                };

                var services = new ServiceCollection();

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

                // Registrar Servicios de Lógica
                services.AddTransient<TicketService>();
                services.AddTransient<UsuarioService>();
                services.AddTransient<CatalogoService>();
                services.AddTransient<DetTicketService>();
                services.AddTransient<MaterialService>();

                // Registrar Formularios
                services.AddTransient<frmIniciarSesion>();
                services.AddTransient<frmDashboardAdmin>();
                services.AddTransient<frmDashboardCliente>();
                services.AddTransient<frmDashboardTecnico>();

                ServiceProvider = services.BuildServiceProvider();

                // === ACTUALIZACIÓN AUTOMÁTICA Y ROBUSTA DE LA BASE DE DATOS ===
                try
                {
                    Serilog.Log.Information("Verificando consistencia del esquema de base de datos...");
                    using var dbFactory = ServiceProvider.GetRequiredService<IDbContextFactory<HSisDbContext>>();
                    using var db = dbFactory.CreateDbContext();
                    
                    // Verificar e inyectar la columna Prioridad de forma condicional
                    db.Database.ExecuteSqlRaw(@"
                        IF NOT EXISTS (
                            SELECT * FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'Ticket' AND COLUMN_NAME = 'Prioridad'
                        )
                        BEGIN
                            ALTER TABLE Ticket ADD Prioridad VARCHAR(20) NULL;
                        END
                    ");
                    Serilog.Log.Information("Esquema de base de datos verificado y actualizado con éxito.");
                }
                catch (Exception ex)
                {
                    Serilog.Log.Error(ex, "Error crítico al actualizar el esquema de la base de datos al iniciar.");
                }

                Application.Run(ServiceProvider.GetRequiredService<frmIniciarSesion>());
            }
            catch (Exception ex)
            {
                Serilog.Log.Fatal(ex, "La aplicación terminó inesperadamente debido a una excepción fatal.");
            }
            finally
            {
                Serilog.Log.Information("Cerrando la aplicación HSis...");
                Serilog.Log.CloseAndFlush();
            }
        }
    }
}