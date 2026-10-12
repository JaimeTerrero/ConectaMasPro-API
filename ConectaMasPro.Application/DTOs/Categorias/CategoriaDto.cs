namespace ConectaMasPro.Application.DTOs.Categorias
{
    public class CategoriaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
