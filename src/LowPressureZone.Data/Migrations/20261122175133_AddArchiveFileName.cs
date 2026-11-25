using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20261122175133_AddArchiveFileName : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "ArchivedFilePath",
            table: "Broadcasts",
            type: "text",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "ArchivedFilePath",
            table: "Broadcasts");
    }
}
