using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BLDAL.Migrations
{
    /// <inheritdoc />
    public partial class addnewclassesss : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                schema: "rkg",
                table: "Ranking",
                newName: "time");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "time",
                schema: "rkg",
                table: "Ranking",
                newName: "Time");
        }
    }
}
