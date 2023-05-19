using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HealthCareSysAPI.Migrations
{
    public partial class NewAttributesAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "price",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "AppointmentReview",
                table: "Appointments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "Appointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "price",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "AppointmentReview",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "Appointments");
        }
    }
}
