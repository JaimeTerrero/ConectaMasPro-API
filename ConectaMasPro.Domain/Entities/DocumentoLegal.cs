using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class DocumentoLegal : BaseEntity
    {
        public TipoDocumentoLegal Tipo { get; set; }
        public string Version { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateTime FechaPublicacion { get; set; }
        public bool EstaVigente { get; set; }
        public bool RequiereAceptacion { get; set; } = true;
        public ICollection<AceptacionDocumentoLegal> Aceptaciones { get; set; } = [];
    }
}
