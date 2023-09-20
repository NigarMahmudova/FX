using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyExperienceApp.Migrations
{
    public partial class addUserConfirmCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MailConfirmCode",
                table: "AspNetUsers",
                type: "int",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MailConfirmCode",
                table: "AspNetUsers");
        }
    }
}
