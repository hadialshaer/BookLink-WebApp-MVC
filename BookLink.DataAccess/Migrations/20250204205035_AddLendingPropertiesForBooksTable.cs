using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLink.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddLendingPropertiesForBooksTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BookStatus",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BorrowerId",
                table: "Books",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DueDate",
                table: "Books",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxLendDurationDays",
                table: "Books",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TransactionType",
                table: "Books",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 1,
                columns: new[] { "BookStatus", "BorrowerId", "DueDate", "MaxLendDurationDays", "TransactionType" },
                values: new object[] { null, null, null, null, 0 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 2,
                columns: new[] { "BookStatus", "BorrowerId", "DueDate", "MaxLendDurationDays", "TransactionType" },
                values: new object[] { null, null, null, null, 0 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 3,
                columns: new[] { "BookStatus", "BorrowerId", "DueDate", "MaxLendDurationDays", "TransactionType" },
                values: new object[] { null, null, null, null, 0 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 4,
                columns: new[] { "BookStatus", "BorrowerId", "DueDate", "MaxLendDurationDays", "TransactionType" },
                values: new object[] { null, null, null, null, 0 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 5,
                columns: new[] { "BookStatus", "BorrowerId", "DueDate", "MaxLendDurationDays", "TransactionType" },
                values: new object[] { null, null, null, null, 0 });

            migrationBuilder.UpdateData(
                table: "Books",
                keyColumn: "BookId",
                keyValue: 6,
                columns: new[] { "BookStatus", "BorrowerId", "DueDate", "MaxLendDurationDays", "TransactionType" },
                values: new object[] { null, null, null, null, 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Books_BorrowerId",
                table: "Books",
                column: "BorrowerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books",
                column: "BorrowerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_BorrowerId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_BorrowerId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BookStatus",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "BorrowerId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "DueDate",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "MaxLendDurationDays",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "TransactionType",
                table: "Books");
        }
    }
}
