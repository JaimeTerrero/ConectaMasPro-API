using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class ResenaConfiguration : IEntityTypeConfiguration<Resena>
    {
        public void Configure(EntityTypeBuilder<Resena> builder)
        {
            builder.ToTable("Resenas", table =>
            {
                table.HasCheckConstraint(
                    "CK_Resenas_CalificacionGeneral",
                    "[CalificacionGeneral] >= 1 AND [CalificacionGeneral] <= 5");
                table.HasCheckConstraint(
                    "CK_Resenas_EvaluadorEvaluado",
                    "[EvaluadorId] <> [EvaluadoId]");
            });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.CalificacionGeneral)
                .IsRequired();

            builder.Property(x => x.Comentario)
                .HasMaxLength(1000);

            builder.HasIndex(x => new { x.SolicitudServicioId, x.EvaluadorId })
                .IsUnique()
                .HasFilter("[Activo] = 1");

            builder.HasOne(x => x.Evaluador)
                .WithMany()
                .HasForeignKey(x => x.EvaluadorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Evaluado)
                .WithMany(x => x.ResenasRecibidas)
                .HasForeignKey(x => x.EvaluadoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SolicitudServicio)
                .WithMany(x => x.Resenas)
                .HasForeignKey(x => x.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
