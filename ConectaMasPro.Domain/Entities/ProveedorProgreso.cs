using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class ProveedorProgreso : BaseEntity
    {
        public Guid UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public int ServiciosCompletados { get; set; }

        public NivelProveedor Nivel { get; set; }

        public ICollection<ProveedorMetaMensual> MetasMensuales { get; set; }
            = new List<ProveedorMetaMensual>();

        public ICollection<MovimientoProgresoProveedor> Movimientos { get; set; }
            = new List<MovimientoProgresoProveedor>();
    }
}
