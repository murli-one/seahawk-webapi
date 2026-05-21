using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Philip_GetResidualSampleNumberDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE [dbo].[Philip_GetResidualSampleNumberDetails]
    @SampleNumber NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        -- text
        SampleNumber,
        CASE WHEN Laboratory <> '' THEN Laboratory ELSE 'Seahawk Services' END AS Laboratory,
        VesselName,
        PortBunkered,
        BunkerTanker,
        SampleLocation,
        Supplier,
        Grade,
        FuelType,
        SealNumber,
        Comment,
        [FileName],

        -- ids / dates / supplier
        LID,
        DateBunkered,
        DateReceived,
        SampleReportDate,
        SupplierDensity,
        SupplierViscosity,
        SupplierSulphur,
        ASSESS,

        -- results (numerics + *Ass)
        cstAt50,            cstAt50Ass,
        Density,            DensityAss,
        CCAI,               CCAIAss,
        Sulphur,            SulphurAss,
        FlashPoint,         FlashPointAss,
        H2S,                H2SAss,
        TotalAcid,          TotalAcidAss,
        TSE,                TSEAss,
        MCR,                MCRAss,
        PourPointSummStd,   PourPointAss,
        Water,              WaterAss,
        Ash,                AshAss,
        Sodium,             SodiumAss,
        Vanadium,           VanadiumAss,
        AlSi,               AlSiAss,
        ULO,                ULOAss,
        Zn,                 ZnAss,
        P,                  PAss,
        Ca,                 CaAss
    FROM dbo.MSCAnalysis
    WHERE SampleNumber = @SampleNumber;
END
");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
