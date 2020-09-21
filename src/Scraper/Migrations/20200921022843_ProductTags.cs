using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace coach_bags_selenium.Migrations
{
    public partial class ProductTags : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "tags",
                table: "products",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "tags",
                table: "products");
        }
    }
}
