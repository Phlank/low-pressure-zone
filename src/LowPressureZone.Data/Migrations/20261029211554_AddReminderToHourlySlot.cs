using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Data.Migrations;

/// <inheritdoc />
public partial class _20261029211554_AddReminderToHourlySlot : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Reminder_JobId",
            table: "HourlySlots",
            type: "text",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "Reminder_ReminderTime",
            table: "HourlySlots",
            type: "timestamp with time zone",
            nullable: true);
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Reminder_JobId",
            table: "HourlySlots");

        migrationBuilder.DropColumn(
            name: "Reminder_ReminderTime",
            table: "HourlySlots");
    }
}
