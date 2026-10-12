namespace ConectaMasPro.Application.DTOs.Resenas
{
    public class CrearResenaRequest
    {
        public Guid SolicitudServicioId { get; set; }
        public int CalificacionGeneral { get; set; }
        public string? Comentario { get; set; }
    }
}
