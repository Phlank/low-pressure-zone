using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20260829221717_UpdateSettings : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Relationship_NewCommunities_CommunityId",
            table: "Relationship");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Relationship",
            table: "Relationship");

        migrationBuilder.DropPrimaryKey(
            name: "PK_NewSchedules",
            table: "NewSchedules");

        migrationBuilder.DropPrimaryKey(
            name: "PK_NewCommunities",
            table: "NewCommunities");

        migrationBuilder.DropColumn(
            name: "CreatedDate",
            table: "Settings");

        migrationBuilder.DropColumn(
            name: "LastModifiedDate",
            table: "Settings");

        migrationBuilder.RenameTable(
            name: "Relationship",
            newName: "Relationships");

        migrationBuilder.RenameTable(
            name: "NewSchedules",
            newName: "Schedules");

        migrationBuilder.RenameTable(
            name: "NewCommunities",
            newName: "Communities");

        migrationBuilder.RenameIndex(
            name: "IX_Relationship_CommunityId",
            table: "Relationships",
            newName: "IX_Relationships_CommunityId");

        migrationBuilder.RenameIndex(
            name: "IX_NewSchedules_TimeRange_StartsAt",
            table: "Schedules",
            newName: "IX_Schedules_TimeRange_StartsAt");

        migrationBuilder.RenameIndex(
            name: "IX_NewSchedules_CommunityId",
            table: "Schedules",
            newName: "IX_Schedules_CommunityId");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Relationships",
            table: "Relationships",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Schedules",
            table: "Schedules",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Communities",
            table: "Communities",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Relationships_Communities_CommunityId",
            table: "Relationships",
            column: "CommunityId",
            principalTable: "Communities",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Relationships_Communities_CommunityId",
            table: "Relationships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Schedules",
            table: "Schedules");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Relationships",
            table: "Relationships");

        migrationBuilder.DropPrimaryKey(
            name: "PK_Communities",
            table: "Communities");

        migrationBuilder.RenameTable(
            name: "Schedules",
            newName: "NewSchedules");

        migrationBuilder.RenameTable(
            name: "Relationships",
            newName: "Relationship");

        migrationBuilder.RenameTable(
            name: "Communities",
            newName: "NewCommunities");

        migrationBuilder.RenameIndex(
            name: "IX_Schedules_TimeRange_StartsAt",
            table: "NewSchedules",
            newName: "IX_NewSchedules_TimeRange_StartsAt");

        migrationBuilder.RenameIndex(
            name: "IX_Schedules_CommunityId",
            table: "NewSchedules",
            newName: "IX_NewSchedules_CommunityId");

        migrationBuilder.RenameIndex(
            name: "IX_Relationships_CommunityId",
            table: "Relationship",
            newName: "IX_Relationship_CommunityId");

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedDate",
            table: "Settings",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "LastModifiedDate",
            table: "Settings",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddPrimaryKey(
            name: "PK_NewSchedules",
            table: "NewSchedules",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_Relationship",
            table: "Relationship",
            column: "Id");

        migrationBuilder.AddPrimaryKey(
            name: "PK_NewCommunities",
            table: "NewCommunities",
            column: "Id");

        migrationBuilder.AddForeignKey(
            name: "FK_Relationship_NewCommunities_CommunityId",
            table: "Relationship",
            column: "CommunityId",
            principalTable: "NewCommunities",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
