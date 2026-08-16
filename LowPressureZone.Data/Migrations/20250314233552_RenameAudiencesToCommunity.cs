using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Domain.Migrations
{
    /// <inheritdoc />
    public partial class RenameAudiencesToCommunity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "AudienceRelationships",
                newName: "CommunityRelationships");
            
            migrationBuilder.RenameColumn(
                name: "AudienceId",
                table: "CommunityRelationships",
                newName: "CommunityId");

            migrationBuilder.RenameTable(
                name: "Audiences",
                newName: "Communities");

            migrationBuilder.RenameColumn(
                name: "AudienceId",
                table: "Schedules",
                newName: "CommunityId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_AudienceId",
                table: "Schedules",
                newName: "IX_Schedules_CommunityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CommunityRelationships",
                newName: "AudienceRelationships");
            
            migrationBuilder.RenameColumn(
                name: "CommunityId",
                table: "AudienceRelationships",
                newName: "AudienceId");

            migrationBuilder.RenameTable(
                name: "Communities",
                newName: "Audiences");

            migrationBuilder.RenameColumn(
                name: "CommunityId",
                table: "Schedules",
                newName: "AudienceId");

            migrationBuilder.RenameIndex(
                name: "IX_Schedules_CommunityId",
                table: "Schedules",
                newName: "IX_Schedules_AudienceId");
        }
    }
}
