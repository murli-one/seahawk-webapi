using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SeaHawkServices.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedData_Tbl_MSCAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
INSERT INTO [dbo].[MSCAnalysis] (
    [SampleNumber],[Density],[DensityAss],[cstAt40],[cstAr40Ass],[cstAt50],[cstAt50Ass],[cstAt80],[cstAt100],
    [VISC],[VISCTEMP],[FlashPoint],[FlashPointAss],[PourPointSummStd],[PourPointAss],[CloudPoint],[CloudPointAss],
    [MCR],[MCRAss],[Ash],[AshAss],[Water],[WaterAss],[Sulphur],[SulphurAss],[SulphurPPM],[SulphurPPMAss],
    [Compatibility],[Vanadium],[VanadiumAss],[Sodium],[SodiumAss],[Aluminum],[Silicon],[AlSi],[AlSiAss],[ULO],[ULOAss],
    [TSP],[TSE],[TSEAss],[TSA],[NetCalVal],[CCAI],[CCAIAss],[Asphaltenes],[Pb],[Zn],[ZnAss],[Ca],[CaAss],[Mg],[Ni],[K],
    [Li],[Fe],[Confirmation],[Cr],[Ant],[AASS],[Hg],[Be],[Co],[Cu],[Se],[Cd],[C],[H],[N],[BSW],[IBP],[pct10],[pct20],[pct30],
    [pct40],[pct50],[pct90],[EP],[Cetane],[CetaneAss],[Appearance],[AppearanceAss],[Sediment],[BurningTemp],[TotalAcid],[TotalAcidAss],
    [StrongAcid],[Stability],[GrossHeatComb],[NetHeatComb],[Neutrality],[Grade],[PortBunkered],[DateBunkered],[SampleLocation],[SealNumber],
    [Supplier],[DateReceived],[Laboratory],[Technician],[FuelType],[BunkerReceipt],[SupplierDensity],[SupplierViscosity],[PourPointWint],
    [VesselName],[Initials],[Halogens],[H2S],[H2SAss],[P],[PAss],[Color],[ColdFilterPluggingPoint],[MineralAcidity],[OxidationStability],
    [OxidationStabilityAss],[CopperCorrosion],[SampleSentOn],[AirwayNumber],[SealNumberSupplier],[SealNumberRetained],[SealNumberMARPOL],
    [FuelMixWithFuelOnboard],[ShippingCharges],[SampleSentFrom],[TankNumber],[SampleReportDate],[SampleMethod],[MCR10],[FTIR],[FTIRAss],
    [WaterKarlFischer],[Demulsification],[ParticulateContamination],[ParticulateAss],[FreeGlycerin],[TotalGlycerin],[Lubricity],[LubricityAss],
    [ClientRefNumber],[DieselRangeOrganics],[APIGravity],[DistResLoss],[InjTemp10cst],[InjTemp13cst],[InjTemp15cst],[MPT100cst],[NaVRatio],[NaK],
    [BunkerTanker],[FuelTypeID],[SupplierSulphur],[TestType],[NetSpecificEnergy],[Polymer],[TotalDeposits],[OrderID],[SupplierSulfur],[SSQuantity],
    [InjTemp12cst],[InjTemp14cst],[Comment],[Specification],[InjTemp20cst],[InjTemp25cst],[OffSpec1],[OffSpec2],[OffSpec3],[OffSpec4],
    [QuantityDiff],[SupplierSulfurPPM],[Comment1],[Comment2],[Comment3],[Comment4],[Comment5],[Comment6],[ASSESS],[LID],[FileName]
    )
    SELECT
        [SampleNumber],[Density],[DensityAss],[cstAt40],[cstAr40Ass],[cstAt50],[cstAt50Ass],[cstAt80],[cstAt100],
        [VISC],[VISCTEMP],[FlashPoint],[FlashPointAss],[PourPointSummStd],[PourPointAss],[CloudPoint],[CloudPointAss],
        [MCR],[MCRAss],[Ash],[AshAss],[Water],[WaterAss],[Sulphur],[SulphurAss],[SulphurPPM],[SulphurPPMAss],
        [Compatibility],[Vanadium],[VanadiumAss],[Sodium],[SodiumAss],[Aluminum],[Silicon],[AlSi],[AlSiAss],[ULO],[ULOAss],
        [TSP],[TSE],[TSEAss],[TSA],[NetCalVal],[CCAI],[CCAIAss],[Asphaltenes],[Pb],[Zn],[ZnAss],[Ca],[CaAss],[Mg],[Ni],[K],
        [Li],[Fe],[Confirmation],[Cr],[Ant],[AASS],[Hg],[Be],[Co],[Cu],[Se],[Cd],[C],[H],[N],[BSW],[IBP],[pct10],[pct20],[pct30],
        [pct40],[pct50],[pct90],[EP],[Cetane],[CetaneAss],[Appearance],[AppearanceAss],[Sediment],[BurningTemp],[TotalAcid],[TotalAcidAss],
        [StrongAcid],[Stability],[GrossHeatComb],[NetHeatComb],[Neutrality],[Grade],[PortBunkered],[DateBunkered],[SampleLocation],[SealNumber],
        [Supplier],[DateReceived],[Laboratory],[Technician],[FuelType],[BunkerReceipt],[SupplierDensity],[SupplierViscosity],[PourPointWint],
        [VesselName],[Initials],[Halogens],[H2S],[H2SAss],[P],[PAss],[Color],[ColdFilterPluggingPoint],[MineralAcidity],[OxidationStability],
        [OxidationStabilityAss],[CopperCorrosion],[SampleSentOn],[AirwayNumber],[SealNumberSupplier],[SealNumberRetained],[SealNumberMARPOL],
        [FuelMixWithFuelOnboard],[ShippingCharges],[SampleSentFrom],[TankNumber],[SampleReportDate],[SampleMethod],[MCR10],[FTIR],[FTIRAss],
        [WaterKarlFischer],[Demulsification],[ParticulateContamination],[ParticulateAss],[FreeGlycerin],[TotalGlycerin],[Lubricity],[LubricityAss],
        [ClientRefNumber],[DieselRangeOrganics],[APIGravity],[DistResLoss],[InjTemp10cst],[InjTemp13cst],[InjTemp15cst],[MPT100cst],[NaVRatio],[NaK],
        [BunkerTanker],[FuelTypeID],[SupplierSulphur],[TestType],[NetSpecificEnergy],[Polymer],[TotalDeposits],[OrderID],[SupplierSulfur],[SSQuantity],
        [InjTemp12cst],[InjTemp14cst],[Comment],[Specification],[InjTemp20cst],[InjTemp25cst],[OffSpec1],[OffSpec2],[OffSpec3],[OffSpec4],
        [QuantityDiff],[SupplierSulfurPPM],[Comment1],[Comment2],[Comment3],[Comment4],[Comment5],[Comment6],[ASSESS],[LID],[FileName]
    FROM [SS].[dbo].[MSCAnalysis] WITH (NOLOCK);

    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
