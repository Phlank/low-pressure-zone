using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20260829190621_AddNewScheduleAndCommunity : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        // Community Aggregate
        
        migrationBuilder.CreateTable(
            name: "NewCommunities",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                SocialUrl = table.Column<string>(type: "character varying(512)", maxLength: 512, nullable: false),
                IsDeleted = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NewCommunities", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Relationship",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                UserId = table.Column<Guid>(type: "uuid", nullable: false),
                IsOrganizer = table.Column<bool>(type: "boolean", nullable: false),
                IsPerformer = table.Column<bool>(type: "boolean", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Relationship", x => x.Id);
                table.ForeignKey(
                    name: "FK_Relationship_NewCommunities_CommunityId",
                    column: x => x.CommunityId,
                    principalTable: "NewCommunities",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.Sql(@"
            INSERT INTO ""NewCommunities"" (""Id"", ""Name"", ""SocialUrl"", ""IsDeleted"")
            SELECT ""Id"", ""Name"", ""Url"", ""IsDeleted"" FROM ""Communities"";

            INSERT INTO ""Relationship"" (""Id"", ""CommunityId"", ""UserId"", ""IsOrganizer"", ""IsPerformer"")
            SELECT ""Id"", ""CommunityId"", ""UserId"", ""IsOrganizer"", ""IsPerformer"" FROM ""CommunityRelationships"";
        ");
        
        // Schedule aggregate
        
        migrationBuilder.CreateTable(
            name: "NewSchedules",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                Description = table.Column<string>(type: "character varying(16384)", maxLength: 16384, nullable: false),
                CommunityId = table.Column<Guid>(type: "uuid", nullable: false),
                IsVisibleToPublic = table.Column<bool>(type: "boolean", nullable: false),
                AllowedSlotTypes_IsClashAllowed = table.Column<bool>(type: "boolean", nullable: false),
                AllowedSlotTypes_IsHourlyAllowed = table.Column<bool>(type: "boolean", nullable: false),
                TimeRange_EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                TimeRange_StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_NewSchedules", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ClashSlots",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                PerformerOneId = table.Column<Guid>(type: "uuid", nullable: false),
                PerformerTwoId = table.Column<Guid>(type: "uuid", nullable: false),
                Rounds = table.Column<List<string>>(type: "text[]", nullable: false),
                TimeRange_Duration = table.Column<int>(type: "integer", nullable: false),
                TimeRange_EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                TimeRange_StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ClashSlots", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "HourlySlots",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uuid", nullable: false),
                Subtitle = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: true),
                PerformerId = table.Column<Guid>(type: "uuid", nullable: false),
                ScheduleId = table.Column<Guid>(type: "uuid", nullable: false),
                Prerecord_AzuraCastMediaId = table.Column<int>(type: "integer", nullable: true),
                Prerecord_IsPrerecorded = table.Column<bool>(type: "boolean", nullable: false),
                Prerecord_UploadedFileName = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true),
                TimeRange_Duration = table.Column<int>(type: "integer", nullable: false),
                TimeRange_EndsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                TimeRange_StartsAt = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_HourlySlots", x => x.Id);
            });

        migrationBuilder.CreateIndex(
            name: "IX_ClashSlots_PerformerOneId",
            table: "ClashSlots",
            column: "PerformerOneId");

        migrationBuilder.CreateIndex(
            name: "IX_ClashSlots_PerformerTwoId",
            table: "ClashSlots",
            column: "PerformerTwoId");

        migrationBuilder.CreateIndex(
            name: "IX_ClashSlots_ScheduleId",
            table: "ClashSlots",
            column: "ScheduleId");

        migrationBuilder.CreateIndex(
            name: "IX_ClashSlots_TimeRange_StartsAt",
            table: "ClashSlots",
            column: "TimeRange_StartsAt",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_HourlySlots_PerformerId",
            table: "HourlySlots",
            column: "PerformerId");

        migrationBuilder.CreateIndex(
            name: "IX_HourlySlots_ScheduleId",
            table: "HourlySlots",
            column: "ScheduleId");

        migrationBuilder.CreateIndex(
            name: "IX_HourlySlots_TimeRange_StartsAt",
            table: "HourlySlots",
            column: "TimeRange_StartsAt",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_NewSchedules_CommunityId",
            table: "NewSchedules",
            column: "CommunityId");

        migrationBuilder.CreateIndex(
            name: "IX_NewSchedules_TimeRange_StartsAt",
            table: "NewSchedules",
            column: "TimeRange_StartsAt",
            unique: true);
        
        migrationBuilder.Sql(@"
            INSERT INTO ""NewSchedules"" (""Id"", ""Name"", ""Description"", ""CommunityId"", ""IsVisibleToPublic"", ""AllowedSlotTypes_IsClashAllowed"", ""AllowedSlotTypes_IsHourlyAllowed"", ""TimeRange_EndsAt"", ""TimeRange_StartsAt"")
            SELECT ""Id"", ""Name"", ""Description"", ""CommunityId"", NOT ""IsOrganizersOnly"", ""Type"" = 1, ""Type"" = 0, ""EndsAt"", ""StartsAt"" FROM ""Schedules"";
        ");
        
        migrationBuilder.Sql(@"
            INSERT INTO ""HourlySlots"" (""Id"", ""Subtitle"", ""PerformerId"", ""ScheduleId"", ""Prerecord_AzuraCastMediaId"", ""Prerecord_IsPrerecorded"", ""Prerecord_UploadedFileName"", ""TimeRange_Duration"", ""TimeRange_EndsAt"", ""TimeRange_StartsAt"")
            SELECT ""Id"", ""Subtitle"", ""PerformerId"", ""ScheduleId"", ""AzuraCastMediaId"", ""Type"" = 'Prerecorded', ""UploadedFileName"", EXTRACT(HOUR FROM (""EndsAt"" - ""StartsAt"")), ""EndsAt"", ""StartsAt"" FROM ""Timeslots"";
        ");
        
        migrationBuilder.Sql(@"
            INSERT INTO ""ClashSlots"" (""Id"", ""ScheduleId"", ""PerformerOneId"", ""PerformerTwoId"", ""Rounds"", ""TimeRange_Duration"", ""TimeRange_EndsAt"", ""TimeRange_StartsAt"")
            SELECT ""Id"", ""ScheduleId"", ""PerformerOneId"", ""PerformerTwoId"", ARRAY[""RoundOne"", ""RoundTwo"", ""RoundThree""], EXTRACT(HOUR FROM (""EndsAt"" - ""StartsAt"")), ""EndsAt"", ""StartsAt"" FROM ""Soundclashes"";
        ");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "ClashSlots");

        migrationBuilder.DropTable(
            name: "HourlySlots");

        migrationBuilder.DropTable(
            name: "Relationship");

        migrationBuilder.DropTable(
            name: "NewSchedules");

        migrationBuilder.DropTable(
            name: "NewCommunities");
    }
}
