using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20260831041152_EnhanceBroadcastAggregate : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "CreatedDate",
            table: "Broadcasts");
        
        migrationBuilder.DropColumn(
            name: "LastModifiedDate",
            table: "Broadcasts");

        migrationBuilder.AddColumn<string>(
            name: "AzuraCastStreamerDisplayName",
            table: "Broadcasts",
            type: "text",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<int>(
            name: "AzuraCastStreamerId",
            table: "Broadcasts",
            type: "integer",
            nullable: false,
            defaultValue: 0);

        migrationBuilder.AddColumn<bool>(
            name: "HasFile",
            table: "Broadcasts",
            type: "boolean",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "Time_StartsAt",
            table: "Broadcasts",
            type: "timestamp with time zone",
            nullable: false);
        
        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "Time_EndsAt",
            table: "Broadcasts",
            type: "timestamp with time zone",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Broadcasts_AzuraCastBroadcastId",
            table: "Broadcasts",
            column: "AzuraCastBroadcastId",
            unique: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Broadcasts_AzuraCastBroadcastId",
            table: "Broadcasts");

        migrationBuilder.DropColumn(
            name: "AzuraCastStreamerDisplayName",
            table: "Broadcasts");

        migrationBuilder.DropColumn(
            name: "AzuraCastStreamerId",
            table: "Broadcasts");

        migrationBuilder.DropColumn(
            name: "HasFile",
            table: "Broadcasts");

        migrationBuilder.DropColumn(
            name: "Time_EndsAt",
            table: "Broadcasts");
        
        migrationBuilder.DropColumn(
            name: "Time_StartsAt",
            table: "Broadcasts");

        
        migrationBuilder.AddColumn<DateTime>(
            name: "LastModifiedDate",
            table: "Broadcasts",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedDate",
            table: "Broadcasts",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }
}
