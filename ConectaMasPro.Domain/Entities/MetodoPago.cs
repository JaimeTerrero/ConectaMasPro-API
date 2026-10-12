using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class MetodoPago : BaseEntity
    {
        public string Nombre { get; set; } = null!;
        public TipoMetodoPago Tipo { get; set; }

        public ICollection<Pago> Pagos { get; set; } = new List<Pago>();
    }
}
