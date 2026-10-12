using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class MensajeConfiguration : IEntityTypeConfiguration<Mensaje>
    {
        public void Configure(EntityTypeBuilder<Mensaje> builder)
        {
            builder.ToTable("Mensajes");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Contenido)
                .HasMaxLength(3000)
                .IsRequired();

            builder.HasOne(x => x.Conversacion)
                .WithMany(x => x.Mensajes)
                .HasForeignKey(x => x.ConversacionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Emisor)
                .WithMany()
                .HasForeignKey(x => x.EmisorId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
