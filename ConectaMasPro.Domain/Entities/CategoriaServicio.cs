using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class CategoriaServicio : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }

        public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
    }
}
