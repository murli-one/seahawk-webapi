using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSampleKitTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommodityDescription",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "DeclaredValue",
                table: "SamplingKit",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "NumberOfParcels",
                table: "SamplingKit",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PackageWeightLb",
                table: "SamplingKit",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientAddressLine1",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientAddressLine2",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientAddressLine3",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientCity",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientCompanyName",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientCountry",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientName",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPhone",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientPostalCode",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientState",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "SamplingKit",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommodityDescription",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "DeclaredValue",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "NumberOfParcels",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "PackageWeightLb",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientAddressLine1",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientAddressLine2",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientAddressLine3",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientCity",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientCompanyName",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientCountry",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientName",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientPhone",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientPostalCode",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "RecipientState",
                table: "SamplingKit");

            migrationBuilder.DropColumn(
                name: "State",
                table: "SamplingKit");
        }
    }
}
