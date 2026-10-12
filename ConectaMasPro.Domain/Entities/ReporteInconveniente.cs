using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class ReporteInconveniente : BaseEntity
    {
        public Guid ReportanteId { get; set; }
        public Usuario Reportante { get; set; } = null!;
        public Guid? ReportadoId { get; set; }
        public Usuario? Reportado { get; set; }
        public Guid? SolicitudServicioId { get; set; }
        public SolicitudServicio? SolicitudServicio { get; set; }
        public TipoReporteInconveniente Tipo { get; set; }
        public EstadoReporteInconveniente Estado { get; set; } = EstadoReporteInconveniente.Abierto;
        public string Asunto { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string? EvidenciaUrl { get; set; }
        public string? Resolucion { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}
