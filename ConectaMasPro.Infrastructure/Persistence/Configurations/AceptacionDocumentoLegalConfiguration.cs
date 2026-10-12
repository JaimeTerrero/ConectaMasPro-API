using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class AceptacionDocumentoLegalConfiguration : IEntityTypeConfiguration<AceptacionDocumentoLegal>
    {
        public void Configure(EntityTypeBuilder<AceptacionDocumentoLegal> builder)
        {
            builder.ToTable("AceptacionesDocumentosLegales");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.DireccionIp).HasMaxLength(45);
            builder.Property(x => x.AgenteUsuario).HasMaxLength(500);

            builder.HasIndex(x => new { x.UsuarioId, x.DocumentoLegalId }).IsUnique();

            builder.HasOne(x => x.Usuario)
                .WithMany(x => x.AceptacionesDocumentosLegales)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.DocumentoLegal)
                .WithMany(x => x.Aceptaciones)
                .HasForeignKey(x => x.DocumentoLegalId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
