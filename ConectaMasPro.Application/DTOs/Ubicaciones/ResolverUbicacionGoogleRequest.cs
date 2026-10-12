namespace ConectaMasPro.Application.DTOs.Ubicaciones
{
    public class ResolverUbicacionGoogleRequest
    {
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }

        public string? Provincia { get; set; }
        public string? Municipio { get; set; }
        public string? Sector { get; set; }
    }
}
