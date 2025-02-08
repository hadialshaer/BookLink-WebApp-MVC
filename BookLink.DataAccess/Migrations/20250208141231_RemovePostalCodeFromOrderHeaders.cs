using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BookLink.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemovePostalCodeFromOrderHeaders : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostalCode",
                table: "orderHeaders");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PostalCode",
                table: "orderHeaders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
