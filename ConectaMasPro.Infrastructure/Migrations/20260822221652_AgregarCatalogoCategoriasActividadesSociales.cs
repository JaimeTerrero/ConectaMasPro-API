using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarCatalogoCategoriasActividadesSociales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CategoriaActividadSocialId",
                table: "ActividadesSociales",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "CategoriasActividadesSociales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Orden = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoriasActividadesSociales", x => x.Id);
                });

            migrationBuilder.Sql("""
                UPDATE ActividadesSociales
                SET CategoriaActividadSocialId = CASE
                    WHEN LOWER(Categoria) LIKE '%deport%' OR LOWER(Categoria) LIKE '%ejercicio%'
                        OR LOWER(Categoria) LIKE '%física%' OR LOWER(Categoria) LIKE '%correr%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b401'
                    WHEN LOWER(Categoria) LIKE '%aire libre%' OR LOWER(Categoria) LIKE '%naturaleza%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b402'
                    WHEN LOWER(Categoria) LIKE '%estudio%' OR LOWER(Categoria) LIKE '%aprendiz%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b403'
                    WHEN LOWER(Categoria) LIKE '%evento%' OR LOWER(Categoria) LIKE '%entretenimiento%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b404'
                    WHEN LOWER(Categoria) LIKE '%voluntar%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b405'
                    WHEN LOWER(Categoria) LIKE '%tecnolog%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b406'
                    WHEN LOWER(Categoria) LIKE '%arte%' OR LOWER(Categoria) LIKE '%cultura%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b407'
                    WHEN LOWER(Categoria) LIKE '%bienestar%' OR LOWER(Categoria) LIKE '%salud%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b408'
                    WHEN LOWER(Categoria) LIKE '%network%' OR LOWER(Categoria) LIKE '%negocio%'
                        THEN 'e749d707-a2bf-4b1e-9d18-82adca90b409'
                    ELSE 'e749d707-a2bf-4b1e-9d18-82adca90b410'
                END;
                """);

            migrationBuilder.DropColumn(
                name: "Categoria",
                table: "ActividadesSociales");

            migrationBuilder.AlterColumn<Guid>(
                name: "CategoriaActividadSocialId",
                table: "ActividadesSociales",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "CategoriasActividadesSociales",
                columns: new[] { "Id", "Activo", "FechaCreacion", "FechaModificacion", "Nombre", "Orden" },
                values: new object[,]
                {
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b401"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Deportes y ejercicio", 1 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b402"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Actividades al aire libre", 2 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b403"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Estudio y aprendizaje", 3 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b404"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Eventos y entretenimiento", 4 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b405"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Voluntariado", 5 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b406"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Tecnología", 6 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b407"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Arte y cultura", 7 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b408"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Bienestar", 8 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b409"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Networking", 9 },
                    { new Guid("e749d707-a2bf-4b1e-9d18-82adca90b410"), true, new DateTime(2026, 8, 22, 0, 0, 0, 0, DateTimeKind.Utc), null, "Otros", 10 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesSociales_CategoriaActividadSocialId",
                table: "ActividadesSociales",
                column: "CategoriaActividadSocialId");

            migrationBuilder.CreateIndex(
                name: "IX_CategoriasActividadesSociales_Nombre",
                table: "CategoriasActividadesSociales",
                column: "Nombre",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ActividadesSociales_CategoriasActividadesSociales_CategoriaActividadSocialId",
                table: "ActividadesSociales",
                column: "CategoriaActividadSocialId",
                principalTable: "CategoriasActividadesSociales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ActividadesSociales_CategoriasActividadesSociales_CategoriaActividadSocialId",
                table: "ActividadesSociales");

            migrationBuilder.DropIndex(
                name: "IX_ActividadesSociales_CategoriaActividadSocialId",
                table: "ActividadesSociales");

            migrationBuilder.AddColumn<string>(
                name: "Categoria",
                table: "ActividadesSociales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.Sql("""
                UPDATE a
                SET a.Categoria = c.Nombre
                FROM ActividadesSociales a
                INNER JOIN CategoriasActividadesSociales c
                    ON c.Id = a.CategoriaActividadSocialId;
                """);

            migrationBuilder.AlterColumn<string>(
                name: "Categoria",
                table: "ActividadesSociales",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.DropColumn(
                name: "CategoriaActividadSocialId",
                table: "ActividadesSociales");

            migrationBuilder.DropTable(
                name: "CategoriasActividadesSociales");
        }
    }
}
