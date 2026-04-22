using System;
using System.Windows.Forms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using HSis.Data.Models;
using HSis.Logic.Services;

using HSis.Logic.Interceptors;

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

            var services = new ServiceCollection();
            
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

            Application.Run(ServiceProvider.GetRequiredService<frmIniciarSesion>());
        }
    }
}