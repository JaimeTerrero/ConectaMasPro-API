namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class SolicitudServicioDetalleDto
    {
        public Guid Id { get; set; }

        public Guid ClienteId { get; set; }
        public string Cliente { get; set; } = null!;
        public decimal CalificacionCliente { get; set; }
        public int TotalResenasCliente { get; set; }

        public Guid ServicioId { get; set; }
        public string TituloServicio { get; set; } = null!;
        public string DescripcionServicio { get; set; } = null!;
        public string Categoria { get; set; } = null!;

        public Guid ProveedorId { get; set; }
        public string Proveedor { get; set; } = null!;
        public decimal CalificacionProveedor { get; set; }
        public int TotalResenasProveedor { get; set; }

        public string DescripcionSolicitud { get; set; } = null!;
        public DateTime? FechaDeseada { get; set; }
        public decimal? PrecioAcordado { get; set; }
        public DateTime? FechaReporteFinalizacionProveedor { get; set; }
        public DateTime? FechaConfirmacionCliente { get; set; }
        public DateTime? FechaRechazoFinalizacionCliente { get; set; }
        public string? MotivoRechazoFinalizacion { get; set; }

        public int EstadoId { get; set; }
        public string Estado { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaModificacion { get; set; }

        public int AvancePorcentaje { get; set; }
        public bool PuedeCalificar { get; set; }
        public bool YaCalifico { get; set; }
        public bool PuedeConfirmarFinalizacion { get; set; }
        public bool PuedePagar { get; set; }
    }
}
