using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePickuprequesttable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupRequest");

            migrationBuilder.CreateTable(
                name: "SampleCollections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NumberOfBoxes = table.Column<int>(type: "int", nullable: false),
                    SampleType = table.Column<int>(type: "int", nullable: false),
                    Country = table.Column<int>(type: "int", nullable: false),
                    PickupDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonToBeContacted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonToBeContactTelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailCC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FedExLabelUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FedExInvoiceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleCollections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleCollections_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleCollections_ApplicationUserId",
                table: "SampleCollections",
                column: "ApplicationUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleCollections");

            migrationBuilder.CreateTable(
                name: "PickupRequest",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<int>(type: "int", nullable: false),
                    CreatedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EmailCC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FedExInvoiceUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FedExLabelUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumberOfBoxes = table.Column<int>(type: "int", nullable: false),
                    PersonToBeContactTelephoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PersonToBeContacted = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SampleType = table.Column<int>(type: "int", nullable: false),
                    TrackingNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PickupRequest_AspNetUsers_ApplicationUserId",
                        column: x => x.ApplicationUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickupRequest_ApplicationUserId",
                table: "PickupRequest",
                column: "ApplicationUserId");
        }
    }
}
