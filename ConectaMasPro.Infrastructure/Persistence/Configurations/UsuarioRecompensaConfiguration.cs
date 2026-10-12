using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class UsuarioRecompensaConfiguration
        : IEntityTypeConfiguration<UsuarioRecompensa>
    {
        public void Configure(
            EntityTypeBuilder<UsuarioRecompensa> builder)
        {
            builder.ToTable("UsuariosRecompensas");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.PuntosActuales)
                .IsRequired();

            builder.Property(x => x.PuntosHistoricos)
                .IsRequired();

            builder.HasOne(x => x.Usuario)
                .WithOne(x => x.Recompensa)
                .HasForeignKey<UsuarioRecompensa>(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UsuarioId)
                .IsUnique();
        }
    }
}
