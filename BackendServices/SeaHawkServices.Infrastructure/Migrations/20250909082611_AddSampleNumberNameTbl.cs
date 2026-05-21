using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleNumberNameTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PickupRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfBoxes = table.Column<int>(type: "int", nullable: true),
                    SampleType = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<int>(type: "int", nullable: false),
                    PickupDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonToBeContacted = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonToBeContactTelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupCity = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupRequest", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleType = table.Column<int>(type: "int", nullable: false),
                    Vessel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BoxQuantity = table.Column<int>(type: "int", nullable: false),
                    PickUpDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailCC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<int>(type: "int", nullable: false),
                    Company = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampCollection", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SampleNumberNames",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SampleNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleNumberNames", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupRequest");

            migrationBuilder.DropTable(
                name: "SampCollection");

            migrationBuilder.DropTable(
                name: "SampleNumberNames");
        }
    }
}
