using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ProfileCatalogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Municipio",
                table: "UbicacionesUsuario");

            migrationBuilder.DropColumn(
                name: "Provincia",
                table: "UbicacionesUsuario");

            migrationBuilder.DropColumn(
                name: "Sector",
                table: "UbicacionesUsuario");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitud",
                table: "UbicacionesUsuario",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitud",
                table: "UbicacionesUsuario",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(9,6)",
                oldPrecision: 9,
                oldScale: 6,
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "MunicipioId",
                table: "UbicacionesUsuario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "ProvinciaId",
                table: "UbicacionesUsuario",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "SectorId",
                table: "UbicacionesUsuario",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Intereses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Intereses", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Provincias",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Provincias", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UsuariosIntereses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InteresId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosIntereses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosIntereses_Intereses_InteresId",
                        column: x => x.InteresId,
                        principalTable: "Intereses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UsuariosIntereses_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Municipios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProvinciaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Municipios", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Municipios_Provincias_ProvinciaId",
                        column: x => x.ProvinciaId,
                        principalTable: "Provincias",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Sectores",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MunicipioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Nombre = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Codigo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sectores", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Sectores_Municipios_MunicipioId",
                        column: x => x.MunicipioId,
                        principalTable: "Municipios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UbicacionesUsuario_MunicipioId",
                table: "UbicacionesUsuario",
                column: "MunicipioId");

            migrationBuilder.CreateIndex(
                name: "IX_UbicacionesUsuario_ProvinciaId",
                table: "UbicacionesUsuario",
                column: "ProvinciaId");

            migrationBuilder.CreateIndex(
                name: "IX_UbicacionesUsuario_SectorId",
                table: "UbicacionesUsuario",
                column: "SectorId");

            migrationBuilder.CreateIndex(
                name: "IX_Municipios_ProvinciaId_Nombre",
                table: "Municipios",
                columns: new[] { "ProvinciaId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Provincias_Nombre",
                table: "Provincias",
                column: "Nombre",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Sectores_MunicipioId_Nombre",
                table: "Sectores",
                columns: new[] { "MunicipioId", "Nombre" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosIntereses_InteresId",
                table: "UsuariosIntereses",
                column: "InteresId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosIntereses_UsuarioId_InteresId",
                table: "UsuariosIntereses",
                columns: new[] { "UsuarioId", "InteresId" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UbicacionesUsuario_Municipios_MunicipioId",
                table: "UbicacionesUsuario",
                column: "MunicipioId",
                principalTable: "Municipios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UbicacionesUsuario_Provincias_ProvinciaId",
                table: "UbicacionesUsuario",
                column: "ProvinciaId",
                principalTable: "Provincias",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UbicacionesUsuario_Sectores_SectorId",
                table: "UbicacionesUsuario",
                column: "SectorId",
                principalTable: "Sectores",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UbicacionesUsuario_Municipios_MunicipioId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropForeignKey(
                name: "FK_UbicacionesUsuario_Provincias_ProvinciaId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropForeignKey(
                name: "FK_UbicacionesUsuario_Sectores_SectorId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropTable(
                name: "Sectores");

            migrationBuilder.DropTable(
                name: "UsuariosIntereses");

            migrationBuilder.DropTable(
                name: "Municipios");

            migrationBuilder.DropTable(
                name: "Intereses");

            migrationBuilder.DropTable(
                name: "Provincias");

            migrationBuilder.DropIndex(
                name: "IX_UbicacionesUsuario_MunicipioId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropIndex(
                name: "IX_UbicacionesUsuario_ProvinciaId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropIndex(
                name: "IX_UbicacionesUsuario_SectorId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropColumn(
                name: "MunicipioId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropColumn(
                name: "ProvinciaId",
                table: "UbicacionesUsuario");

            migrationBuilder.DropColumn(
                name: "SectorId",
                table: "UbicacionesUsuario");

            migrationBuilder.AlterColumn<decimal>(
                name: "Longitud",
                table: "UbicacionesUsuario",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Latitud",
                table: "UbicacionesUsuario",
                type: "decimal(9,6)",
                precision: 9,
                scale: 6,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(10,7)",
                oldPrecision: 10,
                oldScale: 7,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Municipio",
                table: "UbicacionesUsuario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Provincia",
                table: "UbicacionesUsuario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Sector",
                table: "UbicacionesUsuario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
