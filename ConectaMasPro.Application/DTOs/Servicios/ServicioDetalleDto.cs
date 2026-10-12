namespace ConectaMasPro.Application.DTOs.Servicios
{
    public class ServicioDetalleDto
    {
        public Guid Id { get; set; }

        public string TituloServicio { get; set; } = null!;
        public string Descripcion { get; set; } = null!;

        public Guid CategoriaId { get; set; }
        public string Categoria { get; set; } = null!;

        public Guid ProveedorId { get; set; }
        public string Proveedor { get; set; } = null!;
        public decimal CalificacionProveedor { get; set; }
        public int TotalResenasProveedor { get; set; }
        public Guid? ProvinciaId { get; set; }
        public string? Provincia { get; set; }
        public string UbicacionProveedor { get; set; } = "Ubicación no especificada";

        public decimal? Precio { get; set; }
        public bool PrecioNegociable { get; set; }

        public DateTime FechaCreacion { get; set; }
    }
}
