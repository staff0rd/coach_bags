using Microsoft.EntityFrameworkCore.Migrations;

namespace coach_bags_selenium.Migrations
{
    public partial class ProductEdit : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "edit",
                table: "products",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "edit",
                table: "products");
        }
    }
}
