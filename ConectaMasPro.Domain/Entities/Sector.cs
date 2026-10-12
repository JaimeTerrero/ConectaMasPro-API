using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Sector : BaseEntity
    {
        public Guid MunicipioId { get; set; }
        public Municipio Municipio { get; set; } = null!;

        public string Nombre { get; set; } = null!;
        public string? Codigo { get; set; }

        public decimal? LatitudCentro { get; set; }
        public decimal? LongitudCentro { get; set; }
        public string? GooglePlaceId { get; set; }
    }
}
