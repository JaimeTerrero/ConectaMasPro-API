using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarModuloRecompensas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsuariosRecompensas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PuntosActuales = table.Column<int>(type: "int", nullable: false),
                    PuntosHistoricos = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsuariosRecompensas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UsuariosRecompensas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosRecompensas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioRecompensaId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudServicioId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TipoMovimiento = table.Column<int>(type: "int", nullable: false),
                    Puntos = table.Column<int>(type: "int", nullable: false),
                    SaldoAnterior = table.Column<int>(type: "int", nullable: false),
                    SaldoPosterior = table.Column<int>(type: "int", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosRecompensas", x => x.Id);
                    table.CheckConstraint("CK_MovimientosRecompensas_Puntos", "[Puntos] > 0");
                    table.CheckConstraint("CK_MovimientosRecompensas_Saldos", "[SaldoAnterior] >= 0 AND [SaldoPosterior] >= 0");
                    table.ForeignKey(
                        name: "FK_MovimientosRecompensas_SolicitudesServicio_SolicitudServicioId",
                        column: x => x.SolicitudServicioId,
                        principalTable: "SolicitudesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosRecompensas_UsuariosRecompensas_UsuarioRecompensaId",
                        column: x => x.UsuarioRecompensaId,
                        principalTable: "UsuariosRecompensas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosRecompensas_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosRecompensas_SolicitudServicioId",
                table: "MovimientosRecompensas",
                column: "SolicitudServicioId",
                unique: true,
                filter: "[SolicitudServicioId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosRecompensas_UsuarioId",
                table: "MovimientosRecompensas",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosRecompensas_UsuarioRecompensaId",
                table: "MovimientosRecompensas",
                column: "UsuarioRecompensaId");

            migrationBuilder.CreateIndex(
                name: "IX_UsuariosRecompensas_UsuarioId",
                table: "UsuariosRecompensas",
                column: "UsuarioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosRecompensas");

            migrationBuilder.DropTable(
                name: "UsuariosRecompensas");
        }
    }
}
