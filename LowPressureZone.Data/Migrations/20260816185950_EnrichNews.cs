using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20260816185950_EnrichNews : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "Body",
            table: "News",
            newName: "Content");

        migrationBuilder.AlterColumn<string>("Content", "News", maxLength: 16384);

        migrationBuilder.RenameColumn("CreatedDate", "News", "PublishedAt");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            schema: "lpz",
            table: "News",
            type: "character varying(256)",
            maxLength: 256,
            nullable: false,
            oldClrType: typeof(string),
            oldType: "text");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.RenameColumn(
            name: "PublishedAt",
            table: "News",
            newName: "CreatedDate");

        migrationBuilder.AlterColumn<string>(
            name: "Title",
            table: "News",
            type: "text",
            nullable: false,
            oldClrType: typeof(string),
            oldType: "character varying(256)",
            oldMaxLength: 256);
        
        migrationBuilder.RenameColumn("Content", "News", "Body");
        migrationBuilder.AlterColumn<string>("Body", "News", maxLength: null, nullable: false);

        migrationBuilder.AddColumn<DateTime>(
            name: "CreatedDate",
            table: "News",
            type: "timestamp with time zone",
            nullable: false,
            defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
    }
}
