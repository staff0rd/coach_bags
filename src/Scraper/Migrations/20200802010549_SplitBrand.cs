using Microsoft.EntityFrameworkCore.Migrations;

namespace coach_bags_selenium.Migrations
{
    public partial class SplitBrand : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "brand",
                table: "products",
                nullable: true);

            migrationBuilder.Sql(@"UPDATE products SET brand =
                CASE Category
                WHEN 0 THEN 'Coach'
                ELSE substring(name, 0, position(' - ' in name))
                END,
                name = CASE Category
                WHEN 0 THEN name
                ELSE substring(name, position(' - ' in name) + 3)
                END");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "brand",
                table: "products");
        }
    }
}
