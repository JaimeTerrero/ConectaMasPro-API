using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class PerfilUsuarioConfiguration : IEntityTypeConfiguration<PerfilUsuario>
    {
        public void Configure(EntityTypeBuilder<PerfilUsuario> builder)
        {
            builder.ToTable("PerfilesUsuario");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Biografia)
                .HasMaxLength(1000);

            builder.Property(x => x.FotoUrl)
                .HasMaxLength(500);

            builder.Property(x => x.Ocupacion)
                .HasMaxLength(150);

            builder.Property(x => x.Experiencia)
                .HasMaxLength(1000);

            builder.Property(x => x.CalificacionPromedio)
                .HasPrecision(3, 2);
        }
    }
}
