namespace ConectaMasPro.Application.DTOs.ReportesInconvenientes
{
    public class ReporteInconvenienteDto
    {
        public Guid Id { get; set; }
        public Guid ReportanteId { get; set; }
        public string Reportante { get; set; } = null!;
        public Guid? ReportadoId { get; set; }
        public string? Reportado { get; set; }
        public Guid? SolicitudServicioId { get; set; }
        public string? Servicio { get; set; }
        public int TipoId { get; set; }
        public string Tipo { get; set; } = null!;
        public int EstadoId { get; set; }
        public string Estado { get; set; } = null!;
        public string Asunto { get; set; } = null!;
        public string Descripcion { get; set; } = null!;
        public string? EvidenciaUrl { get; set; }
        public string? Resolucion { get; set; }
        public DateTime FechaCreacion { get; set; }
        public DateTime? FechaResolucion { get; set; }
    }
}
