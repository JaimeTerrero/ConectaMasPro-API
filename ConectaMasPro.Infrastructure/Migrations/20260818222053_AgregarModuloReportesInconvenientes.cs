using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloReportesInconvenientes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportesInconvenientes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportanteId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportadoId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SolicitudServicioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Tipo = table.Column<int>(type: "int", nullable: false),
                    Estado = table.Column<int>(type: "int", nullable: false),
                    Asunto = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Descripcion = table.Column<string>(type: "nvarchar(3000)", maxLength: 3000, nullable: false),
                    EvidenciaUrl = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Resolucion = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    FechaResolucion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportesInconvenientes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReportesInconvenientes_SolicitudesServicio_SolicitudServicioId",
                        column: x => x.SolicitudServicioId,
                        principalTable: "SolicitudesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportesInconvenientes_Usuarios_ReportadoId",
                        column: x => x.ReportadoId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ReportesInconvenientes_Usuarios_ReportanteId",
                        column: x => x.ReportanteId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesInconvenientes_Estado_FechaCreacion",
                table: "ReportesInconvenientes",
                columns: new[] { "Estado", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesInconvenientes_ReportadoId",
                table: "ReportesInconvenientes",
                column: "ReportadoId");

            migrationBuilder.CreateIndex(
                name: "IX_ReportesInconvenientes_ReportanteId_FechaCreacion",
                table: "ReportesInconvenientes",
                columns: new[] { "ReportanteId", "FechaCreacion" });

            migrationBuilder.CreateIndex(
                name: "IX_ReportesInconvenientes_SolicitudServicioId",
                table: "ReportesInconvenientes",
                column: "SolicitudServicioId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportesInconvenientes");
        }
    }
}
