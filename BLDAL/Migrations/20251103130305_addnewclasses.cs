using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BLDAL.Migrations
{
    /// <inheritdoc />
    public partial class addnewclasses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "rkg");

            migrationBuilder.CreateTable(
                name: "Ranking",
                schema: "rkg",
                columns: table => new
                {
                    RankID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Time = table.Column<TimeOnly>(type: "time", nullable: false),
                    PlayedAtDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranking", x => x.RankID);
                    table.ForeignKey(
                        name: "FK_Ranking_Users_UserID",
                        column: x => x.UserID,
                        principalSchema: "usr",
                        principalTable: "Users",
                        principalColumn: "UserID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Ranking_UserID",
                schema: "rkg",
                table: "Ranking",
                column: "UserID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ranking",
                schema: "rkg");
        }
    }
}
