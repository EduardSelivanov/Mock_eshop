using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotsService.Migrations
{
    /// <inheritdoc />
    public partial class addBooked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "SlotsTable",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "SlotsTable");
        }
    }
}
