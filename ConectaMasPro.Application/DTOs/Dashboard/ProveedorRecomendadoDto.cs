namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class ProveedorRecomendadoDto
    {
        public Guid ProveedorId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Categoria { get; set; }
        public decimal CalificacionPromedio { get; set; }
    }
}
