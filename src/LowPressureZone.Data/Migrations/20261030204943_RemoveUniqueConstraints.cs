using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20261030204943_RemoveUniqueConstraints : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Communities_Name",
            table: "Communities");

        migrationBuilder.DropIndex(
            name: "IX_Communities_SocialUrl",
            table: "Communities");

        migrationBuilder.CreateIndex(
            name: "IX_Communities_Name",
            table: "Communities",
            column: "Name");

        migrationBuilder.CreateIndex(
            name: "IX_Communities_SocialUrl",
            table: "Communities",
            column: "SocialUrl");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_Communities_Name",
            table: "Communities");

        migrationBuilder.DropIndex(
            name: "IX_Communities_SocialUrl",
            table: "Communities");

        migrationBuilder.CreateIndex(
            name: "IX_Communities_Name",
            table: "Communities",
            column: "Name",
            unique: true);

        migrationBuilder.CreateIndex(
            name: "IX_Communities_SocialUrl",
            table: "Communities",
            column: "SocialUrl",
            unique: true);
    }
}
