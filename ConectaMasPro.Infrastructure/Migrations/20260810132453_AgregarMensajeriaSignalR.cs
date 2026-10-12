using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarMensajeriaSignalR : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SolicitudServicioId",
                table: "Conversaciones",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversaciones_SolicitudServicioId",
                table: "Conversaciones",
                column: "SolicitudServicioId",
                unique: true,
                filter: "[SolicitudServicioId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Conversaciones_SolicitudesServicio_SolicitudServicioId",
                table: "Conversaciones",
                column: "SolicitudServicioId",
                principalTable: "SolicitudesServicio",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Conversaciones_SolicitudesServicio_SolicitudServicioId",
                table: "Conversaciones");

            migrationBuilder.DropIndex(
                name: "IX_Conversaciones_SolicitudServicioId",
                table: "Conversaciones");

            migrationBuilder.DropColumn(
                name: "SolicitudServicioId",
                table: "Conversaciones");
        }
    }
}
