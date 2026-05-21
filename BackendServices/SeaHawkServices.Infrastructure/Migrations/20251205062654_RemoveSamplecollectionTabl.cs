using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSamplecollectionTabl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SampleCollection");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SampleCollection",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    CreatedUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    AWBNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualDeliveryCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualDeliveryCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualDeliveryCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ActualDeliveryResidential = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AgentEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsigneeCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsigneeCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsigneeCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsigneeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsigneePostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsigneeResidential = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConsigneeStateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ContactPerson = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CurrentStatus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryAttempts = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryLocationDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliverySignatureName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationServiceAreaCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DestinationServiceAreaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DlvyNotificationFlag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DuplicateWaybill = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCC1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCC2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCC3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EstimatedDeliveryDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    GlobalProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NoOfSamples = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginServiceAreaCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OriginServiceAreaDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherIdentifiersType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OtherIdentifiersValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageCount = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageDimensionsHeight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageDimensionsLength = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageDimensionsUnits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageDimensionsWidth = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageWeightUnits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackageWeightValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PackagingType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PickupAddress = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Pieces = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PreferredDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PreferredTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipmentDesc = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipmentWeightUnits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipmentWeightValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperCountry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperReferenceID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperResidential = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShipperStateCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ShippingDate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialHandlingsDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialHandlingsPaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SpecialHandlingsType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetailCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetailCreationTime = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetailDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetailLocationCity = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetailLocationCountryCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetailLocationCountryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StatusDetailLocationResidential = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TelephoneNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Town = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrackingNumberUniqueIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedByUser = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Weight = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WeightUnit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SampleCollection", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SampleCollection_AspNetUsers_CreatedUserId",
                        column: x => x.CreatedUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SampleCollection_Company_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
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
    }
}
