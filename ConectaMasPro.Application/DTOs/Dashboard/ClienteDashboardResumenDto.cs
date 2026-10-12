namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class ClienteDashboardResumenDto
    {
        public int SolicitudesActivas { get; set; }
        public int ServiciosCompletados { get; set; }
        public decimal PagosRealizados { get; set; }
        public decimal CalificacionPromedio { get; set; }
    }
}
