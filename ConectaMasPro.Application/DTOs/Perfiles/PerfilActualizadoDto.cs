namespace ConectaMasPro.Application.DTOs.Perfiles
{
    public class PerfilActualizadoDto
    {
        public Guid UsuarioId { get; set; }

        public string NombreCompleto { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string? Telefono { get; set; }

        public Guid ProvinciaId { get; set; }

        public string Provincia { get; set; } = null!;

        public Guid MunicipioId { get; set; }

        public string Municipio { get; set; } = null!;

        public Guid? SectorId { get; set; }

        public string? Sector { get; set; }

        public string UbicacionCompleta { get; set; } = null!;

        public UbicacionMapaDto? UbicacionMapa { get; set; }

        public string? Biografia { get; set; }

        public string? FotoUrl { get; set; }

        public IReadOnlyList<InteresPerfilDto> Intereses { get; set; } = [];
    }
}
