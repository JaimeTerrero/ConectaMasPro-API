using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class VerificacionIdentidadConfiguration : IEntityTypeConfiguration<VerificacionIdentidad>
    {
        public void Configure(EntityTypeBuilder<VerificacionIdentidad> builder)
        {
            builder.ToTable("VerificacionesIdentidad");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.DocumentoUrl)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.ComentarioRevision)
                .HasMaxLength(1000);

            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
