using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HSis.Logic.DTOs;
using HSis.Server.Configurations;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace HSis.Server.Services
{
    public interface IJwtTokenService
    {
        string GenerarToken(UsuarioDto usuario);
    }

    public class JwtTokenService(IOptions<JwtSettings> jwtSettings) : IJwtTokenService
    {
        private readonly JwtSettings _jwtSettings = jwtSettings.Value;

        public string GenerarToken(UsuarioDto usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            string secretKeyString = string.IsNullOrWhiteSpace(_jwtSettings.SecretKey)
                ? "HSis_Secret_Key_For_JWT_Authentication_Token_2026_SecureKey!"
                : _jwtSettings.SecretKey;
            var key = Encoding.UTF8.GetBytes(secretKeyString);

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, usuario.IdUsuario.ToString()),
                new(ClaimTypes.Name, usuario.Nombre ?? string.Empty)
            };

            if (usuario.IdRol.HasValue)
            {
                claims.Add(new Claim(ClaimTypes.Role, usuario.IdRol.Value.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}

