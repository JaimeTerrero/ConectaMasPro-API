using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class Pago : BaseEntity
    {
        public Guid SolicitudServicioId { get; set; }
        public SolicitudServicio SolicitudServicio { get; set; } = null!;

        public Guid? MetodoPagoId { get; set; }
        public MetodoPago? MetodoPago { get; set; }

        public decimal MontoServicio { get; set; }
        public decimal Monto { get; set; }
        public decimal Comision { get; set; }
        public decimal PorcentajeComision { get; set; }
        public string Moneda { get; set; } = "dop";
        public EstadoPago Estado { get; set; } = EstadoPago.Pendiente;

        public string? ReferenciaTransaccion { get; set; }
        public string? EstadoPasarela { get; set; }
        public string? UltimoError { get; set; }
        public DateTime? FechaPago { get; set; }
    }
}
