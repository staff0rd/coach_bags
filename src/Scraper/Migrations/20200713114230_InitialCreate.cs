using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace coach_bags_selenium.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Link = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    SalePrice = table.Column<decimal>(nullable: false),
                    Price = table.Column<decimal>(nullable: false),
                    Savings = table.Column<decimal>(nullable: false),
                    LastPostedUtc = table.Column<DateTime>(nullable: true),
                    LastUpdatedUtc = table.Column<DateTime>(nullable: true),
                    CreatedUtc = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
