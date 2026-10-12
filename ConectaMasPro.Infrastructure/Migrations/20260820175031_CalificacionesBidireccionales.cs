using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConectaMasPro.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CalificacionesBidireccionales : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resenas_SolicitudServicioId",
                table: "Resenas");

            migrationBuilder.Sql(
                """
                WITH ResenasDuplicadas AS
                (
                    SELECT [Id],
                           ROW_NUMBER() OVER (
                               PARTITION BY [SolicitudServicioId], [EvaluadorId]
                               ORDER BY [FechaCreacion], [Id]) AS [Numero]
                    FROM [Resenas]
                    WHERE [Activo] = 1
                )
                UPDATE [Resenas]
                SET [Activo] = 0,
                    [FechaModificacion] = GETUTCDATE()
                WHERE [Id] IN (
                    SELECT [Id]
                    FROM ResenasDuplicadas
                    WHERE [Numero] > 1);
                """);

            migrationBuilder.Sql(
                """
                WITH Reputacion AS
                (
                    SELECT [EvaluadoId],
                           CAST(AVG(CAST([CalificacionGeneral] AS decimal(10, 2))) AS decimal(3, 2)) AS [Promedio],
                           COUNT(*) AS [Total]
                    FROM [Resenas]
                    WHERE [Activo] = 1
                    GROUP BY [EvaluadoId]
                )
                UPDATE perfil
                SET perfil.[CalificacionPromedio] = reputacion.[Promedio],
                    perfil.[TotalResenas] = reputacion.[Total],
                    perfil.[FechaModificacion] = GETUTCDATE()
                FROM [PerfilesUsuario] perfil
                INNER JOIN Reputacion reputacion
                    ON reputacion.[EvaluadoId] = perfil.[UsuarioId];

                WITH Reputacion AS
                (
                    SELECT [EvaluadoId],
                           CAST(AVG(CAST([CalificacionGeneral] AS decimal(10, 2))) AS decimal(3, 2)) AS [Promedio],
                           COUNT(*) AS [Total]
                    FROM [Resenas]
                    WHERE [Activo] = 1
                    GROUP BY [EvaluadoId]
                )
                INSERT INTO [PerfilesUsuario]
                    ([Id], [UsuarioId], [IdentidadVerificada], [CalificacionPromedio],
                     [TotalResenas], [FechaCreacion], [Activo])
                SELECT NEWID(), reputacion.[EvaluadoId], 0, reputacion.[Promedio],
                       reputacion.[Total], GETUTCDATE(), 1
                FROM Reputacion reputacion
                WHERE NOT EXISTS
                (
                    SELECT 1
                    FROM [PerfilesUsuario] perfil
                    WHERE perfil.[UsuarioId] = reputacion.[EvaluadoId]
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Resenas_SolicitudServicioId_EvaluadorId",
                table: "Resenas",
                columns: new[] { "SolicitudServicioId", "EvaluadorId" },
                unique: true,
                filter: "[Activo] = 1");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Resenas_CalificacionGeneral",
                table: "Resenas",
                sql: "[CalificacionGeneral] >= 1 AND [CalificacionGeneral] <= 5");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Resenas_EvaluadorEvaluado",
                table: "Resenas",
                sql: "[EvaluadorId] <> [EvaluadoId]");

            migrationBuilder.AddCheckConstraint(
                name: "CK_CriteriosCalificacion_Puntuacion",
                table: "CriteriosCalificacion",
                sql: "[Puntuacion] >= 1 AND [Puntuacion] <= 5");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Resenas_SolicitudServicioId_EvaluadorId",
                table: "Resenas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Resenas_CalificacionGeneral",
                table: "Resenas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Resenas_EvaluadorEvaluado",
                table: "Resenas");

            migrationBuilder.DropCheckConstraint(
                name: "CK_CriteriosCalificacion_Puntuacion",
                table: "CriteriosCalificacion");

            migrationBuilder.CreateIndex(
                name: "IX_Resenas_SolicitudServicioId",
                table: "Resenas",
                column: "SolicitudServicioId");
        }
    }
}
