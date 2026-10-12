using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities;

public class CategoriaActividadSocial : BaseEntity
{
    public string Nombre { get; set; } = null!;
    public int Orden { get; set; }
    public ICollection<ActividadSocial> Actividades { get; set; } = [];
}
