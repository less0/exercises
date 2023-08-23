using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bowling_backend_persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "BowlingGame",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BowlingGame_Id_UserId",
                table: "BowlingGame",
                columns: new[] { "Id", "UserId" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_BowlingGame_Id_UserId",
                table: "BowlingGame");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "BowlingGame");
        }
    }
}
