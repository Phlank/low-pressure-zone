using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20260816182254_EnrichPerformer : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Timeslots_Performers_PerformerId",
            table: "Timeslots");

        migrationBuilder.DropColumn(
            name: "CreatedDate",
            table: "Performers");

        migrationBuilder.DropColumn(
            name: "LastModifiedDate",
            table: "Performers");

        migrationBuilder.RenameColumn(
            name: "Url",
            table: "Performers",
            newName: "SocialUrl");

        migrationBuilder.AlterColumn<string>(name: "SocialUrl", 
                                             table: "Performers", 
                                             maxLength: 512);

        migrationBuilder.RenameColumn(
            name: "LinkedUserId",
            table: "Performers",
            newName: "CreatorUserId");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
                                      name: "SocialUrl",
                                      table: "Performers",
                                      newName: "Url");

        migrationBuilder.AlterColumn<string>(name: "Url", 
                                             table: "Performers", 
                                             maxLength: null);

        migrationBuilder.RenameColumn(
            name: "CreatorUserId",
            table: "Performers",
            newName: "LinkedUserId");

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedDate",
            table: "Performers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "LastModifiedDate",
            table: "Performers",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<string>(
            name: "Url",
            table: "Performers",
            type: "character varying(256)",
            maxLength: 256,
            nullable: true);

        migrationBuilder.AddForeignKey(
            name: "FK_Timeslots_Performers_PerformerId",
            table: "Timeslots",
            column: "PerformerId",
            principalTable: "Performers",
            principalColumn: "Id",
            onDelete: ReferentialAction.Cascade);
    }
}
