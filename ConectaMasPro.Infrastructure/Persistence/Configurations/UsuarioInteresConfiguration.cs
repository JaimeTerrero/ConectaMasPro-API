using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class UsuarioInteresConfiguration
    : IEntityTypeConfiguration<UsuarioInteres>
    {
        public void Configure(EntityTypeBuilder<UsuarioInteres> builder)
        {
            builder.ToTable("UsuariosIntereses");

            builder.HasKey(x => x.Id);

            builder.HasOne(x => x.Usuario)
                .WithMany(x => x.Intereses)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Interes)
                .WithMany(x => x.Usuarios)
                .HasForeignKey(x => x.InteresId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => new { x.UsuarioId, x.InteresId })
                .IsUnique();
        }
    }
}
