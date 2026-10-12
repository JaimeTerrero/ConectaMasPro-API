using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class SolicitudServicioFiltroRequest : PagedRequest
    {
        public string? Busqueda { get; set; }

        public EstadoSolicitudServicio? Estado { get; set; }

        public string? OrdenarPor { get; set; }
    }
}
