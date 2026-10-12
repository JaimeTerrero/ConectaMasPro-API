using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class SectorConfiguration : IEntityTypeConfiguration<Sector>
    {
        public void Configure(EntityTypeBuilder<Sector> builder)
        {
            builder.ToTable("Sectores");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Codigo)
                .HasMaxLength(20);

            builder.Property(x => x.LatitudCentro)
                .HasPrecision(10, 7);

            builder.Property(x => x.LongitudCentro)
                .HasPrecision(10, 7);

            builder.Property(x => x.GooglePlaceId)
                .HasMaxLength(255);

            builder.HasOne(x => x.Municipio)
                .WithMany(x => x.Sectores)
                .HasForeignKey(x => x.MunicipioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.MunicipioId, x.Nombre })
                .IsUnique();
        }
    }
}
