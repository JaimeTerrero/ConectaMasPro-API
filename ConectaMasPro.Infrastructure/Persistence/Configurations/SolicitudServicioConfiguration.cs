using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class SolicitudServicioConfiguration : IEntityTypeConfiguration<SolicitudServicio>
    {
        public void Configure(EntityTypeBuilder<SolicitudServicio> builder)
        {
            builder.ToTable("SolicitudesServicio");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DescripcionSolicitud)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.PrecioAcordado)
                .HasPrecision(18, 2);

            builder.Property(x => x.Estado)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.MotivoRechazoFinalizacion)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Cliente)
                .WithMany(x => x.SolicitudesCreadas)
                .HasForeignKey(x => x.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Servicio)
                .WithMany(x => x.Solicitudes)
                .HasForeignKey(x => x.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Pago)
                .WithOne(x => x.SolicitudServicio)
                .HasForeignKey<Pago>(x => x.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
