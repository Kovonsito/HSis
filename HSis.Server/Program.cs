using HSis.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Configurar para ejecutar como Servicio de Windows
builder.Host.UseWindowsService();

// Configurar la URL de escucha para red local (LAN)
builder.WebHost.UseUrls(builder.Configuration["Urls"] ?? "http://0.0.0.0:5000");

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

var app = builder.Build();

// Habilitar CORS
app.UseCors("AllowAll");

// Mapear el Hub de SignalR
app.MapHub<NotificationHub>("/notificationHub");

// Endpoint básico de verificación de estado
app.MapGet("/", () => new { Status = "HSis SignalR Server is running", DateTime.Now });

app.Run();
