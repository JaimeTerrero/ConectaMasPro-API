using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class MovimientoProgresoProveedorConfiguration
        : IEntityTypeConfiguration<MovimientoProgresoProveedor>
    {
        public void Configure(
            EntityTypeBuilder<MovimientoProgresoProveedor> builder)
        {
            builder.ToTable(
                "MovimientosProgresoProveedor",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_MovimientosProgresoProveedor_Servicios",
                        "[ServiciosAnteriores] >= 0 AND [ServiciosPosteriores] >= 0");
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ServiciosAnteriores)
                .IsRequired();

            builder.Property(x => x.ServiciosPosteriores)
                .IsRequired();

            builder.Property(x => x.Concepto)
                .HasMaxLength(250)
                .IsRequired();

            builder.HasOne(x => x.ProveedorProgreso)
                .WithMany(x => x.Movimientos)
                .HasForeignKey(x => x.ProveedorProgresoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SolicitudServicio)
                .WithOne(x => x.MovimientoProgresoProveedor)
                .HasForeignKey<MovimientoProgresoProveedor>(
                    x => x.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Una solicitud solamente puede incrementar
             * una vez el progreso del proveedor.
             */
            builder.HasIndex(x => x.SolicitudServicioId)
                .IsUnique();
        }
    }
}
