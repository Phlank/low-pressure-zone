using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class DropPerformerNameUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Performers_Name",
                table: "Performers");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Performers_Name",
                table: "Performers",
                column: "Name",
                unique: true);
        }
    }
}
