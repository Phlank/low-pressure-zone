using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Identity.Migrations
{
    /// <inheritdoc />
    public partial class AddStreamerId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StreamerId",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StreamerId",
                table: "AspNetUsers");
        }
    }
}
