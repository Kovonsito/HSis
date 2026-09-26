using System.Net.Http.Headers;
using HSis.Contracts.Constants;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;
using HSis.Data.Models;
using HSis.Server.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace HSis.Tests.Integration;

public sealed class NotificationServerFixture : WebApplicationFactory<Program>
{
    private readonly string databaseName = $"HSis.Integration.{Guid.NewGuid():N}";
    private readonly SemaphoreSlim seedLock = new(1, 1);

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<IDbContextFactory<HSisDbContext>>();
            services.RemoveAll<DbContextOptions<HSisDbContext>>();
            services.RemoveAll<Microsoft.EntityFrameworkCore.Infrastructure.IDbContextOptionsConfiguration<HSisDbContext>>();
            services.AddDbContextFactory<HSisDbContext>(options =>
                options.UseInMemoryDatabase(databaseName));
        });
    }

    public async Task ReiniciarDatosAsync()
    {
        await seedLock.WaitAsync();
        try
        {
            using var scope = Services.CreateScope();
            var dbContextFactory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<HSisDbContext>>();
            await using var db = await dbContextFactory.CreateDbContextAsync();

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();

            db.RolesUsuarios.AddRange(
                new RolUsuario { IdRol = (int)RolUsuarioEnum.Administrador, Descripcion = "Administrador" },
                new RolUsuario { IdRol = (int)RolUsuarioEnum.Tecnico, Descripcion = "Técnico" },
                new RolUsuario { IdRol = (int)RolUsuarioEnum.Cliente, Descripcion = "Cliente" });

            db.Usuarios.AddRange(
                new Usuario
                {
                    IdUsuario = 100,
                    Nombre = "Cliente de integración",
                    IdRol = (int)RolUsuarioEnum.Cliente,
                    Contraseña = "cliente"
                },
                new Usuario
                {
                    IdUsuario = 200,
                    Nombre = "Técnico de integración",
                    IdRol = (int)RolUsuarioEnum.Tecnico,
                    Contraseña = "tecnico"
                },
                new Usuario
                {
                    IdUsuario = 300,
                    Nombre = "Administrador de integración",
                    IdRol = (int)RolUsuarioEnum.Administrador,
                    Contraseña = "administrador"
                });

            db.Tickets.Add(new Ticket
            {
                IdTicket = 42,
                IdUsuario = 100,
                IdTecnico = 200,
                FechaAlta = DateTime.UtcNow,
                Estatus = ConstantesEstatus.ABIERTO,
                Descripcion = "Ticket de integración",
                Prioridad = ConstantesPrioridad.MEDIA
            });

            var fechaBase = DateTime.UtcNow.AddMinutes(-2);
            db.Notificaciones.AddRange(
                new Notificacion
                {
                    IdNotificacion = 1001,
                    UsuarioDestinoId = 100,
                    TicketId = 42,
                    Tipo = ConstantesTiposNotificacion.EstadoTicket,
                    Mensaje = "El ticket 42 cambió a En proceso.",
                    FechaCreacion = fechaBase,
                    Leido = false
                },
                new Notificacion
                {
                    IdNotificacion = 1002,
                    UsuarioDestinoId = 100,
                    TicketId = 42,
                    Tipo = ConstantesTiposNotificacion.NuevoTicket,
                    Mensaje = "Historial leído del ticket 42.",
                    FechaCreacion = fechaBase.AddMinutes(1),
                    Leido = true
                },
                new Notificacion
                {
                    IdNotificacion = 2001,
                    UsuarioDestinoId = 200,
                    TicketId = 42,
                    Tipo = ConstantesTiposNotificacion.Calificacion,
                    Mensaje = "Calificación del ticket 42.",
                    FechaCreacion = fechaBase,
                    Leido = false
                });

            await db.SaveChangesAsync();
        }
        finally
        {
            seedLock.Release();
        }
    }

    public HttpClient CrearClienteAutenticado(int usuarioId, RolUsuarioEnum rol)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", CrearToken(usuarioId, rol));
        return client;
    }

    public string CrearToken(int usuarioId, RolUsuarioEnum rol)
    {
        var tokenService = Services.GetRequiredService<IJwtTokenService>();
        return tokenService.GenerarToken(new UsuarioDto
        {
            IdUsuario = usuarioId,
            Nombre = $"Usuario {usuarioId}",
            IdRol = (int)rol
        });
    }

    public HubConnection CrearConexion(string? token)
    {
        return new HubConnectionBuilder()
            .WithUrl(
                new Uri(Server.BaseAddress, "notificationHub"),
                options =>
                {
                    options.AccessTokenProvider = token is null
                        ? null
                        : () => Task.FromResult<string?>(token);
                    options.HttpMessageHandlerFactory = _ => Server.CreateHandler();
                    options.Transports = HttpTransportType.LongPolling;
                })
            .Build();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            seedLock.Dispose();
        }

        base.Dispose(disposing);
    }
}

[CollectionDefinition(Name, DisableParallelization = true)]
public sealed class NotificationServerCollection : ICollectionFixture<NotificationServerFixture>
{
    public const string Name = "Notification server integration collection";
}
