using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WareHouse.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RackTable",
                columns: table => new
                {
                    RackId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RackName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RackDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RackX = table.Column<int>(type: "int", nullable: false),
                    RackY = table.Column<int>(type: "int", nullable: false),
                    TotalPlaces = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RackTable", x => x.RackId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RackTable");
        }
    }
}
