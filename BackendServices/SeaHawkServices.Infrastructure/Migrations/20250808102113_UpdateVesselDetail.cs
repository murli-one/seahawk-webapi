using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVesselDetail : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BillTo",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Built",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ByEMail",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ByFax",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ByTelex",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CallSign",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Charterer",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Class",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CommissionType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DOReportType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Diesel",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Draft",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Dwt",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ExVesselName",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FOReportType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaxArea",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaxCountry",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FaxNumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Filter",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FuelSystem",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "GeneratorType",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HFO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "HFOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IFO",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "IFOGrade",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Owner",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Propulsion",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Purifier",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Registry",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TlxNumber",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "VesselDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BillTo",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Built",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "ByEMail",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "ByFax",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "ByTelex",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "CallSign",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Charterer",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Class",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "CommissionType",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "DOGrade",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "DOReportType",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Diesel",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Draft",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Dwt",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "ExVesselName",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "FOReportType",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "FaxArea",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "FaxCountry",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "FaxNumber",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Filter",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "FuelSystem",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "GO",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "GOGrade",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "GeneratorType",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "HFO",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "HFOGrade",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "IFO",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "IFOGrade",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Owner",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Propulsion",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Purifier",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Registry",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "TlxNumber",
                table: "VesselDetails");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "VesselDetails");
        }
    }
}
