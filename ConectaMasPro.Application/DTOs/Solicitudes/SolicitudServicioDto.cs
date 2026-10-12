namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class SolicitudServicioDto
    {
        public Guid Id { get; set; }
        public Guid ClienteId { get; set; }
        public Guid ServicioId { get; set; }
        public string DescripcionSolicitud { get; set; } = null!;
        public DateTime? FechaDeseada { get; set; }
        public decimal? PrecioAcordado { get; set; }
        public string Estado { get; set; } = null!;
    }
}
