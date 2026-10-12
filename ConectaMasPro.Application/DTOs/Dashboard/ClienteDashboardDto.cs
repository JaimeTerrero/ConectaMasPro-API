namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class ClienteDashboardDto
    {
        public ClienteDashboardResumenDto Resumen { get; set; } = new();
        public IReadOnlyList<SolicitudRecienteDto> SolicitudesRecientes { get; set; } = [];
        public IReadOnlyList<ProveedorRecomendadoDto> ProveedoresRecomendados { get; set; } = [];
    }
}
