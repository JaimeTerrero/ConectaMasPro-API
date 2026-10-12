using ConectaMasPro.Domain.Common;

namespace ConectaMasPro.Domain.Entities
{
    public class UsuarioRecompensa : BaseEntity
    {
        public Guid UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public int PuntosActuales { get; set; }

        public int PuntosHistoricos { get; set; }

        public ICollection<MovimientoRecompensa> Movimientos { get; set; }
            = new List<MovimientoRecompensa>();
    }
}
