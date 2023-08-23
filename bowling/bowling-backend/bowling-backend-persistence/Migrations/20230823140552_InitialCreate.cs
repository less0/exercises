using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bowling_backend_persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BowlingGame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerNames = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BowlingGame", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Frame",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerIndex = table.Column<int>(type: "int", nullable: false),
                    Rolls = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsLastFrame = table.Column<bool>(type: "bit", nullable: false),
                    BonusPoints = table.Column<int>(type: "int", nullable: false),
                    BowlingGameId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Frame", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Frame_BowlingGame_BowlingGameId",
                        column: x => x.BowlingGameId,
                        principalTable: "BowlingGame",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Frame_BowlingGameId",
                table: "Frame",
                column: "BowlingGameId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Frame");

            migrationBuilder.DropTable(
                name: "BowlingGame");
        }
    }
}
