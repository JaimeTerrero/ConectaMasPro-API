using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarTerminosCondiciones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DocumentosLegales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Version = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Contenido = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FechaPublicacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EstaVigente = table.Column<bool>(type: "bit", nullable: false),
                    RequiereAceptacion = table.Column<bool>(type: "bit", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentosLegales", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AceptacionesDocumentosLegales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentoLegalId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FechaAceptacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DireccionIp = table.Column<string>(type: "nvarchar(45)", maxLength: 45, nullable: true),
                    AgenteUsuario = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AceptacionesDocumentosLegales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AceptacionesDocumentosLegales_DocumentosLegales_DocumentoLegalId",
                        column: x => x.DocumentoLegalId,
                        principalTable: "DocumentosLegales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AceptacionesDocumentosLegales_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AceptacionesDocumentosLegales_DocumentoLegalId",
                table: "AceptacionesDocumentosLegales",
                column: "DocumentoLegalId");

            migrationBuilder.CreateIndex(
                name: "IX_AceptacionesDocumentosLegales_UsuarioId_DocumentoLegalId",
                table: "AceptacionesDocumentosLegales",
                columns: new[] { "UsuarioId", "DocumentoLegalId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosLegales_Tipo",
                table: "DocumentosLegales",
                column: "Tipo",
                unique: true,
                filter: "[EstaVigente] = 1 AND [Activo] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentosLegales_Tipo_Version",
                table: "DocumentosLegales",
                columns: new[] { "Tipo", "Version" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AceptacionesDocumentosLegales");

            migrationBuilder.DropTable(
                name: "DocumentosLegales");
        }
    }
}
