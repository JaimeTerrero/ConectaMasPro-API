namespace ConectaMasPro.Application.DTOs.DocumentosLegales
{
    public class AceptacionDocumentoLegalDto
    {
        public Guid DocumentoLegalId { get; set; }
        public int TipoId { get; set; }
        public string Tipo { get; set; } = null!;
        public string Version { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public DateTime FechaAceptacion { get; set; }
    }
}
