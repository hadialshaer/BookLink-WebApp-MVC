using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BookLink.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationsTableAndAddedPopulatedToDb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Locations", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Locations",
                columns: new[] { "Id", "Address", "Name" },
                values: new object[,]
                {
                    { 1, "", "Beirut" },
                    { 2, "", "South" },
                    { 3, "", "North" },
                    { 4, "", "Sidon" },
                    { 5, "", "Tyre" },
                    { 6, "", "Tripoli" },
                    { 7, "", "Zahle" },
                    { 8, "", "Baalbek" },
                    { 9, "", "Nabatiye" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
