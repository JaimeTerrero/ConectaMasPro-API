using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities;

public class ActividadSocial : BaseEntity
{
    public Guid CreadorId { get; set; }
    public Usuario Creador { get; set; } = null!;
    public string Titulo { get; set; } = null!;
    public string Descripcion { get; set; } = null!;
    public Guid CategoriaActividadSocialId { get; set; }
    public CategoriaActividadSocial CategoriaActividadSocial { get; set; } = null!;
    public DateTime FechaActividad { get; set; }
    public string Ubicacion { get; set; } = null!;
    public int CantidadPersonasBuscadas { get; set; }
    public ModalidadActividadSocial Modalidad { get; set; }
    public decimal? MontoOfrecido { get; set; }
    public string? Requisitos { get; set; }
    public EstadoActividadSocial Estado { get; set; } = EstadoActividadSocial.Abierta;
    public ICollection<PostulacionActividadSocial> Postulaciones { get; set; } = [];
    public ICollection<Conversacion> Conversaciones { get; set; } = [];
}
