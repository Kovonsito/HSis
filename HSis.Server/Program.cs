using System.Reflection;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using FluentValidation;
using HSis.Data.Models;
using HSis.Contracts.Errors;
using HSis.Contracts.Services;
using HSis.Logic.Interceptors;
using HSis.Logic.Services;
using HSis.Server.Configurations;
using HSis.Server.Hubs;
using HSis.Server.Middleware;
using HSis.Server.Services;
using Mapster;
using MapsterMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using HSis.Contracts.Constants;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});
builder.Services.AddWindowsService(options => options.ServiceName = "HSis.Server");

X509Certificate2? productionCertificate = null;
if (builder.Environment.IsProduction())
{
    if (!OperatingSystem.IsWindows())
    {
        throw new InvalidOperationException("La configuración de producción requiere Windows para proteger la clave JWT con DPAPI.");
    }

    var protectedSecretPath = builder.Configuration["JwtSettings:ProtectedSecretPath"];
    if (string.IsNullOrWhiteSpace(protectedSecretPath))
    {
        throw new InvalidOperationException("Falta configurar JwtSettings:ProtectedSecretPath para producción.");
    }

    var protectedSecret = File.ReadAllBytes(protectedSecretPath);
    var decryptedJwtSecret = ProtectedData.Unprotect(protectedSecret, optionalEntropy: null, DataProtectionScope.CurrentUser);
    builder.Configuration["JwtSettings:SecretKey"] = Encoding.UTF8.GetString(decryptedJwtSecret);
    CryptographicOperations.ZeroMemory(decryptedJwtSecret);

    var allowedHosts = builder.Configuration["AllowedHosts"];
    if (string.IsNullOrWhiteSpace(allowedHosts) || allowedHosts.Split(';').Any(host => host.Trim() == "*"))
    {
        throw new InvalidOperationException("En producción, AllowedHosts debe especificar los nombres DNS permitidos.");
    }

    var port = builder.Configuration.GetValue<int?>("Kestrel:HttpsPort") ?? 443;
    if (port is < 1 or > 65535)
    {
        throw new InvalidOperationException("Kestrel:HttpsPort debe estar entre 1 y 65535.");
    }

    var thumbprint = builder.Configuration["Kestrel:CertificateThumbprint"]?.Replace(" ", string.Empty, StringComparison.Ordinal);
    if (string.IsNullOrWhiteSpace(thumbprint))
    {
        throw new InvalidOperationException("Falta configurar Kestrel:CertificateThumbprint para producción.");
    }

    using var certificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
    certificateStore.Open(OpenFlags.ReadOnly);
    productionCertificate = certificateStore.Certificates
        .Find(X509FindType.FindByThumbprint, thumbprint, validOnly: true)
        .OfType<X509Certificate2>()
        .FirstOrDefault(certificate => certificate.HasPrivateKey)
        ?? throw new InvalidOperationException("No se encontró un certificado TLS válido con clave privada en LocalMachine\\My.");

    builder.WebHost.ConfigureKestrel(options =>
        options.ListenAnyIP(port, listenOptions => listenOptions.UseHttps(productionCertificate)));
}

// Registrar controladores de Web API
builder.Services.AddControllers();
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance ??= context.HttpContext.Request.Path;
        context.ProblemDetails.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        var statusCode = context.HttpContext.Response.StatusCode;
        context.ProblemDetails.Extensions.TryAdd("code", ApiExceptionHandler.ObtenerCodigoError(statusCode));
        context.ProblemDetails.Title ??= ApiExceptionHandler.ObtenerTituloError(statusCode);
        context.ProblemDetails.Detail ??= ApiExceptionHandler.ObtenerMensajeError(statusCode);
    };
});
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrar servicios de CORS y SignalR
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowedClients", policy =>
    {
        policy.AllowAnyHeader().AllowAnyMethod();
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins).AllowCredentials();
        }
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
builder.Services.AddValidatorsFromAssemblyContaining<HSis.Contracts.Validators.TicketCreateValidator>();

// Registrar servicios JWT y HttpContextAccessor
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();
builder.Services.AddHttpContextAccessor();

var jwtSettings = builder.Configuration.GetSection("JwtSettings").Get<JwtSettings>()
    ?? throw new InvalidOperationException("Falta la configuración JwtSettings.");
