using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class PagoConfiguration : IEntityTypeConfiguration<Pago>
    {
        public void Configure(EntityTypeBuilder<Pago> builder)
        {
            builder.ToTable("Pagos");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Monto)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.MontoServicio)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.Comision)
                .HasPrecision(18, 2)
                .IsRequired();

            builder.Property(x => x.PorcentajeComision)
                .HasPrecision(5, 2)
                .IsRequired();

            builder.Property(x => x.Moneda)
                .HasMaxLength(3)
                .IsRequired();

            builder.Property(x => x.Estado)
                .HasConversion<int>()
                .IsRequired();

            builder.Property(x => x.ReferenciaTransaccion)
                .HasMaxLength(150);

            builder.Property(x => x.EstadoPasarela)
                .HasMaxLength(80);

            builder.Property(x => x.UltimoError)
                .HasMaxLength(500);

            builder.HasOne(x => x.MetodoPago)
                .WithMany(x => x.Pagos)
                .HasForeignKey(x => x.MetodoPagoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
