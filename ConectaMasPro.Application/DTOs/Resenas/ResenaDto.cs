namespace ConectaMasPro.Application.DTOs.Resenas
{
    public class ResenaDto
    {
        public Guid Id { get; set; }
        public Guid SolicitudServicioId { get; set; }
        public Guid EvaluadorId { get; set; }
        public string Evaluador { get; set; } = null!;
        public Guid EvaluadoId { get; set; }
        public int CalificacionGeneral { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
