using Microsoft.AspNetCore.Identity;

namespace ConectaMasPro.Infrastructure.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
    }
}
