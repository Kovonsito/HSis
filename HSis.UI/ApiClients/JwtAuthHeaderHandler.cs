using System.Net.Http;
using System.Net.Http.Headers;
using HSis.UI.Helpers;

namespace HSis.UI.ApiClients
{
    public class JwtAuthHeaderHandler : DelegatingHandler
    {
        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrWhiteSpace(SesionSistema.TokenJWT))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", SesionSistema.TokenJWT);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
