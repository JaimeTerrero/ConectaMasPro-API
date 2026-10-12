using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations
{
    public class MetodoPagoConfiguration : IEntityTypeConfiguration<MetodoPago>
    {
        public void Configure(EntityTypeBuilder<MetodoPago> builder)
        {
            builder.ToTable("MetodosPago");

            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.Tipo)
                .HasConversion<int>()
                .IsRequired();
        }
    }
}
