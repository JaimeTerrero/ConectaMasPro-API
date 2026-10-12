namespace ConectaMasPro.Application.DTOs.Resenas
{
    public class EstadoCalificacionSolicitudDto
    {
        public Guid SolicitudServicioId { get; set; }
        public bool SolicitudCompletada { get; set; }
        public bool PuedeCalificar { get; set; }
        public bool YaCalifico { get; set; }
        public Guid UsuarioACalificarId { get; set; }
        public string UsuarioACalificar { get; set; } = null!;
        public decimal CalificacionPromedioUsuario { get; set; }
        public int TotalResenasUsuario { get; set; }
        public ResenaDto? MiResena { get; set; }
    }
}
