using ConectaMasPro.Application.Common.Pagination;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.DTOs.ActividadesSociales;

public class CrearActividadSocialRequest
{
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public Guid CategoriaActividadSocialId { get; set; }
    public DateTime FechaActividad { get; set; }
    public string Ubicacion { get; set; } = null!;
    public int CantidadPersonasBuscadas { get; set; }
    public ModalidadActividadSocial Modalidad { get; set; }
    public decimal? MontoOfrecido { get; set; }
    public string? Requisitos { get; set; }
}

public class ActividadSocialFiltroRequest : PagedRequest
{
    public string? Busqueda { get; set; }
    public Guid? CategoriaActividadSocialId { get; set; }
    public ModalidadActividadSocial? Modalidad { get; set; }
}

public class CrearPostulacionActividadRequest
{
    public string? Mensaje { get; set; }
}

public class DecidirPostulacionRequest
{
    public EstadoPostulacionActividad Estado { get; set; }
}

public class CambiarEstadoActividadRequest
{
    public EstadoActividadSocial Estado { get; set; }
}
