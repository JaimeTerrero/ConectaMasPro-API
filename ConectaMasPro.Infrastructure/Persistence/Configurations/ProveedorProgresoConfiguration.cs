using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class ProveedorProgresoConfiguration
        : IEntityTypeConfiguration<ProveedorProgreso>
    {
        public void Configure(
            EntityTypeBuilder<ProveedorProgreso> builder)
        {
            builder.ToTable(
                "ProveedoresProgresos",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_ProveedoresProgresos_ServiciosCompletados",
                        "[ServiciosCompletados] >= 0");
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.ServiciosCompletados)
                .IsRequired();

            builder.Property(x => x.Nivel)
                .HasConversion<int>()
                .IsRequired();

            builder.HasOne(x => x.Usuario)
                .WithOne(x => x.ProgresoProveedor)
                .HasForeignKey<ProveedorProgreso>(
                    x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.UsuarioId)
                .IsUnique();
        }
    }
}
