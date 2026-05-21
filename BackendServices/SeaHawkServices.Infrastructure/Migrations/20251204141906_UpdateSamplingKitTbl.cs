using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSamplingKitTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FedExErrorMessage",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FedExRequestJson",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestStatus",
                table: "SamplingKit",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FedExErrorMessage",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "FedExRequestJson",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RequestStatus",
                table: "SamplingKit");
        }
    }
}
