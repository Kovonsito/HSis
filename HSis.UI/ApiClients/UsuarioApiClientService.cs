using System.Net.Http;
using System.Net.Http.Json;
using HSis.Contracts.DTOs;
using HSis.Contracts.Services;

namespace HSis.UI.ApiClients
{
    public class UsuarioApiClientService(HttpClient httpClient, IAdministradorSesionUsuario sesionUsuario) : IUsuarioService
    {
        public async Task<List<UsuarioDto>> ObtenerUsuariosAsync()
        {
            return await httpClient.GetFromJsonAsync<List<UsuarioDto>>("api/Catalogos/usuarios") ?? [];
        }

        public async Task<UsuarioDto> CrearUsuarioAsync(UsuarioCatalogoRequestDto request)
        {
            var response = await httpClient.PostAsJsonAsync("api/Catalogos/usuarios", request);
            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadFromJsonAsync<UsuarioDto>()
                ?? throw new HttpRequestException("La API no devolvió el usuario creado.");
        }

        public async Task<UsuarioDto?> ActualizarUsuarioAsync(int idUsuario, UsuarioCatalogoRequestDto request)
        {
            var response = await httpClient.PutAsJsonAsync($"api/Catalogos/usuarios/{idUsuario}", request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return await response.Content.ReadFromJsonAsync<UsuarioDto>();
        }

        public async Task<bool> EliminarUsuarioAsync(int idUsuario)
        {
            var response = await httpClient.DeleteAsync($"api/Catalogos/usuarios/{idUsuario}");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return false;
            }

            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return true;
        }


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
                    sesionUsuario.TokenJWT = loginResponse.Token;
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

