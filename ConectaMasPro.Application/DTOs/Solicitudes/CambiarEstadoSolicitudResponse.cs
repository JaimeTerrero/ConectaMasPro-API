namespace ConectaMasPro.Application.DTOs.Solicitudes;

public class CambiarEstadoSolicitudResponse
{
    public Guid SolicitudId { get; set; }
    public Guid ClienteId { get; set; }
    public int EstadoId { get; set; }
    public string Estado { get; set; } = null!;
    public Guid? ConversacionId { get; set; }
    public string? MensajeAutomatico { get; set; }
}
