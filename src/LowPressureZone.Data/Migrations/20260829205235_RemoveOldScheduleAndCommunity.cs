using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20260829205235_RemoveOldScheduleAndCommunity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "CommunityRelationships");

        migrationBuilder.DropTable(
            name: "Soundclashes");

        migrationBuilder.DropTable(
            name: "Timeslots");

        migrationBuilder.DropTable(
            name: "Schedules");

        migrationBuilder.DropTable(
            name: "Communities");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Communities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false),
                LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Name = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                Url = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Communities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "CommunityRelationships",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                IsOrganizer = table.Column<bool>(type: "boolean", nullable: false),
                IsPerformer = table.Column<bool>(type: "boolean", nullable: false),
                LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_CommunityRelationships", x => x.Id);
                table.ForeignKey(
                    name: "FK_CommunityRelationships_Communities_CommunityId",
                    column: x => x.CommunityId,
                    principalTable: "Communities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Schedules",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CommunityId = table.Column<Guid>(type: "uuid", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Description = table.Column<string>(type: "text", nullable: false),
                EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                IsOrganizersOnly = table.Column<bool>(type: "boolean", nullable: false),
                LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                Name = table.Column<string>(type: "text", nullable: false),
                StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Type = table.Column<int>(type: "integer", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Schedules", x => x.Id);
                table.ForeignKey(
                    name: "FK_Schedules_Communities_CommunityId",
                    column: x => x.CommunityId,
                    principalTable: "Communities",
                    principalColumn: "Id");
            });

        migrationBuilder.CreateTable(
            name: "Soundclashes",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PerformerOneId = table.Column<Guid>(type: "uuid", nullable: false),
                PerformerTwoId = table.Column<Guid>(type: "uuid", nullable: false),
                ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                RoundOne = table.Column<string>(type: "text", nullable: false),
                RoundThree = table.Column<string>(type: "text", nullable: false),
                RoundTwo = table.Column<string>(type: "text", nullable: false),
                StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
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

        migrationBuilder.CreateTable(
            name: "Timeslots",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                PerformerId = table.Column<Guid>(type: "uuid", nullable: false),
                ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                AzuraCastMediaId = table.Column<int>(type: "integer", nullable: true),
                CreatedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                LastModifiedDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                Subtitle = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: true),
                Type = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                UploadedFileName = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: true)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Timeslots", x => x.Id);
                table.ForeignKey(
                    name: "FK_Timeslots_Performers_PerformerId",
                    column: x => x.PerformerId,
                    principalTable: "Performers",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
                table.ForeignKey(
                    name: "FK_Timeslots_Schedules_ScheduleId",
                    column: x => x.ScheduleId,
                    principalTable: "Schedules",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex(
            name: "IX_CommunityRelationships_CommunityId",
            table: "CommunityRelationships",
            column: "CommunityId");

        migrationBuilder.CreateIndex(
            name: "IX_Schedules_CommunityId",
            table: "Schedules",
            column: "CommunityId");

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

        migrationBuilder.CreateIndex(
            name: "IX_Timeslots_PerformerId",
            table: "Timeslots",
            column: "PerformerId");

        migrationBuilder.CreateIndex(
            name: "IX_Timeslots_ScheduleId",
            table: "Timeslots",
            column: "ScheduleId");
    }
}
