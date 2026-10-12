namespace ConectaMasPro.Application.DTOs.Perfiles
{
    public class ActualizarPerfilRequest
    {
        public string? Telefono { get; set; }

        public Guid ProvinciaId { get; set; }
        public Guid MunicipioId { get; set; }
        public Guid? SectorId { get; set; }

        public decimal? Latitud { get; set; }
        public decimal? Longitud { get; set; }
        public bool UbicacionExacta { get; set; }

        public string? Biografia { get; set; }

        public IReadOnlyList<Guid> InteresesIds { get; set; } = [];
    }
}
