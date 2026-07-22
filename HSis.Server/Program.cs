using HSis.Server.Middleware;
using HSis.Server.Hubs;
using HSis.Data.Models;
using HSis.Logic.Services;
using HSis.Logic.Interceptors;
using MapsterMapper;
using Mapster;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Configurar para ejecutar como Servicio de Windows
builder.Host.UseWindowsService();

// Configurar la URL de escucha para red local (LAN)
builder.WebHost.UseUrls(builder.Configuration["Urls"] ?? "http://0.0.0.0:5000");

// Registrar controladores de Web API
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar servicios de CORS y SignalR
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyHeader()
              .AllowAnyMethod()
              .SetIsOriginAllowed(_ => true) // Requerido para SignalR en clientes de escritorio
              .AllowCredentials();
    });
});

builder.Services.AddSignalR();

// === CONFIGURACIÓN DE DEPENDENCIAS (Extraída de HSis.UI) ===

// Registrar Mapster
var config = TypeAdapterConfig.GlobalSettings;
config.Scan(Assembly.Load("HSis.Logic"));
builder.Services.AddSingleton(config);
builder.Services.AddScoped<IMapper, ServiceMapper>();

// Registrar FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<HSis.Logic.Validators.TicketCreateValidator>();

// Configurar Sesión de Usuario (Mock para el API de momento, se debe usar JWT HttpContext)
builder.Services.AddSingleton<ICurrentUserService, ServicioUsuarioActualMock>();
builder.Services.AddSingleton<TicketAuditInterceptor>();

// Configurar DbContextFactory con Interceptor
builder.Services.AddDbContextFactory<HSisDbContext>((sp, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"))
           .AddInterceptors(sp.GetRequiredService<TicketAuditInterceptor>()));

// Registrar Servicios de Lógica mediante Interfaces
builder.Services.AddTransient<ITicketService, TicketService>();
builder.Services.AddTransient<IUsuarioService, UsuarioService>();
builder.Services.AddTransient<ICatalogoService, CatalogoService>();
builder.Services.AddTransient<ITicketDetalleService, TicketDetalleService>();
builder.Services.AddTransient<IMaterialService, MaterialService>();
builder.Services.AddTransient<IReportExportService, ReportExportService>();

var app = builder.Build();

// Registrar Middleware de Manejo de Excepciones Globales
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Habilitar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar servir archivos estáticos (para carpeta wwwroot/updates)
app.UseStaticFiles();

// Habilitar CORS
app.UseCors("AllowAll");

app.MapControllers(); // Mapear controladores REST

// Mapear el Hub de SignalR
app.MapHub<NotificationHub>("/notificationHub");

// Endpoint básico de verificación de estado
app.MapGet("/", () => new { Status = "HSis Web API Server is running", DateTime.Now });

app.Run();

// Clase Mock para ICurrentUserService en la API (hasta que implementemos JWT auth)
public class ServicioUsuarioActualMock : ICurrentUserService
{
    public HSis.Logic.DTOs.UsuarioDto? ObtenerUsuarioActual() 
    {
        return new HSis.Logic.DTOs.UsuarioDto { IdUsuario = 1, Nombre = "API User" };
    }
    
    public int GetCurrentUserId()
    {
        return 1;
    }
}

