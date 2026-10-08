using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddIndexesAndSetupIdProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timeslots_Performers_PerformerId",
                table: "Timeslots");

            migrationBuilder.DropForeignKey(
                name: "FK_Timeslots_Schedules_ScheduleId",
                table: "Timeslots");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "Timeslots",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "PerformerId",
                table: "Timeslots",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Schedules_Start_End",
                table: "Schedules",
                columns: new[] { "Start", "End" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Performers_Name",
                table: "Performers",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Audiences_Name",
                table: "Audiences",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Timeslots_Performers_PerformerId",
                table: "Timeslots",
                column: "PerformerId",
                principalTable: "Performers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Timeslots_Schedules_ScheduleId",
                table: "Timeslots",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Timeslots_Performers_PerformerId",
                table: "Timeslots");

            migrationBuilder.DropForeignKey(
                name: "FK_Timeslots_Schedules_ScheduleId",
                table: "Timeslots");

            migrationBuilder.DropIndex(
                name: "IX_Schedules_Start_End",
                table: "Schedules");

            migrationBuilder.DropIndex(
                name: "IX_Performers_Name",
                table: "Performers");

            migrationBuilder.DropIndex(
                name: "IX_Audiences_Name",
                table: "Audiences");

            migrationBuilder.AlterColumn<Guid>(
                name: "ScheduleId",
                table: "Timeslots",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "PerformerId",
                table: "Timeslots",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddForeignKey(
                name: "FK_Timeslots_Performers_PerformerId",
                table: "Timeslots",
                column: "PerformerId",
                principalTable: "Performers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Timeslots_Schedules_ScheduleId",
                table: "Timeslots",
                column: "ScheduleId",
                principalTable: "Schedules",
                principalColumn: "Id");
        }
    }
}
