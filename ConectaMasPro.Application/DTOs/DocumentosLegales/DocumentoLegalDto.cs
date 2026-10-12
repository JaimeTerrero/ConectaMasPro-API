namespace ConectaMasPro.Application.DTOs.DocumentosLegales
{
    public class DocumentoLegalDto
    {
        public Guid Id { get; set; }
        public int TipoId { get; set; }
        public string Tipo { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public string Contenido { get; set; } = null!;
        public DateTime FechaPublicacion { get; set; }
        public bool RequiereAceptacion { get; set; }
    }
}
