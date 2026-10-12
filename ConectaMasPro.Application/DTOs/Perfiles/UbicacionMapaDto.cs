namespace ConectaMasPro.Application.DTOs.Perfiles
{
    public class UbicacionMapaDto
    {
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public string Precision { get; set; } = null!;
        public bool EsExacta { get; set; }
    }
}
