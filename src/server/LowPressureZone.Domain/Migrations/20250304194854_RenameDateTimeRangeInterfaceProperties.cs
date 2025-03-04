using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameDateTimeRangeInterfaceProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Timeslots",
                newName: "StartsAt");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Timeslots",
                newName: "EndsAt");

            migrationBuilder.RenameIndex(
                name: "IX_Timeslots_Start",
                table: "Timeslots",
                newName: "IX_Timeslots_StartsAt");

            migrationBuilder.RenameIndex(
                name: "IX_Timeslots_End",
                table: "Timeslots",
                newName: "IX_Timeslots_EndsAt");

            migrationBuilder.RenameColumn(
                name: "Start",
                table: "Schedules",
                newName: "StartsAt");

            migrationBuilder.RenameColumn(
                name: "End",
                table: "Schedules",
                newName: "EndsAt");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_Start",
                table: "Schedules",
                newName: "IX_Schedules_StartsAt");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_End",
                table: "Schedules",
                newName: "IX_Schedules_EndsAt");

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Performers",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Audiences",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Performers");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Audiences");

            migrationBuilder.RenameColumn(
                name: "StartsAt",
                table: "Timeslots",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndsAt",
                table: "Timeslots",
                newName: "End");

            migrationBuilder.RenameIndex(
                name: "IX_Timeslots_StartsAt",
                table: "Timeslots",
                newName: "IX_Timeslots_Start");

            migrationBuilder.RenameIndex(
                name: "IX_Timeslots_EndsAt",
                table: "Timeslots",
                newName: "IX_Timeslots_End");

            migrationBuilder.RenameColumn(
                name: "StartsAt",
                table: "Schedules",
                newName: "Start");

            migrationBuilder.RenameColumn(
                name: "EndsAt",
                table: "Schedules",
                newName: "End");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_StartsAt",
                table: "Schedules",
                newName: "IX_Schedules_Start");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_EndsAt",
                table: "Schedules",
                newName: "IX_Schedules_End");
        }
    }
}
