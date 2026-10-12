using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class MovimientoRecompensaConfiguration
        : IEntityTypeConfiguration<MovimientoRecompensa>
    {
        public void Configure(
            EntityTypeBuilder<MovimientoRecompensa> builder)
        {
            builder.ToTable("MovimientosRecompensas",
                tableBuilder =>
                {
                    tableBuilder.HasCheckConstraint(
                        "CK_MovimientosRecompensas_Puntos",
                        "[Puntos] > 0");

                    tableBuilder.HasCheckConstraint(
                        "CK_MovimientosRecompensas_Saldos",
                        "[SaldoAnterior] >= 0 AND [SaldoPosterior] >= 0");
                });

            builder.HasKey(x => x.Id);

            builder.Property(x => x.TipoMovimiento)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.Puntos)
                .IsRequired();

            builder.Property(x => x.SaldoAnterior)
                .IsRequired();

            builder.Property(x => x.SaldoPosterior)
                .IsRequired();

            builder.Property(x => x.Concepto)
                .HasMaxLength(250)
                .IsRequired();

            builder.HasOne(x => x.UsuarioRecompensa)
                .WithMany(x => x.Movimientos)
                .HasForeignKey(x => x.UsuarioRecompensaId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.Usuario)
                .WithMany(x => x.MovimientosRecompensas)
                .HasForeignKey(x => x.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(x => x.SolicitudServicio)
                .WithOne(x => x.MovimientoRecompensa)
                .HasForeignKey<MovimientoRecompensa>(
                    x => x.SolicitudServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasIndex(x => x.SolicitudServicioId)
                .IsUnique()
                .HasFilter("[SolicitudServicioId] IS NOT NULL");
        }
    }
}
