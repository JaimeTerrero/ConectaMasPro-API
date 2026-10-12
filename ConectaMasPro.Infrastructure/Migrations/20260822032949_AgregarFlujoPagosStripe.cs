using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarFlujoPagosStripe : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "FechaConfirmacionCliente",
                table: "SolicitudesServicio",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaRechazoFinalizacionCliente",
                table: "SolicitudesServicio",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FechaReporteFinalizacionProveedor",
                table: "SolicitudesServicio",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MotivoRechazoFinalizacion",
                table: "SolicitudesServicio",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MetodoPagoId",
                table: "Pagos",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<decimal>(
                name: "Comision",
                table: "Pagos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EstadoPasarela",
                table: "Pagos",
                type: "nvarchar(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Moneda",
                table: "Pagos",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "dop");

            migrationBuilder.AddColumn<decimal>(
                name: "MontoServicio",
                table: "Pagos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PorcentajeComision",
                table: "Pagos",
                type: "decimal(5,2)",
                precision: 5,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "UltimoError",
                table: "Pagos",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FechaConfirmacionCliente",
                table: "SolicitudesServicio");

            migrationBuilder.DropColumn(
                name: "FechaRechazoFinalizacionCliente",
                table: "SolicitudesServicio");

            migrationBuilder.DropColumn(
                name: "FechaReporteFinalizacionProveedor",
                table: "SolicitudesServicio");

            migrationBuilder.DropColumn(
                name: "MotivoRechazoFinalizacion",
                table: "SolicitudesServicio");

            migrationBuilder.DropColumn(
                name: "EstadoPasarela",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "Moneda",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "MontoServicio",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "PorcentajeComision",
                table: "Pagos");

            migrationBuilder.DropColumn(
                name: "UltimoError",
                table: "Pagos");

            migrationBuilder.AlterColumn<Guid>(
                name: "MetodoPagoId",
                table: "Pagos",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Comision",
                table: "Pagos",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldPrecision: 18,
                oldScale: 2);
        }
    }
}
