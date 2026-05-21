using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AltercareerTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Career");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Career",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Career");

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Career",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
