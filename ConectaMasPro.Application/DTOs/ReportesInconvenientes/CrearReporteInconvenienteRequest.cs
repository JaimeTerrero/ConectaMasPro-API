using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.DTOs.ReportesInconvenientes
{
    public class CrearReporteInconvenienteRequest
    {
        public TipoReporteInconveniente Tipo { get; set; }
        public Guid? SolicitudServicioId { get; set; }
        public string Asunto { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
    }
}
