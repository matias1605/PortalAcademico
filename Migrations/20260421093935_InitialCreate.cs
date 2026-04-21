using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PortalAcademico.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cursos",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Codigo = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    Nombre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Creditos = table.Column<int>(type: "INTEGER", nullable: false),
                    CupoMaximo = table.Column<int>(type: "INTEGER", nullable: false),
                    HorarioInicio = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    HorarioFin = table.Column<TimeOnly>(type: "TEXT", nullable: false),
                    Activo = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cursos", x => x.Id);
                    table.CheckConstraint("CK_Curso_Creditos", "\"Creditos\" > 0");
                    table.CheckConstraint("CK_Curso_Horario", "\"HorarioInicio\" < \"HorarioFin\"");
                });

            migrationBuilder.CreateTable(
                name: "Matriculas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CursoId = table.Column<int>(type: "INTEGER", nullable: false),
                    UsuarioId = table.Column<string>(type: "TEXT", nullable: false),
                    FechaRegistro = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Estado = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Matriculas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Matriculas_Cursos_CursoId",
                        column: x => x.CursoId,
                        principalTable: "Cursos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cursos",
                columns: new[] { "Id", "Activo", "Codigo", "Creditos", "CupoMaximo", "HorarioFin", "HorarioInicio", "Nombre" },
                values: new object[,]
                {
                    { 1, true, "MAT101", 4, 30, new TimeOnly(10, 0, 0), new TimeOnly(8, 0, 0), "Matemáticas I" },
                    { 2, true, "PRG201", 3, 25, new TimeOnly(12, 0, 0), new TimeOnly(10, 0, 0), "Programación I" },
                    { 3, true, "FIS101", 4, 20, new TimeOnly(16, 0, 0), new TimeOnly(14, 0, 0), "Física I" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cursos_Codigo",
                table: "Cursos",
                column: "Codigo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Matriculas_CursoId_UsuarioId",
                table: "Matriculas",
                columns: new[] { "CursoId", "UsuarioId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Matriculas");

            migrationBuilder.DropTable(
                name: "Cursos");
        }
    }
}
