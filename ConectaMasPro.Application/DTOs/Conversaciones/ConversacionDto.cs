namespace ConectaMasPro.Application.DTOs.Conversaciones;

public class ConversacionDto
{
    public Guid Id { get; set; }
    public Guid? SolicitudServicioId { get; set; }
    public Guid? ActividadSocialId { get; set; }
    public string Tipo { get; set; } = null!;
    public Guid OtroUsuarioId { get; set; }
    public string OtroUsuarioNombre { get; set; } = null!;
    public string? ServicioTitulo { get; set; }
    public string? ActividadSocialTitulo { get; set; }
    public MensajeDto? UltimoMensaje { get; set; }
    public int MensajesNoLeidos { get; set; }
}
