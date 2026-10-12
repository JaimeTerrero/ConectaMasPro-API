using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class UsuarioInteres : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public Guid InteresId { get; set; }
        public Interes Interes { get; set; } = null!;
    }
}
