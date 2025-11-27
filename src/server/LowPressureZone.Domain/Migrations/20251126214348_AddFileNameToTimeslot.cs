using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddFileNameToTimeslot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UploadedFileName",
                table: "Timeslots",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UploadedFileName",
                table: "Timeslots");
        }
    }
}
