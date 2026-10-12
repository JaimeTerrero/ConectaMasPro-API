using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class Resena : BaseEntity
    {
        public Guid EvaluadorId { get; set; }
        public Usuario Evaluador { get; set; } = null!;

        public Guid EvaluadoId { get; set; }
        public Usuario Evaluado { get; set; } = null!;

        public Guid SolicitudServicioId { get; set; }
        public SolicitudServicio SolicitudServicio { get; set; } = null!;

        public int CalificacionGeneral { get; set; }
        public string? Comentario { get; set; }

        public ICollection<CriterioCalificacion> Criterios { get; set; } = new List<CriterioCalificacion>();
    }
}
