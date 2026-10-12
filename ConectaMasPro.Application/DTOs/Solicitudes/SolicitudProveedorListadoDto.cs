namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class SolicitudProveedorListadoDto
    {
        public Guid Id { get; set; }

        public Guid ServicioId { get; set; }

        public string TituloServicio { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public Guid ClienteId { get; set; }

        public string Cliente { get; set; } = null!;

        public decimal CalificacionCliente { get; set; }

        public int TotalResenasCliente { get; set; }

        public string DescripcionSolicitud { get; set; } = null!;

        public DateTime? FechaDeseada { get; set; }

        public decimal? PrecioAcordado { get; set; }

        public int EstadoId { get; set; }

        public string Estado { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public int AvancePorcentaje { get; set; }

        public bool PuedeCalificar { get; set; }

        public bool YaCalifico { get; set; }
    }
}
