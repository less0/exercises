using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace bowling_backend_persistence.Migrations
{
    /// <inheritdoc />
    public partial class FrameIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "FrameIndex",
                table: "Frame",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FrameIndex",
                table: "Frame");
        }
    }
}
