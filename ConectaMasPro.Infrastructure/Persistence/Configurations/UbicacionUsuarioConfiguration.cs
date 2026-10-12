using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class UbicacionUsuarioConfiguration
    : IEntityTypeConfiguration<UbicacionUsuario>
    {
        public void Configure(EntityTypeBuilder<UbicacionUsuario> builder)
        {
            builder.ToTable("UbicacionesUsuario");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.UsuarioId)
                .IsRequired();

            builder.Property(x => x.ProvinciaId)
                .IsRequired();

            builder.Property(x => x.MunicipioId)
                .IsRequired();

            builder.Property(x => x.Latitud)
                .HasPrecision(10, 7);

            builder.Property(x => x.Longitud)
                .HasPrecision(10, 7);

            builder.HasOne(x => x.Usuario)
                .WithOne(x => x.Ubicacion)
                .HasForeignKey<UbicacionUsuario>(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Provincia)
                .WithMany()
                .HasForeignKey(x => x.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Municipio)
                .WithMany()
                .HasForeignKey(x => x.MunicipioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Sector)
                .WithMany()
                .HasForeignKey(x => x.SectorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UsuarioId)
                .IsUnique();
        }
    }
}
