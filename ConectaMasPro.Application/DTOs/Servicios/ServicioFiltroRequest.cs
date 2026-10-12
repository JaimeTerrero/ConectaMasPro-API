using ConectaMasPro.Application.Common.Pagination;

namespace ConectaMasPro.Application.DTOs.Servicios
{
    public class ServicioFiltroRequest : PagedRequest
    {
        public string? Busqueda { get; set; }
        public string? Categoria { get; set; }
        public IReadOnlyCollection<Guid>? CategoriaServicioIds { get; set; }
        public decimal? PrecioMaximo { get; set; }
        public decimal? CalificacionMinima { get; set; }
        public Guid? ProvinciaId { get; set; }
        public string? OrdenarPor { get; set; }
    }

}
