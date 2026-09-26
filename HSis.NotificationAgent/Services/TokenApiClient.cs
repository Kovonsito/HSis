using System.Net.Http.Json;
using HSis.Contracts.DTOs;
using Microsoft.Extensions.Logging;

namespace HSis.NotificationAgent.Services;

internal sealed class TokenApiClient(
    HttpClient httpClient,
    ILogger<TokenApiClient> logger)
{
    public async Task<LoginResponseDto?> AutenticarAsync(
        string username,
        string password,
        CancellationToken cancellationToken)
    {
        using var response = await httpClient.PostAsJsonAsync(
            "api/Auth/login",
            new LoginRequestDto
            {
                Username = username,
                Password = password
            },
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning(
                "El agente no pudo autenticarse para el usuario {Username}. Código HTTP: {StatusCode}.",
                username,
                (int)response.StatusCode);
            return null;
        }

        return await response.Content.ReadFromJsonAsync<LoginResponseDto>(cancellationToken);
    }
}
