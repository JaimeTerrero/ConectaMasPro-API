using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Provincia : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string? Codigo { get; set; }

        public ICollection<Municipio> Municipios { get; set; }
            = new List<Municipio>();
    }
}
