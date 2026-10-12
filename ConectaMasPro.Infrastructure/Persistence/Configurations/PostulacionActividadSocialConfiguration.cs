using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations;

public class PostulacionActividadSocialConfiguration : IEntityTypeConfiguration<PostulacionActividadSocial>
{
    public void Configure(EntityTypeBuilder<PostulacionActividadSocial> builder)
    {
        builder.ToTable("PostulacionesActividadesSociales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Mensaje).HasMaxLength(1000);
        builder.Property(x => x.Estado).HasConversion<int>().IsRequired();
        builder.HasIndex(x => new { x.ActividadSocialId, x.PostulanteId }).IsUnique();
        builder.HasOne(x => x.ActividadSocial).WithMany(x => x.Postulaciones)
            .HasForeignKey(x => x.ActividadSocialId).OnDelete(DeleteBehavior.Cascade);
        builder.HasOne(x => x.Postulante).WithMany(x => x.PostulacionesActividades)
            .HasForeignKey(x => x.PostulanteId).OnDelete(DeleteBehavior.Restrict);
    }
}
