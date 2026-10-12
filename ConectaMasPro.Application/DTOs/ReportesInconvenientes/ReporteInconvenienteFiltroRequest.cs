using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.DTOs.ReportesInconvenientes
{
    public class ReporteInconvenienteFiltroRequest : PagedRequest
    {
        public TipoReporteInconveniente? Tipo { get; set; }
        public EstadoReporteInconveniente? Estado { get; set; }
        public string? Busqueda { get; set; }
    }
}
