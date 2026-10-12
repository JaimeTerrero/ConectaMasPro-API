namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class SolicitudRecienteDto
    {
        public Guid Id { get; set; }
        public string TituloServicio { get; set; } = null!;
        public string Proveedor { get; set; } = null!;
        public DateTime FechaCreacion { get; set; }
        public int EstadoId { get; set; }
        public string Estado { get; set; } = null!;
    }
}
