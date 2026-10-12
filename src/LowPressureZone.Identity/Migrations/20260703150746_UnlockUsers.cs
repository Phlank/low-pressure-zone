using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LowPressureZone.Identity.Migrations
{
    /// <inheritdoc />
    public partial class UnlockUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"UPDATE ""AspNetUsers"" SET ""LockoutEnabled"" = false;");
            migrationBuilder.Sql(@"UPDATE ""AspNetUsers"" SET ""LockoutEnd"" = '9999-12-31 00:00:00.000+00';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
