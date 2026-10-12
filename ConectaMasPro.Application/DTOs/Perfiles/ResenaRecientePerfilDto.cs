namespace ConectaMasPro.Application.DTOs.Perfiles
{
    public class ResenaRecientePerfilDto
    {
        public Guid Id { get; set; }
        public Guid EvaluadorId { get; set; }
        public string Evaluador { get; set; } = null!;
        public int CalificacionGeneral { get; set; }
        public string? Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
