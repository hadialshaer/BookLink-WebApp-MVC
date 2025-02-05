using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLink.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddBorrowerRelationShip : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
