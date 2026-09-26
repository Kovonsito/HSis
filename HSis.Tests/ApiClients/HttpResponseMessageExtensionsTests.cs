using System.Net;
using System.Net.Http.Json;
using System.Text;
using FluentAssertions;
using HSis.Contracts.DTOs;
using HSis.UI.ApiClients;
using Xunit;

namespace HSis.Tests.ApiClients;

public sealed class HttpResponseMessageExtensionsTests
{
    [Fact]
    public async Task ReadFromJsonWithDetailsAsyncDebeDeserializarRespuestaCorrecta()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = JsonContent.Create(new NotificacionDto(1, 42, "Tipo", "Mensaje", DateTimeOffset.UtcNow, false))
        };

        var resultado = await response.ReadFromJsonWithDetailsAsync<NotificacionDto>();

        resultado.Should().NotBeNull();
        resultado!.IdNotificacion.Should().Be(1);
        resultado.TicketId.Should().Be(42);
    }

    [Fact]
    public async Task ReadFromJsonWithDetailsAsyncConNoContentDebeRetornarNulo()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.NoContent);

        var resultado = await response.ReadFromJsonWithDetailsAsync<NotificacionDto>();

        resultado.Should().BeNull();
    }

    [Fact]
    public async Task EnsureSuccessStatusCodeWithDetailsAsyncDebeConservarProblemDetailsYValidaciones()
    {
        using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
        {
            Content = new StringContent(
                "{\"type\":\"https://httpstatuses.com/400\",\"title\":\"Validación\",\"detail\":\"Revise los datos.\",\"code\":\"VALIDATION_ERROR\",\"traceId\":\"trace-123\",\"errors\":{\"Nombre\":[\"El nombre es obligatorio.\"]}}",
                Encoding.UTF8,
                "application/problem+json")
        };

        var exception = await Assert.ThrowsAsync<ApiException>(
            () => response.EnsureSuccessStatusCodeWithDetailsAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        exception.Code.Should().Be("VALIDATION_ERROR");
        exception.TraceId.Should().Be("trace-123");
        exception.Title.Should().Be("Validación");
        exception.ValidationErrors.Should().ContainKey("Nombre");
        exception.UserMessage.Should().Contain("El nombre es obligatorio.");
    }

    [Theory]
    [InlineData(HttpStatusCode.Unauthorized, "sesión")]
    [InlineData(HttpStatusCode.NotFound, "información")]
    [InlineData(HttpStatusCode.Conflict, "información cambió")]
    public async Task EnsureSuccessStatusCodeWithDetailsAsyncDebeMapearMensajesPorCodigo(
        HttpStatusCode statusCode,
        string fragmento)
    {
        using var response = new HttpResponseMessage(statusCode);

        var exception = await Assert.ThrowsAsync<ApiException>(
            () => response.EnsureSuccessStatusCodeWithDetailsAsync());

        exception.UserMessage.Should().Contain(fragmento);
    }
}
