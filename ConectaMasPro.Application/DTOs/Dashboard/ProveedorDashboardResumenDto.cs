namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class ProveedorDashboardResumenDto
    {
        public int SolicitudesRecibidas { get; set; }

        public int ServiciosActivos { get; set; }

        public decimal IngresosEstimados { get; set; }

        public decimal CalificacionPromedio { get; set; }
    }
}
