using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class CriterioCalificacion : BaseEntity
    {
        public Guid ResenaId { get; set; }
        public Resena Resena { get; set; } = null!;

        public string Nombre { get; set; } = null!;
        public int Puntuacion { get; set; }
    }
}
