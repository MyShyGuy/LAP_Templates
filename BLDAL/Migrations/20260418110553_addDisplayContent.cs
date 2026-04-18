using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BLDAL.Migrations
{
    /// <inheritdoc />
    public partial class addDisplayContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "disp");

            migrationBuilder.CreateTable(
                name: "Advertisements",
                schema: "disp",
                columns: table => new
                {
                    AdID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    ImagePath = table.Column<string>(type: "nvarchar(260)", maxLength: 260, nullable: false),
                    DurationSeconds = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Advertisements", x => x.AdID);
                });

            migrationBuilder.CreateTable(
                name: "BarActions",
                schema: "disp",
                columns: table => new
                {
                    ActionID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    StartsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndsAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarActions", x => x.ActionID);
                });

            migrationBuilder.CreateTable(
                name: "BarMenuItems",
                schema: "disp",
                columns: table => new
                {
                    MenuItemID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(320)", maxLength: 320, nullable: true),
                    Price = table.Column<decimal>(type: "decimal(10,2)", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BarMenuItems", x => x.MenuItemID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Advertisements_DisplayOrder",
                schema: "disp",
                table: "Advertisements",
                column: "DisplayOrder");

            migrationBuilder.CreateIndex(
                name: "IX_BarActions_StartsAt_EndsAt",
                schema: "disp",
                table: "BarActions",
                columns: new[] { "StartsAt", "EndsAt" });

            migrationBuilder.CreateIndex(
                name: "IX_BarMenuItems_DisplayOrder",
                schema: "disp",
                table: "BarMenuItems",
                column: "DisplayOrder");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Advertisements",
                schema: "disp");

            migrationBuilder.DropTable(
                name: "BarActions",
                schema: "disp");

            migrationBuilder.DropTable(
                name: "BarMenuItems",
                schema: "disp");
        }
    }
}
