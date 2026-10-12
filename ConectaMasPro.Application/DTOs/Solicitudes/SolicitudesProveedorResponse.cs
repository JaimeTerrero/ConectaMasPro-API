namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class SolicitudesProveedorResponse
    {
        public int TotalSolicitudes { get; set; }

        public int Pendientes { get; set; }

        public int Aceptadas { get; set; }

        public int Completadas { get; set; }

        public int Canceladas { get; set; }

        public IReadOnlyList<SolicitudProveedorListadoDto> Solicitudes { get; set; }
            = new List<SolicitudProveedorListadoDto>();
    }
}
