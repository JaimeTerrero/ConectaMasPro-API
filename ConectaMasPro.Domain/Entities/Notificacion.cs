using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Notificacion : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string Titulo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
        public bool Leida { get; set; }
    }
}
