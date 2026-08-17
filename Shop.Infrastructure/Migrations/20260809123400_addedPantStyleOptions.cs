using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedPantStyleOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BackPocketType",
                table: "Measurements",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FrontPocketType",
                table: "Measurements",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "HemType",
                table: "Measurements",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RiseType",
                table: "Measurements",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "TicketPocket",
                table: "Measurements",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BackPocketType",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "FrontPocketType",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "HemType",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "RiseType",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "TicketPocket",
                table: "Measurements");
        }
    }
}
