using ConectaMasPro.Application.DTOs.Usuarios;

namespace ConectaMasPro.Application.DTOs.Auth
{
    public class LoginResponse
    {
        public bool RequiereDosFactores { get; set; }

        public string? TwoFactorToken { get; set; }

        public string? Token { get; set; }

        public DateTime? Expiracion { get; set; }

        public UsuarioDto? Usuario { get; set; }
    }
}
