using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FamilyExperienceApp.Migrations
{
    public partial class OrderColumnDeletedFromProductTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Order",
                table: "Products");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Order",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
