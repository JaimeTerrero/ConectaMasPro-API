using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class AceptacionDocumentoLegal : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public Guid DocumentoLegalId { get; set; }
        public DocumentoLegal DocumentoLegal { get; set; } = null!;
        public DateTime FechaAceptacion { get; set; }
        public string? DireccionIp { get; set; }
        public string? AgenteUsuario { get; set; }
    }
}
