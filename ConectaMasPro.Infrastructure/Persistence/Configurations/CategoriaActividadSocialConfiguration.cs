using ConectaMasPro.Domain.Constants;
using ConectaMasPro.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ConectaMasPro.Infrastructure.Persistence.Configurations;

public class CategoriaActividadSocialConfiguration : IEntityTypeConfiguration<CategoriaActividadSocial>
{
    public void Configure(EntityTypeBuilder<CategoriaActividadSocial> builder)
    {
        builder.ToTable("CategoriasActividadesSociales");
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Nombre).HasMaxLength(100).IsRequired();
        builder.Property(x => x.Orden).IsRequired();
        builder.HasIndex(x => x.Nombre).IsUnique();

        var fecha = new DateTime(2026, 8, 22, 0, 0, 0, DateTimeKind.Utc);
        builder.HasData(
            Crear(CategoriasActividadesSocialesIds.DeportesEjercicio, "Deportes y ejercicio", 1, fecha),
            Crear(CategoriasActividadesSocialesIds.AireLibre, "Actividades al aire libre", 2, fecha),
            Crear(CategoriasActividadesSocialesIds.EstudioAprendizaje, "Estudio y aprendizaje", 3, fecha),
            Crear(CategoriasActividadesSocialesIds.EventosEntretenimiento, "Eventos y entretenimiento", 4, fecha),
            Crear(CategoriasActividadesSocialesIds.Voluntariado, "Voluntariado", 5, fecha),
            Crear(CategoriasActividadesSocialesIds.Tecnologia, "Tecnología", 6, fecha),
            Crear(CategoriasActividadesSocialesIds.ArteCultura, "Arte y cultura", 7, fecha),
            Crear(CategoriasActividadesSocialesIds.Bienestar, "Bienestar", 8, fecha),
            Crear(CategoriasActividadesSocialesIds.Networking, "Networking", 9, fecha),
            Crear(CategoriasActividadesSocialesIds.Otros, "Otros", 10, fecha));
    }

    private static CategoriaActividadSocial Crear(Guid id, string nombre, int orden, DateTime fecha) => new()
    {
        Id = id,
        Nombre = nombre,
        Orden = orden,
        FechaCreacion = fecha,
        Activo = true
    };
}
