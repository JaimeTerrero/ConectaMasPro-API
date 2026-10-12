using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.DTOs.Auth
{
    public class RegisterRequest
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public string Password { get; set; } = null!;
        public int TipoUsuarioId { get; set; }
        public Guid TerminosCondicionesId { get; set; }
        public Guid PoliticaPrivacidadId { get; set; }
        public bool AceptaTerminosCondiciones { get; set; }
        public bool AceptaPoliticaPrivacidad { get; set; }
        public bool ConfirmaMayoriaEdad { get; set; }
    }
}
