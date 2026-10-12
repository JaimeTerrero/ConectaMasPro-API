namespace ConectaMasPro.Application.DTOs.Conversaciones
{
    public class EnviarMensajeRequest
    {
        public Guid ConversacionId { get; set; }
        public string Contenido { get; set; } = null!;
    }
}
