using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class PerfilUsuario : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public string? Biografia { get; set; }
        public string? FotoUrl { get; set; }
        public string? Ocupacion { get; set; }
        public string? Experiencia { get; set; }
        public bool IdentidadVerificada { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
    }
}
