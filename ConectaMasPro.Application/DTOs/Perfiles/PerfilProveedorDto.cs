namespace ConectaMasPro.Application.DTOs.Perfiles
{
    public class PerfilProveedorDto
    {
        public Guid UsuarioId { get; set; }
        public string NombreCompleto { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public int TipoUsuarioId { get; set; }
        public string TipoUsuario { get; set; } = null!;
        public string? FotoUrl { get; set; }
        public bool IdentidadVerificada { get; set; }
        public string? Provincia { get; set; }
        public Guid? ProvinciaId { get; set; }
        public string? Municipio { get; set; }
        public Guid? MunicipioId { get; set; }
        public string? Sector { get; set; }
        public Guid? SectorId { get; set; }
        public string? UbicacionCompleta { get; set; }
        public UbicacionMapaDto? UbicacionMapa { get; set; }
        public string? Biografia { get; set; }
        public int ServiciosActivos { get; set; }
        public int SolicitudesRecibidas { get; set; }
        public int ServiciosCompletados { get; set; }
        public decimal CalificacionPromedio { get; set; }
        public int TotalResenas { get; set; }
        public IReadOnlyList<ResenaRecientePerfilDto> ResenasRecientes { get; set; } = [];
        public DateTime MiembroDesde { get; set; }
        public bool AutenticacionDosFactoresHabilitada { get; set; }
        public IReadOnlyList<InteresPerfilDto> Intereses { get; set; } = [];
    }
}
