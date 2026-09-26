using System.Net.Http;
using System.Net.Http.Headers;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients
{
    public class JwtAuthHeaderHandler(IAdministradorSesionUsuario contextoSesion) : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(contextoSesion.TokenJWT))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", contextoSesion.TokenJWT);
            }

            var response = await base.SendAsync(request, cancellationToken);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                contextoSesion.CerrarSesion();
            }

            return response;
        }
    }
}
