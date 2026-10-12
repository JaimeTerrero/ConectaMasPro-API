namespace ConectaMasPro.Application.DTOs.Solicitudes
{
    public class SolicitudesClienteResponse
    {
        public int TotalSolicitudes { get; set; }
        public int Pendientes { get; set; }
        public int Activas { get; set; }
        public int Completadas { get; set; }
        public int Canceladas { get; set; }

        public IReadOnlyList<SolicitudServicioListadoDto> Solicitudes { get; set; }
            = new List<SolicitudServicioListadoDto>();
    }
}
