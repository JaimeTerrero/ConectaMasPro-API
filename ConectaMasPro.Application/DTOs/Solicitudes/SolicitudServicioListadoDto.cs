namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class SolicitudServicioListadoDto
    {
        public Guid Id { get; set; }
        public Guid ServicioId { get; set; }

        public string TituloServicio { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public Guid ProveedorId { get; set; }
        public string Proveedor { get; set; } = null!;
        public decimal CalificacionProveedor { get; set; }
        public int TotalResenasProveedor { get; set; }

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
