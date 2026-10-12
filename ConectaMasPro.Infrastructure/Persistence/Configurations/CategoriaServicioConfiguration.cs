using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class CategoriaServicioConfiguration : IEntityTypeConfiguration<CategoriaServicio>
    {
        public void Configure(EntityTypeBuilder<CategoriaServicio> builder)
        {
            builder.ToTable("CategoriasServicio");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasMaxLength(500);

            builder.HasIndex(x => x.Nombre)
                .IsUnique();
        }
    }
}
