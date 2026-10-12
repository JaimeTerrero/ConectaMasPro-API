namespace ConectaMasPro.Application.DTOs.ReportesInconvenientes
{
    public class ActualizarEvidenciaReporteRequest
    {
        public byte[] Contenido { get; set; } = [];
        public string TipoContenido { get; set; } = null!;
    }
}
