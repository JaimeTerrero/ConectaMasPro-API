using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class VerificacionIdentidad : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string DocumentoUrl { get; set; } = null!;
        public bool Aprobada { get; set; }
        public DateTime? FechaRevision { get; set; }
        public string? ComentarioRevision { get; set; }
    }
}
