using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class ReporteInconvenienteConfiguration : IEntityTypeConfiguration<ReporteInconveniente>
    {
        public void Configure(EntityTypeBuilder<ReporteInconveniente> builder)
        {
            builder.ToTable("ReportesInconvenientes");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Tipo).HasConversion<int>().IsRequired();
            builder.Property(x => x.Estado).HasConversion<int>().IsRequired();
            builder.Property(x => x.Asunto).HasMaxLength(150).IsRequired();
            builder.Property(x => x.Descripcion).HasMaxLength(3000).IsRequired();
            builder.Property(x => x.EvidenciaUrl).HasMaxLength(1000);
            builder.Property(x => x.Resolucion).HasMaxLength(2000);

            builder.HasIndex(x => new { x.ReportanteId, x.FechaCreacion });
            builder.HasIndex(x => new { x.Estado, x.FechaCreacion });
            builder.HasIndex(x => x.SolicitudServicioId);

            builder.HasOne(x => x.Reportante)
                .WithMany(x => x.ReportesCreados)
                .HasForeignKey(x => x.ReportanteId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Reportado)
                .WithMany(x => x.ReportesRecibidos)
                .HasForeignKey(x => x.ReportadoId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SolicitudServicio)
                .WithMany(x => x.ReportesInconvenientes)
                .HasForeignKey(x => x.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
