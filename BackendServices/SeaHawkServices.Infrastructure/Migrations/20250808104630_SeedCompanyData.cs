using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedCompanyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
        //    migrationBuilder.Sql(@"
        //    INSERT INTO [Company] (
        //        CompanyName,
        //        BillingAddress,
        //        City,
        //        StateOrProvince,
        //        PostalCode,
        //        Country,
        //        PhoneNumber,
        //        FaxNumber,
        //        EmailAddress,
        //        Notes,
        //        ShipOwner,
        //        FuelSupplier,
        //        ContractLab,
        //        Charterer,
        //        InvoiceType,
        //        CompanyKey,
        //        FaxCountry,
        //        FaxArea,
        //        ClientRef
        //    )
        //    SELECT 
        //        ISNULL(CompanyName, ''),
        //        ISNULL(BillingAddress, ''),
        //        ISNULL(City, ''),
        //        ISNULL(StateOrProvince, ''),
        //        ISNULL(PostalCode, ''),
        //        ISNULL(Country, ''),
        //        ISNULL(PhoneNumber, ''),
        //        ISNULL(FaxNumber, ''),
        //        ISNULL(EmailAddress, ''),
        //        ISNULL(Notes, ''),
        //        ISNULL(ShipOwner, ''),
        //        ISNULL(FuelSupplier, ''),
        //        ISNULL(ContractLab, ''),
        //        ISNULL(Charterer, ''),
        //        ISNULL(InvoiceType, ''),
        //        ISNULL(CompanyKey, ''),
        //        ISNULL(FaxCountry, ''),
        //        ISNULL(FaxArea, ''),
        //        ISNULL(ClientRef, '')
        //    FROM [SS].[dbo].[CompaniesList];
        //");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
