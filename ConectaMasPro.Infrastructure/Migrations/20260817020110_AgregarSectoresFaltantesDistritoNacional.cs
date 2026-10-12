using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarSectoresFaltantesDistritoNacional : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                INSERT INTO Sectores
                (
                    Id, MunicipioId, Nombre, Codigo, FechaCreacion,
                    FechaModificacion, Activo, LatitudCentro,
                    LongitudCentro, GooglePlaceId
                )
                SELECT
                    NEWID(),
                    CONVERT(uniqueidentifier, N'F2C6D66E-8D8B-46E2-B3B4-100000000001'),
                    Fuente.Nombre,
                    Fuente.Codigo,
                    SYSUTCDATETIME(),
                    NULL,
                    1,
                    NULL,
                    NULL,
                    NULL
                FROM (VALUES
                    (N'27 de Febrero', N'DN-016'),
                    (N'La Castellana', N'DN-017'),
                    (N'Las Praderas', N'DN-018'),
                    (N'Los Próceres', N'DN-019')
                ) AS Fuente(Nombre, Codigo)
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM Sectores
                    WHERE Sectores.MunicipioId = CONVERT(uniqueidentifier, N'F2C6D66E-8D8B-46E2-B3B4-100000000001')
                      AND Sectores.Nombre = Fuente.Nombre
                );
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE Sectores
                SET Activo = 0,
                    FechaModificacion = SYSUTCDATETIME()
                WHERE MunicipioId = CONVERT(uniqueidentifier, N'F2C6D66E-8D8B-46E2-B3B4-100000000001')
                  AND Codigo IN (N'DN-016', N'DN-017', N'DN-018', N'DN-019')
                  AND EXISTS
                  (
                      SELECT 1
                      FROM UbicacionesUsuario
                      WHERE UbicacionesUsuario.SectorId = Sectores.Id
                  );

                DELETE FROM Sectores
                WHERE MunicipioId = CONVERT(uniqueidentifier, N'F2C6D66E-8D8B-46E2-B3B4-100000000001')
                  AND Codigo IN (N'DN-016', N'DN-017', N'DN-018', N'DN-019')
                  AND NOT EXISTS
                  (
                      SELECT 1
                      FROM UbicacionesUsuario
                      WHERE UbicacionesUsuario.SectorId = Sectores.Id
                  );
                """);
        }
    }
}
