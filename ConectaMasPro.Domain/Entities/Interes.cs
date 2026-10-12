using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Interes : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<UsuarioInteres> Usuarios { get; set; }
            = new List<UsuarioInteres>();
    }
}
