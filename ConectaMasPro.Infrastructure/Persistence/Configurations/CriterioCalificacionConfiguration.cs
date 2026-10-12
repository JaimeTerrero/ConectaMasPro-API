using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class CriterioCalificacionConfiguration : IEntityTypeConfiguration<CriterioCalificacion>
    {
        public void Configure(EntityTypeBuilder<CriterioCalificacion> builder)
        {
            builder.ToTable("CriteriosCalificacion", table =>
                table.HasCheckConstraint(
                    "CK_CriteriosCalificacion_Puntuacion",
                    "[Puntuacion] >= 1 AND [Puntuacion] <= 5"));

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Puntuacion)
                .IsRequired();

            builder.HasOne(x => x.Resena)
                .WithMany(x => x.Criterios)
                .HasForeignKey(x => x.ResenaId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
