using Microsoft.EntityFrameworkCore.Migrations;

namespace BlazorDemoUdemy.Server.Migrations
{
    public partial class AdminRole : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
            INSERT INTO AspNetRoles (Id, [Name], NormalizedName)
            VALUES('f7243df3-0459-4294-8d2d-818ece1ea5a3', 'Admin', 'Admin')
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
