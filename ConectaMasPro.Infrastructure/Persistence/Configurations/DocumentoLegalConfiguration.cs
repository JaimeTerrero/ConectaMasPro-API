using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class DocumentoLegalConfiguration : IEntityTypeConfiguration<DocumentoLegal>
    {
        public void Configure(EntityTypeBuilder<DocumentoLegal> builder)
        {
            builder.ToTable("DocumentosLegales");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Version).HasMaxLength(20).IsRequired();
            builder.Property(x => x.Titulo).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Contenido).IsRequired();

            builder.HasIndex(x => new { x.Tipo, x.Version }).IsUnique();
            builder.HasIndex(x => x.Tipo)
                .IsUnique()
                .HasFilter("[EstaVigente] = 1 AND [Activo] = 1");

        }
    }
}
