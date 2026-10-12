using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Municipio : BaseEntity
    {
        public Guid ProvinciaId { get; set; }
        public Provincia Provincia { get; set; } = null!;

        public string Nombre { get; set; } = null!;
        public string? Codigo { get; set; }

        public ICollection<Sector> Sectores { get; set; }
            = new List<Sector>();
    }
}