if (string.IsNullOrWhiteSpace(jwtSettings.SecretKey) || Encoding.UTF8.GetByteCount(jwtSettings.SecretKey) < 32)
{
    throw new InvalidOperationException("JwtSettings:SecretKey debe contener al menos 32 bytes aleatorios.");
}
if (string.IsNullOrWhiteSpace(jwtSettings.Issuer) || string.IsNullOrWhiteSpace(jwtSettings.Audience))
{
    throw new InvalidOperationException("JwtSettings:Issuer y JwtSettings:Audience son obligatorios.");
}
if (jwtSettings.ExpirationMinutes <= 0)
{
    throw new InvalidOperationException("JwtSettings:ExpirationMinutes debe ser mayor que cero.");
}
var secretKey = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);
var connectionString = builder.Configuration.GetConnectionString("CadenaSQL");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Falta configurar ConnectionStrings:CadenaSQL.");
}
if (builder.Environment.IsProduction())
{
    var sqlConnection = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(connectionString);
    if (!sqlConnection.IntegratedSecurity)
    {
        throw new InvalidOperationException("En producción, SQL Server debe usar autenticación integrada de Windows.");
    }

    sqlConnection.Encrypt = true;
    sqlConnection.TrustServerCertificate = false;
    connectionString = sqlConnection.ConnectionString;
}

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var requestPath = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                requestPath.StartsWithSegments("/notificationHub"))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidateAudience = true,
        ValidAudience = jwtSettings.Audience,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Registrar la política de autorización para administradores, usando el rol numérico emitido por el JWT actual.
builder.Services.AddAuthorization(options =>
    options.AddPolicy(PoliticasAutorizacion.AdministrarCatalogos, policy =>
        policy.RequireAuthenticatedUser()
            .RequireRole(((int)RolUsuarioEnum.Administrador).ToString())));

// Registrar Sesión de Usuario Real mediante HttpContext (Token JWT)
builder.Services.AddSingleton<ICurrentUserService, CurrentUserService>();
builder.Services.AddSingleton<TicketAuditInterceptor>();

// Configurar DbContextFactory con Interceptor de Auditoría
builder.Services.AddDbContextFactory<HSisDbContext>((sp, options) =>
    options.UseSqlServer(connectionString)
           .AddInterceptors(sp.GetRequiredService<TicketAuditInterceptor>()));

// Registrar Servicios de Lógica mediante Interfaces
builder.Services.AddTransient<ITicketService, TicketService>();
builder.Services.AddTransient<IUsuarioService, UsuarioService>();
builder.Services.AddTransient<IDepartamentoService, DepartamentoService>();
builder.Services.AddTransient<ISucursalService, SucursalService>();
builder.Services.AddTransient<IEmpresaService, EmpresaService>();
builder.Services.AddTransient<IPuestoService, PuestoService>();
builder.Services.AddTransient<IRolUsuarioService, RolUsuarioService>();
builder.Services.AddTransient<ITicketDetalleService, TicketDetalleService>();
builder.Services.AddTransient<IMaterialService, MaterialService>();
builder.Services.AddTransient<IReportExportService, ReportExportService>();
builder.Services.AddTransient<INotificacionService, NotificacionService>();
builder.Services.AddTransient<INotificacionDestinatariosService, NotificacionDestinatariosService>();
builder.Services.AddTransient<NotificacionTicketCoordinator>();
builder.Services.AddTransient<IServerNotificationDispatcher, ServerNotificationDispatcher>();

var app = builder.Build();
if (productionCertificate is not null)
{
    app.Lifetime.ApplicationStopped.Register(productionCertificate.Dispose);
}

// Activar el pipeline global de excepciones con IExceptionHandler y ProblemDetails
app.UseExceptionHandler();
app.UseStatusCodePages();

// Habilitar Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Habilitar servir archivos estáticos (para carpeta wwwroot/updates)
app.UseStaticFiles();

// Habilitar CORS
app.UseCors("AllowedClients");

// Habilitar Autenticación y Autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers(); // Mapear controladores REST

// Mapear el Hub de SignalR
app.MapHub<NotificationHub>("/notificationHub").RequireAuthorization();

// Endpoint básico de verificación de estado
app.MapGet("/", () => new { Status = "HSis Web API Server is running", DateTime.Now });

app.Run();

public partial class Program;

