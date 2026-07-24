using System.Net.Http;
using System.Net.Http.Json;
using HSis.Logic.DTOs;
using HSis.Logic.Services;
using HSis.UI.Helpers;

namespace HSis.UI.ApiClients
{
    public class UsuarioApiClientService(HttpClient httpClient) : IUsuarioService
    {

        public Task RehashearContraseñasAsync()
        {
            throw new NotImplementedException("No soportado en el cliente");
        }

        public async Task<UsuarioDto?> AutenticarAsync(string nombreUsuario, string contraseña)
        {
            var request = new { Username = nombreUsuario, Password = contraseña };
            var response = await httpClient.PostAsJsonAsync("api/Auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponseDto>();
                if (loginResponse != null)
                {
                    SesionSistema.TokenJWT = loginResponse.Token;
                    return loginResponse.Usuario;
                }
            }
            return null;
        }

        public async Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(int idRol)
        {
            return await httpClient.GetFromJsonAsync<List<UsuarioDto>>($"api/Usuarios/rol/{idRol}") ?? [];
        }
    }
}
