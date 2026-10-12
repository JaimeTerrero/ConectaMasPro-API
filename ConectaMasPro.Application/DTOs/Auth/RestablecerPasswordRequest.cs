namespace ConectaMasPro.Application.DTOs.Auth
{
    public class RestablecerPasswordRequest
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string NuevaPassword { get; set; } = null!;
    }
}
