using System.Text.Json;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using HSis.Contracts.Errors;
using HSis.Logic.Exceptions;
using HSis.Server.Middleware;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace HSis.Tests.Server;

public sealed class ApiExceptionHandlerTests
{
    [Fact]
    public async Task TryHandleAsyncConValidationExceptionDebeDevolverProblemDetailsConErrores()
    {
        var contexto = CrearContexto("trace-validation");
        var exception = new ValidationException(
        [
            new ValidationFailure("Nombre", "El nombre es obligatorio."),
            new ValidationFailure("Nombre", "El nombre debe ser único.")
        ]);

        var handled = await CrearHandler().TryHandleAsync(contexto, exception, CancellationToken.None);

        handled.Should().BeTrue();
        contexto.Response.StatusCode.Should().Be(StatusCodes.Status400BadRequest);
        var problem = await LeerProblemDetailsAsync(contexto);
        problem.GetProperty("code").GetString().Should().Be(ApiErrorCodes.Validation);
        problem.GetProperty("traceId").GetString().Should().Be("trace-validation");
        problem.GetProperty("errors").GetProperty("Nombre").GetArrayLength().Should().Be(2);
    }

    [Fact]
    public async Task TryHandleAsyncConExcepcionDeDominioDebeDevolverCodigoYEstadoCorrespondientes()
    {
        var contexto = CrearContexto("trace-domain");
        var exception = new TicketNotFoundException(42);

        var handled = await CrearHandler().TryHandleAsync(contexto, exception, CancellationToken.None);

        handled.Should().BeTrue();
        contexto.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
        var problem = await LeerProblemDetailsAsync(contexto);
        problem.GetProperty("code").GetString().Should().Be(ApiErrorCodes.TicketNotFound);
        problem.GetProperty("detail").GetString().Should().Contain("ticket");
        problem.GetProperty("traceId").GetString().Should().Be("trace-domain");
    }

    [Fact]
    public async Task TryHandleAsyncConExcepcionNoMapeadaDebeOcultarDetallesTecnicos()
    {
        var contexto = CrearContexto("trace-unexpected");
        var exception = new Exception("Detalle interno que no debe exponerse");

        var handled = await CrearHandler().TryHandleAsync(contexto, exception, CancellationToken.None);

        handled.Should().BeTrue();
        contexto.Response.StatusCode.Should().Be(StatusCodes.Status500InternalServerError);
        var problem = await LeerProblemDetailsAsync(contexto);
        problem.GetProperty("code").GetString().Should().Be(ApiErrorCodes.InternalServerError);
        problem.GetProperty("detail").GetString().Should().NotContain("Detalle interno");
    }

    private static ApiExceptionHandler CrearHandler()
        => new(NullLogger<ApiExceptionHandler>.Instance);

    private static DefaultHttpContext CrearContexto(string traceId)
    {
        var contexto = new DefaultHttpContext
        {
            TraceIdentifier = traceId
        };
        contexto.Response.Body = new MemoryStream();
        return contexto;
    }

    private static async Task<JsonElement> LeerProblemDetailsAsync(DefaultHttpContext contexto)
    {
        contexto.Response.Body.Position = 0;
        using var documento = await JsonDocument.ParseAsync(contexto.Response.Body);
        return documento.RootElement.Clone();
    }
}
