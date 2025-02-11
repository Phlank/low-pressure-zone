using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIndicesForTimeRanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Schedules_Start_End",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_End",
                table: "Timeslots",
                column: "End",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Timeslots_Start",
                table: "Timeslots",
                column: "Start",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_End",
                table: "Schedules",
                column: "End",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Start",
                table: "Schedules",
                column: "Start",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Timeslots_End",
                table: "Timeslots");

            migrationBuilder.DropIndex(
                name: "IX_Timeslots_Start",
                table: "Timeslots");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_End",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_Start",
                table: "Schedules");

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Start_End",
                table: "Schedules",
                columns: new[] { "Start", "End" },
                unique: true);
        }
    }
}
