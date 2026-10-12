using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class UbicacionUsuario : BaseEntity
    {
        public Guid UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;

        public Guid ProvinciaId { get; set; }
        public Provincia Provincia { get; set; } = null!;

        public Guid MunicipioId { get; set; }
        public Municipio Municipio { get; set; } = null!;

        public Guid? SectorId { get; set; }
        public Sector? Sector { get; set; }

        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public bool UbicacionExacta { get; set; }
    }
}
