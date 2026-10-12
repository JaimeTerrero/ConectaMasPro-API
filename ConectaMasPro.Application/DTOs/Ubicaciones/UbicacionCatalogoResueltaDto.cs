using ConectaMasPro.Application.DTOs.Catalogos;

namespace ConectaMasPro.Application.DTOs.Ubicaciones
{
    public class UbicacionCatalogoResueltaDto
    {
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }

        public Guid? ProvinciaId { get; set; }
        public string? Provincia { get; set; }

        public Guid? MunicipioId { get; set; }
        public string? Municipio { get; set; }

        public Guid? SectorId { get; set; }
        public string? Sector { get; set; }

        public bool RequiereConfirmacion { get; set; }
        public string? MotivoConfirmacion { get; set; }

        public IReadOnlyList<CatalogoDto> ProvinciasSugeridas { get; set; } = [];
        public IReadOnlyList<CatalogoDto> MunicipiosSugeridos { get; set; } = [];
        public IReadOnlyList<SectorCatalogoDto> SectoresSugeridos { get; set; } = [];
    }
}
