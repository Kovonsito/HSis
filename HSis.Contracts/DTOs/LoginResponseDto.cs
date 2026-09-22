namespace HSis.Contracts.DTOs
{
    public class LoginResponseDto
    {
        public UsuarioDto Usuario { get; set; } = null!;
        public string Token { get; set; } = string.Empty;
    }
}

