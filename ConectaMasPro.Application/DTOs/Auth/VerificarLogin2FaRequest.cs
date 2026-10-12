namespace ConectaMasPro.Application.DTOs.Auth
{
    public class VerificarLogin2FaRequest
    {
        public string TwoFactorToken { get; set; } = null!;
        public string Codigo { get; set; } = null!;
    }
}
