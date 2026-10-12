using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Mensaje : BaseEntity
    {
        public Guid ConversacionId { get; set; }
        public Conversacion Conversacion { get; set; } = null!;

        public Guid EmisorId { get; set; }
        public Usuario Emisor { get; set; } = null!;

        public string Contenido { get; set; } = null!;
        public bool Leido { get; set; }
    }
}
