using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class MovimientoRecompensa : BaseEntity
    {
        public Guid UsuarioRecompensaId { get; set; }

        public UsuarioRecompensa UsuarioRecompensa { get; set; } = null!;

        public Guid UsuarioId { get; set; }

        public Usuario Usuario { get; set; } = null!;

        public Guid? SolicitudServicioId { get; set; }

        public SolicitudServicio? SolicitudServicio { get; set; }

        public TipoMovimientoRecompensa TipoMovimiento { get; set; }

        public int Puntos { get; set; }

        public int SaldoAnterior { get; set; }

        public int SaldoPosterior { get; set; }

        public string Concepto { get; set; } = null!;
    }
}
