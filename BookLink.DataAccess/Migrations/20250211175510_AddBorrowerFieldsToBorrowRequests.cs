using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLink.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowerFieldsToBorrowRequests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BorroweName",
                table: "BorrowRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BorrowerEmail",
                table: "BorrowRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "LocationId",
                table: "BorrowRequests",
                type: "int",
                nullable: true
                );

            migrationBuilder.AddColumn<string>(
                name: "Phone",
                table: "BorrowRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_LocationId",
                table: "BorrowRequests",
                column: "LocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_Locations_LocationId",
                table: "BorrowRequests",
                column: "LocationId",
                principalTable: "Locations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_Locations_LocationId",
                table: "BorrowRequests");

            migrationBuilder.DropIndex(
                name: "IX_BorrowRequests_LocationId",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "BorroweName",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "BorrowerEmail",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "LocationId",
                table: "BorrowRequests");

            migrationBuilder.DropColumn(
                name: "Phone",
                table: "BorrowRequests");
        }
    }
}
