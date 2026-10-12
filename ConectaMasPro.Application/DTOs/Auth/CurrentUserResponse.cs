namespace ConectaMasPro.Application.DTOs.Auth
{
    public class CurrentUserResponse
    {
        public Guid IdentityUserId { get; set; }
        public Guid UsuarioId { get; set; }

        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string TipoUsuario { get; set; } = null!;
    }
}
