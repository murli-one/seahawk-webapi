using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCountryName_SampleCollection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Country",
                table: "SampleCollection");

            migrationBuilder.AddColumn<string>(
                name: "CountryName",
                table: "SampleCollection",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountryName",
                table: "SampleCollection");

            migrationBuilder.AddColumn<int>(
                name: "Country",
                table: "SampleCollection",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
