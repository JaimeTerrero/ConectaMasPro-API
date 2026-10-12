namespace ConectaMasPro.Application.DTOs.Auth
{
    public class AuthResponse
    {
        public bool RequiereDosFactores { get; set; }

        public string? TwoFactorToken { get; set; }

        public Guid? IdentityUserId { get; set; }

        public Guid? UsuarioId { get; set; }

        public string? Nombre { get; set; }

        public string? Email { get; set; }

        public string? Token { get; set; }

        public DateTime? ExpiraEn { get; set; }
    }
}
