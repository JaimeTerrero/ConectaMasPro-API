namespace ConectaMasPro.Application.DTOs.Servicios
{
    public class ServicioDto
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public Guid CategoriaServicioId { get; set; }
        public string Titulo { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public decimal? PrecioBase { get; set; }
        public bool PrecioNegociable { get; set; }
    }
}
