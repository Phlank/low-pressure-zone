using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class PerformerReferencesOneUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Performers""
                ADD COLUMN ""LinkedUserId"" uuid NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';

                UPDATE ""Performers""
                SET ""LinkedUserId"" = ""LinkedUserIds""[1];

                ALTER TABLE ""Performers""
                DROP COLUMN ""LinkedUserIds"";
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                ALTER TABLE ""Performers""
                ADD COLUMN ""LinkedUserIds"" uuid[] NOT NULL DEFAULT ARRAY['00000000-0000-0000-0000-000000000000']::uuid[];

                UPDATE ""Performers""
                SET ""LinkedUserIds"" = ARRAY[""LinkedUserId""];

                ALTER TABLE ""Performers""
                DROP COLUMN ""LinkedUserId"";
            ");
        }
    }
}
