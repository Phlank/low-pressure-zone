using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Identity.Migrations
{
    /// <inheritdoc />
    public partial class LinkInvitationToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Invitations");

            migrationBuilder.DropColumn(
                name: "NormalizedEmail",
                table: "Invitations");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Invitations",
                type: "text",
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateIndex(
                name: "IX_Invitations_UserId",
                table: "Invitations",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Invitations_AspNetUsers_UserId",
                table: "Invitations",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Invitations_AspNetUsers_UserId",
                table: "Invitations");

            migrationBuilder.DropIndex(
                name: "IX_Invitations_UserId",
                table: "Invitations");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "Invitations",
                type: "uuid",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Invitations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "NormalizedEmail",
                table: "Invitations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
