namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class CrearSolicitudServicioRequest
    {
        public Guid ServicioId { get; set; }
        public string DescripcionSolicitud { get; set; } = null!;
        public DateTime? FechaDeseada { get; set; }
    }
}
