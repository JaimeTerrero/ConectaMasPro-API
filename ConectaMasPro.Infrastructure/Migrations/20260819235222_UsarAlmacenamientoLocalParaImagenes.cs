using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UsarAlmacenamientoLocalParaImagenes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EvidenciaContenido",
                table: "ReportesInconvenientes");

            migrationBuilder.DropColumn(
                name: "EvidenciaNombreArchivo",
                table: "ReportesInconvenientes");

            migrationBuilder.DropColumn(
                name: "EvidenciaTipoContenido",
                table: "ReportesInconvenientes");

            migrationBuilder.DropColumn(
                name: "FotoContenido",
                table: "PerfilesUsuario");

            migrationBuilder.DropColumn(
                name: "FotoNombreArchivo",
                table: "PerfilesUsuario");

            migrationBuilder.DropColumn(
                name: "FotoTipoContenido",
                table: "PerfilesUsuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "EvidenciaContenido",
                table: "ReportesInconvenientes",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciaNombreArchivo",
                table: "ReportesInconvenientes",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EvidenciaTipoContenido",
                table: "ReportesInconvenientes",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<byte[]>(
                name: "FotoContenido",
                table: "PerfilesUsuario",
                type: "varbinary(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoNombreArchivo",
                table: "PerfilesUsuario",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FotoTipoContenido",
                table: "PerfilesUsuario",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
