using ConectaMasPro.Domain.Entities;
using ConectaMasPro.Infrastructure.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence
{
    public class AppDbContext : IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Usuario> Usuarios => Set<Usuario>();
        public DbSet<PerfilUsuario> PerfilesUsuario => Set<PerfilUsuario>();
        public DbSet<UbicacionUsuario> UbicacionesUsuario => Set<UbicacionUsuario>();
        public DbSet<CategoriaServicio> CategoriasServicio => Set<CategoriaServicio>();
        public DbSet<Servicio> Servicios => Set<Servicio>();
        public DbSet<SolicitudServicio> SolicitudesServicio => Set<SolicitudServicio>();
        public DbSet<MetodoPago> MetodosPago => Set<MetodoPago>();
        public DbSet<Pago> Pagos => Set<Pago>();
        public DbSet<Resena> Resenas => Set<Resena>();
        public DbSet<CriterioCalificacion> CriteriosCalificacion => Set<CriterioCalificacion>();
        public DbSet<Conversacion> Conversaciones => Set<Conversacion>();
        public DbSet<Mensaje> Mensajes => Set<Mensaje>();
        public DbSet<VerificacionIdentidad> VerificacionesIdentidad => Set<VerificacionIdentidad>();
        public DbSet<Notificacion> Notificaciones => Set<Notificacion>();
        public DbSet<Provincia> Provincias => Set<Provincia>();
        public DbSet<Municipio> Municipios => Set<Municipio>();
        public DbSet<Sector> Sectores => Set<Sector>();
        public DbSet<Interes> Intereses => Set<Interes>();
        public DbSet<UsuarioInteres> UsuariosIntereses => Set<UsuarioInteres>();
        public DbSet<UsuarioRecompensa> UsuariosRecompensas => Set<UsuarioRecompensa>();
        public DbSet<MovimientoRecompensa> MovimientosRecompensas => Set<MovimientoRecompensa>();
        public DbSet<ProveedorProgreso> ProveedoresProgresos => Set<ProveedorProgreso>();
        public DbSet<ProveedorMetaMensual> ProveedoresMetasMensuales => Set<ProveedorMetaMensual>();
        public DbSet<MovimientoProgresoProveedor> MovimientosProgresoProveedor => Set<MovimientoProgresoProveedor>();
        public DbSet<ReporteInconveniente> ReportesInconvenientes => Set<ReporteInconveniente>();
        public DbSet<DocumentoLegal> DocumentosLegales => Set<DocumentoLegal>();
        public DbSet<AceptacionDocumentoLegal> AceptacionesDocumentosLegales => Set<AceptacionDocumentoLegal>();
        public DbSet<ActividadSocial> ActividadesSociales => Set<ActividadSocial>();
        public DbSet<PostulacionActividadSocial> PostulacionesActividadesSociales => Set<PostulacionActividadSocial>();
        public DbSet<CategoriaActividadSocial> CategoriasActividadesSociales => Set<CategoriaActividadSocial>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        }
    }
}
