namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class SolicitudRecibidaDto
    {
        public Guid Id { get; set; }

        public string TituloServicio { get; set; } = null!;

        public string Cliente { get; set; } = null!;

        public DateTime FechaCreacion { get; set; }

        public DateTime? FechaDeseada { get; set; }

        public decimal? PrecioAcordado { get; set; }

        public int EstadoId { get; set; }

        public string Estado { get; set; } = null!;
    }
}
