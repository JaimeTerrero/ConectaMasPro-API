namespace ConectaMasPro.Application.DTOs.Perfiles
{
    public class PerfilPublicoDto
    {
        public Guid UsuarioId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public int TipoUsuarioId { get; set; }
        public string TipoUsuario { get; set; } = null!;
        public string? FotoUrl { get; set; }
        public bool IdentidadVerificada { get; set; }
        public string? UbicacionCompleta { get; set; }
        public string? Biografia { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
        public int ServiciosCompletados { get; set; }
        public DateTime MiembroDesde { get; set; }
        public IReadOnlyList<ResenaRecientePerfilDto> ResenasRecientes { get; set; } = [];
    }
}
