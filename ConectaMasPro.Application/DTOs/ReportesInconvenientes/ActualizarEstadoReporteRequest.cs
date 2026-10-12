using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.DTOs.ReportesInconvenientes
{
    public class ActualizarEstadoReporteRequest
    {
        public EstadoReporteInconveniente Estado { get; set; }
        public string? Resolucion { get; set; }
    }
}
