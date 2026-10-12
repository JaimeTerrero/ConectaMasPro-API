using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class ServicioConfiguration : IEntityTypeConfiguration<Servicio>
    {
        public void Configure(EntityTypeBuilder<Servicio> builder)
        {
            builder.ToTable("Servicios");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Titulo)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Descripcion)
                .HasMaxLength(2000)
                .IsRequired();

            builder.Property(x => x.PrecioBase)
                .HasPrecision(18, 2);

            builder.HasOne(x => x.Usuario)
                .WithMany(x => x.Servicios)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.CategoriaServicio)
                .WithMany(x => x.Servicios)
                .HasForeignKey(x => x.CategoriaServicioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
