using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Add_Philip_GetDistillateSampleNumberDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE OR ALTER PROCEDURE [dbo].[Philip_GetDistillateSampleNumberDetails]
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
        FTIR,           -- string in table
        Appearance,     -- string in table
        [FileName],

        -- numbers & dates (native types)
        LID,
        DateBunkered,
        DateReceived,
        SampleReportDate,

        SupplierDensity,
        SupplierViscosity,
        SupplierSulfurPPM,
        ASSESS,

        -- results (numerics + *Ass)
        Ash,                 AshAss,
        FlashPoint,          FlashPointAss,
        FTIRAss,
        cstAt40,             cstAr40Ass,
        Density,             DensityAss,
        Cetane,              CetaneAss,
        SulphurPPM,          SulphurPPMAss,
        H2S,                 H2SAss,
        TotalAcid,           TotalAcidAss,
        ParticulateContamination, ParticulateAss,
        OxidationStability,  OxidationStabilityAss,
        MCR,                 MCRAss,
        CloudPoint,          CloudPointAss,
        PourPointSummStd,    PourPointAss,
        AppearanceAss,
        Water,               WaterAss,
        Lubricity,           LubricityAss
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
