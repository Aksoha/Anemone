using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Anemone.Repository.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "HeatingSystem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    ModificationDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatingSystem", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "HeatingSystemPoint",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Type = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeValue = table.Column<double>(type: "REAL", nullable: false),
                    Resistance = table.Column<double>(type: "REAL", nullable: false),
                    Inductance = table.Column<double>(type: "REAL", nullable: false),
                    HeatingSystemId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HeatingSystemPoint", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HeatingSystemPoint_HeatingSystem_HeatingSystemId",
                        column: x => x.HeatingSystemId,
                        principalTable: "HeatingSystem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_HeatingSystemPoint_HeatingSystemId",
                table: "HeatingSystemPoint",
                column: "HeatingSystemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HeatingSystemPoint");

            migrationBuilder.DropTable(
                name: "HeatingSystem");
        }
    }
}
