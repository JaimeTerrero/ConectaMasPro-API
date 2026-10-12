using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations;

public class ActividadSocialConfiguration : IEntityTypeConfiguration<ActividadSocial>
{
    public void Configure(EntityTypeBuilder<ActividadSocial> builder)
    {
        builder.ToTable("ActividadesSociales", t =>
            t.HasCheckConstraint("CK_ActividadesSociales_CantidadPersonasBuscadas", "[CantidadPersonasBuscadas] > 0"));
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Titulo).HasMaxLength(150).IsRequired();
        builder.Property(x => x.Descripcion).HasMaxLength(2000).IsRequired();
        builder.Property(x => x.Ubicacion).HasMaxLength(300).IsRequired();
        builder.Property(x => x.Requisitos).HasMaxLength(1000);
        builder.Property(x => x.MontoOfrecido).HasPrecision(18, 2);
        builder.Property(x => x.Modalidad).HasConversion<int>().IsRequired();
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();
        builder.HasIndex(x => new { x.Estado, x.FechaActividad });
        builder.HasOne(x => x.Creador).WithMany(x => x.ActividadesSocialesCreadas)
            .HasForeignKey(x => x.CreadorId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.CategoriaActividadSocial).WithMany(x => x.Actividades)
            .HasForeignKey(x => x.CategoriaActividadSocialId).OnDelete(DeleteBehavior.Restrict);
    }
}
