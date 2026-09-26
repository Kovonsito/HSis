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
            return await httpClient.GetFromJsonWithDetailsAsync<List<UsuarioDto>>("api/Catalogos/usuarios") ?? [];
        }

        public async Task<UsuarioDto> CrearUsuarioAsync(UsuarioCatalogoRequestDto request)
        {
            using var response = await httpClient.PostAsJsonAsync("api/Catalogos/usuarios", request);
            return await response.ReadRequiredJsonWithDetailsAsync<UsuarioDto>("usuario creado");
        }

        public async Task<UsuarioDto?> ActualizarUsuarioAsync(int idUsuario, UsuarioCatalogoRequestDto request)
        {
            using var response = await httpClient.PutAsJsonAsync($"api/Catalogos/usuarios/{idUsuario}", request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }

            return await response.ReadFromJsonWithDetailsAsync<UsuarioDto>();
        }

        public async Task<bool> EliminarUsuarioAsync(int idUsuario)
        {
            using var response = await httpClient.DeleteAsync($"api/Catalogos/usuarios/{idUsuario}");
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
            var request = new HSis.Contracts.DTOs.LoginRequestDto
            {
                Username = nombreUsuario,
                Password = contraseña
            };
            using var response = await httpClient.PostAsJsonAsync("api/Auth/login", request);

            if (response.IsSuccessStatusCode)
            {
                var loginResponse = await response.ReadFromJsonWithDetailsAsync<LoginResponseDto>();
                if (loginResponse != null)
                {
                    sesionUsuario.TokenJWT = loginResponse.Token;
                    return loginResponse.Usuario;
                }
            }

            await response.EnsureSuccessStatusCodeWithDetailsAsync();
            return null;
        }

        public async Task<List<UsuarioDto>> ObtenerUsuariosPorRolAsync(int idRol)
        {
            return await httpClient.GetFromJsonWithDetailsAsync<List<UsuarioDto>>($"api/Usuarios/rol/{idRol}") ?? [];
        }

    }
}

