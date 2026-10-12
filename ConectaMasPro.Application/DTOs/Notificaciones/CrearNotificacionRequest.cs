namespace ConectaMasPro.Application.DTOs.Notificaciones
{
    public class CrearNotificacionRequest
    {
        public Guid UsuarioId { get; set; }
        public string Titulo { get; set; } = null!;
        public string Mensaje { get; set; } = null!;
    }
}
