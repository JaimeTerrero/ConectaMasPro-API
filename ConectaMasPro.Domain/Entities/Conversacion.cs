using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Conversacion : BaseEntity
    {
        public Guid? SolicitudServicioId { get; set; }
        public SolicitudServicio? SolicitudServicio { get; set; }

        public Guid? ActividadSocialId { get; set; }
        public ActividadSocial? ActividadSocial { get; set; }

        public Guid UsuarioUnoId { get; set; }
        public Usuario UsuarioUno { get; set; } = null!;

        public Guid UsuarioDosId { get; set; }
        public Usuario UsuarioDos { get; set; } = null!;

        public ICollection<Mensaje> Mensajes { get; set; } = new List<Mensaje>();
    }
}
