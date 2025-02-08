using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLink.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRelationShips : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_BorrowerId",
                table: "BorrowRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_LenderId",
                table: "BorrowRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_BorrowerId",
                table: "BorrowRequests",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_LenderId",
                table: "BorrowRequests",
                column: "LenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_BorrowerId",
                table: "BorrowRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_LenderId",
                table: "BorrowRequests");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_BorrowerId",
                table: "BorrowRequests",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BorrowRequests_AspNetUsers_LenderId",
                table: "BorrowRequests",
                column: "LenderId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
