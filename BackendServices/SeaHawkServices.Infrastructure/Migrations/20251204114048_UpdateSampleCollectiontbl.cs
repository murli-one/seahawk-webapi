using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSampleCollectiontbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FedExErrorMessage",
                table: "SampleCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FedExRequestJson",
                table: "SampleCollections",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                table: "SampleCollections",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FedExErrorMessage",
                table: "SampleCollections");

            migrationBuilder.DropColumn(
                name: "FedExRequestJson",
                table: "SampleCollections");

            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "SampleCollections");
        }
    }
}
