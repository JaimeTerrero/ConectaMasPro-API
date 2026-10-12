namespace ConectaMasPro.Application.DTOs.Resenas
{
    public class ResumenCalificacionesDto
    {
        public Guid UsuarioId { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
        public IReadOnlyList<ResenaDto> Resenas { get; set; } = [];
    }
}
