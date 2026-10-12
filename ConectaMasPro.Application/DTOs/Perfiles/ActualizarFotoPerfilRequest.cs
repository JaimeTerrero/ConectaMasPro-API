namespace ConectaMasPro.Application.DTOs.Perfiles
{
    public class ActualizarFotoPerfilRequest
    {
        public byte[] Contenido { get; set; } = [];
        public string TipoContenido { get; set; } = null!;
        public string? NombreArchivo { get; set; }
    }
}
