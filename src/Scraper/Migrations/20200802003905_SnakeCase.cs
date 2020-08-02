using Microsoft.EntityFrameworkCore.Migrations;

/*

Must run before:

alter table "__EFMigrationsHistory"
rename column "MigrationId" to migration_id;

alter table "__EFMigrationsHistory"
rename COLUMN "ProductVersion" to product_version

*/

namespace coach_bags_selenium.Migrations
{
    public partial class SnakeCase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "products");

            migrationBuilder.RenameColumn(
                name: "Savings",
                table: "products",
                newName: "savings");

            migrationBuilder.RenameColumn(
                name: "Price",
                table: "products",
                newName: "price");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "products",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Link",
                table: "products",
                newName: "link");

            migrationBuilder.RenameColumn(
                name: "Image",
                table: "products",
                newName: "image");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "products",
                newName: "category");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "products",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "SalePrice",
                table: "products",
                newName: "sale_price");

            migrationBuilder.RenameColumn(
                name: "LastUpdatedUtc",
                table: "products",
                newName: "last_updated_utc");

            migrationBuilder.RenameColumn(
                name: "LastPostedUtc",
                table: "products",
                newName: "last_posted_utc");

            migrationBuilder.RenameColumn(
                name: "CreatedUtc",
                table: "products",
                newName: "created_utc");

            migrationBuilder.AddPrimaryKey(
                name: "pk_products",
                table: "products",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_products",
                table: "products");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "Products");

            migrationBuilder.RenameColumn(
                name: "savings",
                table: "Products",
                newName: "Savings");

            migrationBuilder.RenameColumn(
                name: "price",
                table: "Products",
                newName: "Price");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Products",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "link",
                table: "Products",
                newName: "Link");

            migrationBuilder.RenameColumn(
                name: "image",
                table: "Products",
                newName: "Image");

            migrationBuilder.RenameColumn(
                name: "category",
                table: "Products",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Products",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "sale_price",
                table: "Products",
                newName: "SalePrice");

            migrationBuilder.RenameColumn(
                name: "last_updated_utc",
                table: "Products",
                newName: "LastUpdatedUtc");

            migrationBuilder.RenameColumn(
                name: "last_posted_utc",
                table: "Products",
                newName: "LastPostedUtc");

            migrationBuilder.RenameColumn(
                name: "created_utc",
                table: "Products",
                newName: "CreatedUtc");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");
        }
    }
}
