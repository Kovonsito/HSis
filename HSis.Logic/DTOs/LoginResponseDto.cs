namespace HSis.Logic.DTOs
{
    public class LoginResponseDto
    {
        public UsuarioDto Usuario { get; set; } = null!;
        public string Token { get; set; } = string.Empty;
    }
}
