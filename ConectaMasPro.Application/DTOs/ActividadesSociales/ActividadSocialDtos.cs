using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Application.DTOs.ActividadesSociales;

public class ActividadSocialDto
{
    public Guid Id { get; set; }
    public Guid CreadorId { get; set; }
    public string CreadorNombre { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public Guid CategoriaActividadSocialId { get; set; }
    public string Categoria { get; set; } = null!;
    public DateTime FechaActividad { get; set; }
    public string Ubicacion { get; set; } = null!;
    public int CantidadPersonasBuscadas { get; set; }
    public int PersonasAceptadas { get; set; }
    public int CuposDisponibles { get; set; }
    public ModalidadActividadSocial Modalidad { get; set; }
    public decimal? MontoOfrecido { get; set; }
    public string? Requisitos { get; set; }
    public EstadoActividadSocial Estado { get; set; }
    public EstadoPostulacionActividad? MiPostulacion { get; set; }
    public DateTime FechaCreacion { get; set; }
}

public class PostulacionActividadDto
{
    public Guid Id { get; set; }
    public Guid ActividadSocialId { get; set; }
    public string ActividadTitulo { get; set; } = null!;
    public Guid PostulanteId { get; set; }
    public string PostulanteNombre { get; set; } = null!;
    public string? Mensaje { get; set; }
    public EstadoPostulacionActividad Estado { get; set; }
    public DateTime FechaCreacion { get; set; }
    public DateTime? FechaDecision { get; set; }
}

public class DecisionPostulacionDto
{
    public Guid PostulacionId { get; set; }
    public EstadoPostulacionActividad Estado { get; set; }
    public Guid PostulanteId { get; set; }
    public Guid? ConversacionId { get; set; }
}
