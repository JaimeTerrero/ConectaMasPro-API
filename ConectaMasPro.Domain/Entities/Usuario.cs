using ConectaMasPro.Domain.Common;
using ConectaMasPro.Domain.Enums;

namespace ConectaMasPro.Domain.Entities
{
    public class Usuario : BaseEntity
    {
        public Guid IdentityUserId { get; set; }
        public string Nombre { get; set; } = null!;
        public string Apellido { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Telefono { get; set; }
        public TipoUsuario TipoUsuario { get; set; }
        public PerfilUsuario? Perfil { get; set; }
        public UbicacionUsuario? Ubicacion { get; set; }
        public UsuarioRecompensa? Recompensa { get; set; }
        public ProveedorProgreso? ProgresoProveedor { get; set; }
        public ICollection<Servicio> Servicios { get; set; } = new List<Servicio>();
        public ICollection<SolicitudServicio> SolicitudesCreadas { get; set; } = new List<SolicitudServicio>();
        public ICollection<Resena> ResenasRecibidas { get; set; } = new List<Resena>();
        public ICollection<ReporteInconveniente> ReportesCreados { get; set; } = new List<ReporteInconveniente>();
        public ICollection<ReporteInconveniente> ReportesRecibidos { get; set; } = new List<ReporteInconveniente>();
        public ICollection<UsuarioInteres> Intereses { get; set; } = new List<UsuarioInteres>();
        public ICollection<MovimientoRecompensa> MovimientosRecompensas { get; set; }  = new List<MovimientoRecompensa>();
        public ICollection<AceptacionDocumentoLegal> AceptacionesDocumentosLegales { get; set; } = [];
        public ICollection<ActividadSocial> ActividadesSocialesCreadas { get; set; } = [];
        public ICollection<PostulacionActividadSocial> PostulacionesActividades { get; set; } = [];
    }
}
