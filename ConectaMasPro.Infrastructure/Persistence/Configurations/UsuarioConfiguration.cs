using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class UsuarioConfiguration : IEntityTypeConfiguration<Usuario>
    {
        public void Configure(EntityTypeBuilder<Usuario> builder)
        {
            builder.ToTable("Usuarios");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Apellido)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Email)
                .HasMaxLength(150)
                .IsRequired();

            builder.HasIndex(x => x.Email)
                .IsUnique();

            builder.Property(x => x.Telefono)
                .HasMaxLength(20);

            builder.Property(x => x.IdentityUserId)
            .IsRequired();

            builder.Property(x => x.TipoUsuario)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(x => x.Perfil)
                .WithOne(x => x.Usuario)
                .HasForeignKey<PerfilUsuario>(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Ubicacion)
                .WithOne(x => x.Usuario)
                .HasForeignKey<UbicacionUsuario>(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.IdentityUserId)
                .IsUnique();
        }
    }
}
