namespace ConectaMasPro.Application.DTOs.Dashboard
{
    public class ServicioProveedorDto
    {
        public Guid Id { get; set; }

        public string TituloServicio { get; set; } = null!;

        public string Categoria { get; set; } = null!;

        public decimal? PrecioBase { get; set; }

        public bool PrecioNegociable { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
