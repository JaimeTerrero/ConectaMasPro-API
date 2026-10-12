using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarConexionesSociales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ActividadSocialId",
                table: "Conversaciones",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ActividadesSociales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreadorId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Titulo = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    Categoria = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FechaActividad = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ubicacion = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    CantidadPersonasBuscadas = table.Column<int>(type: "int", nullable: false),
                    Modalidad = table.Column<int>(type: "int", nullable: false),
                    MontoOfrecido = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: true),
                    Requisitos = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ActividadesSociales", x => x.Id);
                    table.CheckConstraint("CK_ActividadesSociales_CantidadPersonasBuscadas", "[CantidadPersonasBuscadas] > 0");
                    table.ForeignKey(
                        name: "FK_ActividadesSociales_Usuarios_CreadorId",
                        column: x => x.CreadorId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "PostulacionesActividadesSociales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ActividadSocialId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostulanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Mensaje = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    FechaDecision = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostulacionesActividadesSociales", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostulacionesActividadesSociales_ActividadesSociales_ActividadSocialId",
                        column: x => x.ActividadSocialId,
                        principalTable: "ActividadesSociales",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PostulacionesActividadesSociales_Usuarios_PostulanteId",
                        column: x => x.PostulanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_ActividadSocialId_UsuarioUnoId_UsuarioDosId",
                table: "Conversaciones",
                columns: new[] { "ActividadSocialId", "UsuarioUnoId", "UsuarioDosId" },
                unique: true,
                filter: "[ActividadSocialId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesSociales_CreadorId",
                table: "ActividadesSociales",
                column: "CreadorId");

            migrationBuilder.CreateIndex(
                name: "IX_ActividadesSociales_Estado_FechaActividad",
                table: "ActividadesSociales",
                columns: new[] { "Estado", "FechaActividad" });

            migrationBuilder.CreateIndex(
                name: "IX_PostulacionesActividadesSociales_ActividadSocialId_PostulanteId",
                table: "PostulacionesActividadesSociales",
                columns: new[] { "ActividadSocialId", "PostulanteId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostulacionesActividadesSociales_PostulanteId",
                table: "PostulacionesActividadesSociales",
                column: "PostulanteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversaciones_ActividadesSociales_ActividadSocialId",
                table: "Conversaciones",
                column: "ActividadSocialId",
                principalTable: "ActividadesSociales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversaciones_ActividadesSociales_ActividadSocialId",
                table: "Conversaciones");

            migrationBuilder.DropTable(
                name: "PostulacionesActividadesSociales");

            migrationBuilder.DropTable(
                name: "ActividadesSociales");

            migrationBuilder.DropIndex(
                name: "IX_Conversaciones_ActividadSocialId_UsuarioUnoId_UsuarioDosId",
                table: "Conversaciones");

            migrationBuilder.DropColumn(
                name: "ActividadSocialId",
                table: "Conversaciones");
        }
    }
}
