using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PerformanceType : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Timeslots");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Timeslots");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Schedules");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Performers");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Performers");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "Audiences");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "Audiences");

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Timeslots",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Timeslots");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Timeslots",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Timeslots",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Schedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Performers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Performers",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "Audiences",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "Audiences",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
