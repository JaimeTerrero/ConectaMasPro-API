using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class ConversacionConfiguration : IEntityTypeConfiguration<Conversacion>
    {
        public void Configure(EntityTypeBuilder<Conversacion> builder)
        {
            builder.ToTable("Conversaciones");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.SolicitudServicio)
                .WithOne(x => x.Conversacion)
                .HasForeignKey<Conversacion>(x => x.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SolicitudServicioId)
                .IsUnique();

            builder.HasOne(x => x.ActividadSocial)
                .WithMany(x => x.Conversaciones)
                .HasForeignKey(x => x.ActividadSocialId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.ActividadSocialId, x.UsuarioUnoId, x.UsuarioDosId })
                .IsUnique()
                .HasFilter("[ActividadSocialId] IS NOT NULL");

            builder.HasOne(x => x.UsuarioUno)
                .WithMany()
                .HasForeignKey(x => x.UsuarioUnoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.UsuarioDos)
                .WithMany()
                .HasForeignKey(x => x.UsuarioDosId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
