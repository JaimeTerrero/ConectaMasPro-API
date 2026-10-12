using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class MunicipioConfiguration : IEntityTypeConfiguration<Municipio>
    {
        public void Configure(EntityTypeBuilder<Municipio> builder)
        {
            builder.ToTable("Municipios");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Codigo)
                .HasMaxLength(10);

            builder.HasOne(x => x.Provincia)
                .WithMany(x => x.Municipios)
                .HasForeignKey(x => x.ProvinciaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ProvinciaId, x.Nombre })
                .IsUnique();
        }
    }
}
