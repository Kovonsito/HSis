using FluentAssertions;
using HSis.Desktop.Infrastructure;
using HSis.UI.Services;
using Xunit;

namespace HSis.Tests;

public sealed class DesktopInfrastructureTests
{
    [Fact]
    public void SolicitudAperturaTicketDebeAceptarFormatoSeparadoYConIgual()
    {
        var solicitud = new SolicitudAperturaTicket(["--open-ticket", "42"]);

        solicitud.Consumir().Should().Be(42);
        solicitud.Consumir().Should().BeNull();

        solicitud.Agregar(["--open-ticket=43"]);
        solicitud.Consumir().Should().Be(43);
    }

    [Fact]
    public void SolicitudAperturaTicketDebeIgnorarIdsInvalidos()
    {
        var solicitud = new SolicitudAperturaTicket(["--open-ticket", "-1"]);

        solicitud.Consumir().Should().BeNull();
    }

    [Fact]
    public void InstanciaAplicacionDebeSerUnicaParaElMismoMutex()
    {
        var nombreMutex = $"Local\\HSis.Tests.{Guid.NewGuid():N}";
        using var primera = new InstanciaAplicacion(nombreMutex);
        using var segunda = new InstanciaAplicacion(nombreMutex);

        primera.EsPrimeraInstancia.Should().BeTrue();
        segunda.EsPrimeraInstancia.Should().BeFalse();
    }

    [Fact]
    public async Task CanalActivacionDebeEnviarArgumentosALaInstanciaPrimaria()
    {
        var nombreCanal = $"HSis.Tests.Activation.{Guid.NewGuid():N}";
        await using var servidor = new CanalActivacionAplicacion(nombreCanal);
        var recibido = new TaskCompletionSource<IReadOnlyList<string>>(
            TaskCreationOptions.RunContinuationsAsynchronously);
        servidor.Iniciar(argumentos => recibido.TrySetResult(argumentos));

        using var cliente = new CanalActivacionAplicacion(nombreCanal);
        cliente.IntentarEnviar(["--open-ticket", "42"]).Should().BeTrue();

        var argumentos = await recibido.Task.WaitAsync(TimeSpan.FromSeconds(2));
        argumentos.Should().Equal("--open-ticket", "42");
    }

    [Fact]
    public void RegistroInicioAutomaticoDebeValidarElComandoConstruido()
    {
        var nombre = $"HSis.Tests.{Guid.NewGuid():N}";
        var registro = new RegistroInicioAutomatico();
        var ruta = Path.Combine(Path.GetTempPath(), "HSis NotificationAgent.exe");

        try
        {
            registro.Registrar(nombre, ruta, "--test").Should().BeTrue();
            registro.EstaRegistrado(nombre).Should().BeTrue();
            registro.ObtenerComando(nombre).Should().Be($"\"{ruta}\" --test");
        }
        finally
        {
            registro.Eliminar(nombre);
        }
    }
}
