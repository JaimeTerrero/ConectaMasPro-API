namespace ConectaMasPro.Application.DTOs.Conversaciones;

public class MensajeDto
{
    public Guid Id { get; set; }
    public Guid ConversacionId { get; set; }
    public Guid EmisorId { get; set; }
    public string EmisorNombre { get; set; } = null!;
    public string Contenido { get; set; } = null!;
    public bool Leido { get; set; }
    public DateTime FechaCreacion { get; set; }
}
