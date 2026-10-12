using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities;

public class PostulacionActividadSocial : BaseEntity
{
    public Guid ActividadSocialId { get; set; }
    public ActividadSocial ActividadSocial { get; set; } = null!;
    public Guid PostulanteId { get; set; }
    public Usuario Postulante { get; set; } = null!;
    public string? Mensaje { get; set; }
    public EstadoPostulacionActividad Estado { get; set; } = EstadoPostulacionActividad.Pendiente;
    public DateTime? FechaDecision { get; set; }
}
