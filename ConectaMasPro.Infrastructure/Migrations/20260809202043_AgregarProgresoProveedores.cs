using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AgregarProgresoProveedores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProveedoresProgresos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UsuarioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiciosCompletados = table.Column<int>(type: "int", nullable: false),
                    Nivel = table.Column<int>(type: "int", nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedoresProgresos", x => x.Id);
                    table.CheckConstraint("CK_ProveedoresProgresos_ServiciosCompletados", "[ServiciosCompletados] >= 0");
                    table.ForeignKey(
                        name: "FK_ProveedoresProgresos_Usuarios_UsuarioId",
                        column: x => x.UsuarioId,
                        principalTable: "Usuarios",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MovimientosProgresoProveedor",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProveedorProgresoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SolicitudServicioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiciosAnteriores = table.Column<int>(type: "int", nullable: false),
                    ServiciosPosteriores = table.Column<int>(type: "int", nullable: false),
                    Concepto = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovimientosProgresoProveedor", x => x.Id);
                    table.CheckConstraint("CK_MovimientosProgresoProveedor_Servicios", "[ServiciosAnteriores] >= 0 AND [ServiciosPosteriores] >= 0");
                    table.ForeignKey(
                        name: "FK_MovimientosProgresoProveedor_ProveedoresProgresos_ProveedorProgresoId",
                        column: x => x.ProveedorProgresoId,
                        principalTable: "ProveedoresProgresos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MovimientosProgresoProveedor_SolicitudesServicio_SolicitudServicioId",
                        column: x => x.SolicitudServicioId,
                        principalTable: "SolicitudesServicio",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProveedoresMetasMensuales",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProveedorProgresoId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Anio = table.Column<int>(type: "int", nullable: false),
                    Mes = table.Column<int>(type: "int", nullable: false),
                    MetaServicios = table.Column<int>(type: "int", nullable: false),
                    ServiciosCompletados = table.Column<int>(type: "int", nullable: false),
                    MetaCumplida = table.Column<bool>(type: "bit", nullable: false),
                    FechaCumplimiento = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FechaCreacion = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FechaModificacion = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProveedoresMetasMensuales", x => x.Id);
                    table.CheckConstraint("CK_ProveedoresMetasMensuales_Mes", "[Mes] >= 1 AND [Mes] <= 12");
                    table.CheckConstraint("CK_ProveedoresMetasMensuales_MetaServicios", "[MetaServicios] > 0");
                    table.CheckConstraint("CK_ProveedoresMetasMensuales_ServiciosCompletados", "[ServiciosCompletados] >= 0");
                    table.ForeignKey(
                        name: "FK_ProveedoresMetasMensuales_ProveedoresProgresos_ProveedorProgresoId",
                        column: x => x.ProveedorProgresoId,
                        principalTable: "ProveedoresProgresos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosProgresoProveedor_ProveedorProgresoId",
                table: "MovimientosProgresoProveedor",
                column: "ProveedorProgresoId");

            migrationBuilder.CreateIndex(
                name: "IX_MovimientosProgresoProveedor_SolicitudServicioId",
                table: "MovimientosProgresoProveedor",
                column: "SolicitudServicioId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedoresMetasMensuales_ProveedorProgresoId_Anio_Mes",
                table: "ProveedoresMetasMensuales",
                columns: new[] { "ProveedorProgresoId", "Anio", "Mes" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProveedoresProgresos_UsuarioId",
                table: "ProveedoresProgresos",
                column: "UsuarioId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MovimientosProgresoProveedor");

            migrationBuilder.DropTable(
                name: "ProveedoresMetasMensuales");

            migrationBuilder.DropTable(
                name: "ProveedoresProgresos");
        }
    }
}
