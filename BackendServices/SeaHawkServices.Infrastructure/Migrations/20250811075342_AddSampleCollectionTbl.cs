using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSampleCollectionTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IMONumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoOfSamples = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgentEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailCC1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailCC2 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EmailCC3 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TelephoneNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Country = table.Column<int>(type: "int", nullable: false),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PickupAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PreferredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreferredTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedByUser = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AWBNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastUpdated = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EstimatedDeliveryDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipmentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShippingDate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsigneeName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Pieces = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipmentDesc = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsigneeCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsigneePostalCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsigneeCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DuplicateWaybill = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TrackingNumberUniqueIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDetailCreationTime = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDetailCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDetailDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDetailLocationCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDetailLocationCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDetailLocationCountryName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StatusDetailLocationResidential = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherIdentifiersType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OtherIdentifiersValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageWeightUnits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageWeightValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageDimensionsLength = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageDimensionsWidth = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageDimensionsHeight = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageDimensionsUnits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipmentWeightUnits = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipmentWeightValue = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackagingType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PackageCount = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialHandlingsType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialHandlingsDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SpecialHandlingsPaymentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperStateCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperResidential = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsigneeStateCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsigneeCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ConsigneeResidential = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualDeliveryCity = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualDeliveryCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualDeliveryCountry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ActualDeliveryResidential = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryLocationDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryAttempts = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliverySignatureName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginServiceAreaCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OriginServiceAreaDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationServiceAreaCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DestinationServiceAreaDescription = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GlobalProductCode = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DlvyNotificationFlag = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ShipperReferenceID = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WeightUnit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DeliveryDate = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleCollection_AspNetUsers_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SampleCollection_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SampleCollection_CompanyId",
                table: "SampleCollection",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_SampleCollection_CreatedUserId",
                table: "SampleCollection",
                column: "CreatedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleCollection");
        }
    }
}
