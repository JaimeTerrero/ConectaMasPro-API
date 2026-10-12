using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UbicacionesMapaHibridas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "UbicacionExacta",
                table: "UbicacionesUsuario",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "GooglePlaceId",
                table: "Sectores",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LatitudCentro",
                table: "Sectores",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "LongitudCentro",
                table: "Sectores",
                type: "decimal(10,7)",
                precision: 10,
                scale: 7,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UbicacionExacta",
                table: "UbicacionesUsuario");

            migrationBuilder.DropColumn(
                name: "GooglePlaceId",
                table: "Sectores");

            migrationBuilder.DropColumn(
                name: "LatitudCentro",
                table: "Sectores");

            migrationBuilder.DropColumn(
                name: "LongitudCentro",
                table: "Sectores");
        }
    }
}
