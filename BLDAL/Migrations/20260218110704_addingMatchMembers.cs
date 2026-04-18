using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BLDAL.Migrations
{
    /// <inheritdoc />
    public partial class addingMatchMembers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Ranking",
                schema: "rkg");

            migrationBuilder.EnsureSchema(
                name: "match");

            migrationBuilder.CreateTable(
                name: "MatchMembers",
                schema: "match",
                columns: table => new
                {
                    MemberID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchMembers", x => x.MemberID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchMembers",
                schema: "match");

            migrationBuilder.EnsureSchema(
                name: "rkg");

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Genre = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Ranking",
                schema: "rkg",
                columns: table => new
                {
                    RankID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    PlayedAtDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    time = table.Column<TimeSpan>(type: "time", nullable: false)
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
    }
}
