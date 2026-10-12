using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class ProveedorMetaMensualConfiguration
        : IEntityTypeConfiguration<ProveedorMetaMensual>
    {
        public void Configure(
            EntityTypeBuilder<ProveedorMetaMensual> builder)
        {
            builder.ToTable(
                "ProveedoresMetasMensuales",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_ProveedoresMetasMensuales_MetaServicios",
                        "[MetaServicios] > 0");

                    tableBuilder.HasCheckConstraint(
                        "CK_ProveedoresMetasMensuales_ServiciosCompletados",
                        "[ServiciosCompletados] >= 0");

                    tableBuilder.HasCheckConstraint(
                        "CK_ProveedoresMetasMensuales_Mes",
                        "[Mes] >= 1 AND [Mes] <= 12");
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Anio)
                .IsRequired();

            builder.Property(x => x.Mes)
                .IsRequired();

            builder.Property(x => x.MetaServicios)
                .IsRequired();

            builder.Property(x => x.ServiciosCompletados)
                .IsRequired();

            builder.Property(x => x.MetaCumplida)
                .IsRequired();

            builder.HasOne(x => x.ProveedorProgreso)
                .WithMany(x => x.MetasMensuales)
                .HasForeignKey(x => x.ProveedorProgresoId)
                .OnDelete(DeleteBehavior.Restrict);

            /*
             * Un proveedor solamente puede tener
             * una meta para un determinado mes/año.
             */
            builder.HasIndex(x => new
            {
                x.ProveedorProgresoId,
                x.Anio,
                x.Mes
            })
                .IsUnique();
        }
    }
}
