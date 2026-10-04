using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Shop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addedCoatStyleOptions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ArmType",
                table: "Measurements",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ElbowPatch",
                table: "Measurements",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "PatchPocket",
                table: "Measurements",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ArmType",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "ElbowPatch",
                table: "Measurements");

            migrationBuilder.DropColumn(
                name: "PatchPocket",
                table: "Measurements");
        }
    }
}
