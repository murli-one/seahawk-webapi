using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class New_Tbl_SamplingKit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SamplingKit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FuelQualityTestKit = table.Column<int>(type: "int", nullable: true),
                    PowerPlantKit = table.Column<int>(type: "int", nullable: true),
                    OilConditionMonitoringKit = table.Column<int>(type: "int", nullable: true),
                    FiveLitresCubitainers = table.Column<int>(type: "int", nullable: true),
                    TenLitresCubitainers = table.Column<int>(type: "int", nullable: true),
                    VesselName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IMONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RequestorEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EmailCC = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VPSCustomerName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PONumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Street = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HouseNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AdditionalAddressInfo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonToContact = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PersonToContactTelNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryEmail = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingCompanyName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddressLine3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingPostalCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeliveryDeadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Country = table.Column<int>(type: "int", nullable: false),
                    BillingCountry = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SamplingKit", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SamplingKit");
        }
    }
}
