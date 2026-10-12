namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class ProveedorDashboardDto
    {
        public ProveedorDashboardResumenDto Resumen { get; set; } = new();

        public IReadOnlyList<SolicitudRecibidaDto> SolicitudesRecibidas { get; set; } = [];

        public IReadOnlyList<ServicioProveedorDto> MisServiciosPublicados { get; set; } = [];
    }
}
