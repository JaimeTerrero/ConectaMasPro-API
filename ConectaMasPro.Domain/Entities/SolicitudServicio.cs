using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class SolicitudServicio : BaseEntity
    {
        public Guid ClienteId { get; set; }
        public Usuario Cliente { get; set; } = null!;
        public Guid ServicioId { get; set; }
        public Servicio Servicio { get; set; } = null!;
        public string DescripcionSolicitud { get; set; } = null!;
        public DateTime? FechaDeseada { get; set; }
        public decimal? PrecioAcordado { get; set; }
        public EstadoSolicitudServicio Estado { get; set; } = EstadoSolicitudServicio.Pendiente;
        public DateTime? FechaReporteFinalizacionProveedor { get; set; }
        public DateTime? FechaConfirmacionCliente { get; set; }
        public DateTime? FechaRechazoFinalizacionCliente { get; set; }
        public string? MotivoRechazoFinalizacion { get; set; }
        public Pago? Pago { get; set; }
        public MovimientoRecompensa? MovimientoRecompensa { get; set; }
        public MovimientoProgresoProveedor? MovimientoProgresoProveedor { get; set; }
        public Conversacion? Conversacion { get; set; }
        public ICollection<Resena> Resenas { get; set; } = new List<Resena>();
        public ICollection<ReporteInconveniente> ReportesInconvenientes { get; set; } = new List<ReporteInconveniente>();
    }
}
