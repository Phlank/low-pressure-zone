using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class AddSoundclashes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Timeslots",
                newName: "Subtitle");

            migrationBuilder.RenameColumn(
                name: "Subtitle",
                table: "Schedules",
                newName: "Description");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Schedules",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Soundclashes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformerOneId = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformerTwoId = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundOne = table.Column<string>(type: "text", nullable: false),
                    RoundTwo = table.Column<string>(type: "text", nullable: false),
                    RoundThree = table.Column<string>(type: "text", nullable: false),
                    StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soundclashes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Soundclashes_Performers_PerformerOneId",
                        column: x => x.PerformerOneId,
                        principalTable: "Performers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Soundclashes_Performers_PerformerTwoId",
                        column: x => x.PerformerTwoId,
                        principalTable: "Performers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Soundclashes_Schedules_ScheduleId",
                        column: x => x.ScheduleId,
                        principalTable: "Schedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Soundclashes_PerformerOneId",
                table: "Soundclashes",
                column: "PerformerOneId");

            migrationBuilder.CreateIndex(
                name: "IX_Soundclashes_PerformerTwoId",
                table: "Soundclashes",
                column: "PerformerTwoId");

            migrationBuilder.CreateIndex(
                name: "IX_Soundclashes_ScheduleId",
                table: "Soundclashes",
                column: "ScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Soundclashes");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Schedules");

            migrationBuilder.RenameColumn(
                name: "Subtitle",
                table: "Timeslots",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Schedules",
                newName: "Subtitle");
        }
    }
}
