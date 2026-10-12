namespace ConectaMasPro.Application.DTOs.Usuarios
{
    public class CrearUsuarioRequest
    {
        public Guid IdentityUserId { get; set; }

        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public string PasswordHash { get; set; } = null!;
    }
}
