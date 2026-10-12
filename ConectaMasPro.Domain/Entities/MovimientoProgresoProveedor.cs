using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class MovimientoProgresoProveedor : BaseEntity
    {
        public Guid ProveedorProgresoId { get; set; }

        public ProveedorProgreso ProveedorProgreso { get; set; } = null!;

        public Guid SolicitudServicioId { get; set; }

        public SolicitudServicio SolicitudServicio { get; set; } = null!;

        public int ServiciosAnteriores { get; set; }

        public int ServiciosPosteriores { get; set; }

        public string Concepto { get; set; } = null!;
    }
}
