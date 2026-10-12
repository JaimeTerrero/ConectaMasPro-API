using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class NotificacionConfiguration : IEntityTypeConfiguration<Notificacion>
    {
        public void Configure(EntityTypeBuilder<Notificacion> builder)
        {
            builder.ToTable("Notificaciones");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Titulo)
                .HasMaxLength(150)
                .IsRequired();

            builder.Property(x => x.Mensaje)
                .HasMaxLength(1000)
                .IsRequired();

            builder.HasOne(x => x.Usuario)
                .WithMany()
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
