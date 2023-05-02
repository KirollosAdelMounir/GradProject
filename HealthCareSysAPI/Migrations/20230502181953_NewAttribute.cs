using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareSysAPI.Migrations
{
    public partial class NewAttribute : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SpecImage",
                table: "Specializations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SpecImage",
                table: "Specializations");
        }
    }
}
