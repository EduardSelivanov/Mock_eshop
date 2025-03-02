using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SlotsService.Migrations
{
    /// <inheritdoc />
    public partial class addedBookedTill : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "BookedTill",
                table: "SlotsTable",
                type: "date",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BookedTill",
                table: "SlotsTable");
        }
    }
}
