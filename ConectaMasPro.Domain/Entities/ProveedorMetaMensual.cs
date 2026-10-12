using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class ProveedorMetaMensual : BaseEntity
    {
        public Guid ProveedorProgresoId { get; set; }

        public ProveedorProgreso ProveedorProgreso { get; set; } = null!;

        public int Anio { get; set; }

        public int Mes { get; set; }

        public int MetaServicios { get; set; }

        public int ServiciosCompletados { get; set; }

        public bool MetaCumplida { get; set; }

        public DateTime? FechaCumplimiento { get; set; }
    }
}
