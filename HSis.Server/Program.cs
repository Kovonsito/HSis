using System.Reflection;
using System.Text;
using FluentValidation;
using HSis.Data.Models;
using HSis.Logic.Interceptors;
using HSis.Logic.Services;
using HSis.Server.Configurations;
using HSis.Server.Hubs;
using HSis.Server.Middleware;
using HSis.Server.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Configurar para ejecutar como Servicio de Windows si la plataforma es Windows
if (OperatingSystem.IsWindows())
{
    builder.Host.UseWindowsService(options =>
    {
        options.ServiceName = "HSisNotificationServer";
    });
}

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

// Registrar servicios JWT y HttpContextAccessor
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddHttpContextAccessor();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>() ?? new JwtSettings();
string secretKeyString = string.IsNullOrWhiteSpace(jwtSettings.SecretKey)
    ? "HSis_Secret_Key_For_JWT_Authentication_Token_2026_SecureKey!"
    : jwtSettings.SecretKey;
var secretKey = Encoding.UTF8.GetBytes(secretKeyString);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = !string.IsNullOrEmpty(jwtSettings.Issuer),
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = !string.IsNullOrEmpty(jwtSettings.Audience),
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Registrar Sesión de Usuario Real mediante HttpContext (Token JWT)
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<TicketAuditInterceptor>();

// Configurar DbContextFactory con Interceptor de Auditoría
builder.Services.AddDbContextFactory<HSisDbContext>((sp, options) =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("CadenaSQL"))
           .AddInterceptors(sp.GetRequiredService<TicketAuditInterceptor>()));

// Registrar Servicios de Lógica mediante Interfaces
builder.Services.AddTransient<ITicketService, TicketService>();
builder.Services.AddTransient<ITicketQueryService>(sp => sp.GetRequiredService<ITicketService>());
builder.Services.AddTransient<ITicketCommandService>(sp => sp.GetRequiredService<ITicketService>());
builder.Services.AddTransient<ITicketKpiService>(sp => sp.GetRequiredService<ITicketService>());
builder.Services.AddTransient<IUsuarioService, UsuarioService>();
builder.Services.AddTransient<ICatalogoService, CatalogoService>();
builder.Services.AddTransient<ITicketDetalleService, TicketDetalleService>();
builder.Services.AddTransient<IMaterialService, MaterialService>();
builder.Services.AddTransient<IReportExportService, ReportExportService>();
builder.Services.AddTransient<INotificacionStorageService, NotificacionStorageService>();
builder.Services.AddSingleton<INotificationEventBus, NotificationEventBus>();
builder.Services.AddTransient<IServerNotificationDispatcher, ServerNotificationDispatcher>();

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

// Habilitar Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Mapear controladores REST

// Mapear el Hub de SignalR
app.MapHub<NotificationHub>("/notificationHub");

// Endpoint básico de verificación de estado
app.MapGet("/", () => new { Status = "HSis Web API Server is running", DateTime.Now });

app.Run();


