using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeaHawkServices.Domain.Entities
{
    public class MSCAnalysis
    {
        /* int Params */
        public int Id { get; set; }
        /* bool Params */
        public bool? FuelMixWithFuelOnboard { get; set; }
        /* DateTime Params */
        public DateTime? DateBunkered { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? SampleSentOn { get; set; }
        public DateTime? SampleReportDate { get; set; }

        /* string Params */
        public string SampleNumber { get; set; }
        public string? Confirmation { get; set; }
        public string? OffSpec1 { get; set; }
        public string? OffSpec2 { get; set; }
        public string? OffSpec3 { get; set; }
        public string? OffSpec4 { get; set; }
        public string? QuantityDiff { get; set; }
        public string? Appearance { get; set; }
        public string? Stability { get; set; }
        public string? Neutrality { get; set; }
        public string? Grade { get; set; }
        public string? PortBunkered { get; set; }
        public string? SampleLocation { get; set; }
        public string? SealNumber { get; set; }
        public string? Supplier { get; set; }
        public string? Laboratory { get; set; }
        public string? Technician { get; set; }
        public string? FuelType { get; set; }
        public string? VesselName { get; set; }
        public string? Initials { get; set; }
        public string? AirwayNumber { get; set; }
        public string? SealNumberSupplier { get; set; }
        public string? SealNumberRetained { get; set; }
        public string? SealNumberMARPOL { get; set; }
        public string? SampleSentFrom { get; set; }
        public string? TankNumber { get; set; }
        public string? SampleMethod { get; set; }
        public string? FTIR { get; set; }
        public string? Demulsification { get; set; }
        public string? ClientRefNumber { get; set; }
        public string? BunkerTanker { get; set; }
        public string? FuelTypeID { get; set; }
        public string? TestType { get; set; }
        public string? OrderID { get; set; }
        public string? Comment { get; set; }
        public string? Specification { get; set; }
        public string? Comment1 { get; set; }
        public string? Comment2 { get; set; }
        public string? Comment3 { get; set; }
        public string? Comment4 { get; set; }
        public string? Comment5 { get; set; }
        public string? Comment6 { get; set; }
        public string? FileName { get; set; }

        /* decimal Params */

        [Column(TypeName = "decimal(18,2)")]
        public decimal? Density { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DensityAss { get; set; }
        [Column(TypeName = "decimal(3,2)")]
        public decimal? cstAt40 { get; set; }
        public decimal? cstAr40Ass { get; set; }
        [Column(TypeName = "decimal(4,1)")]
        public decimal? cstAt50 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? cstAt50Ass { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? cstAt80 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? cstAt100 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? VISC { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? VISCTEMP { get; set; }
        // decimal(5,2)
        [Column(TypeName = "decimal(5,2)")]
        public decimal? FlashPoint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FlashPointAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PourPointSummStd { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PourPointAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CloudPoint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CloudPointAss { get; set; }
        [Column(TypeName = "decimal(5,3)")]
        public decimal? MCR { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MCRAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Ash { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AshAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Water { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? WaterAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Sulphur { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SulphurAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SulphurPPM { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SulphurPPMAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Compatibility { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Vanadium { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? VanadiumAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Sodium { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SodiumAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Aluminum { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Silicon { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AlSi { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AlSiAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ULO { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ULOAss { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal? TSP { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal? TSE { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TSEAss { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal? TSA { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetCalVal { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CCAI { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CCAIAss { get; set; }

        [Column(TypeName = "decimal(4,2)")]
        public decimal? Asphaltenes { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Pb { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Zn { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ZnAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Ca { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CaAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Mg { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Ni { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? K { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Li { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Fe { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cr { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Ant { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AASS { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Hg { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Be { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Co { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cu { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Se { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cd { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? C { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? H { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? N { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BSW { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? IBP { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? pct10 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? pct20 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? pct30 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? pct40 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? pct50 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? pct90 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? EP { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Cetane { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CetaneAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? AppearanceAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Sediment { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BurningTemp { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAcid { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalAcidAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? StrongAcid { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? GrossHeatComb { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetHeatComb { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? BunkerReceipt { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SupplierDensity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SupplierViscosity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PourPointWint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Halogens { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? H2S { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? H2SAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? P { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? PAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Color { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ColdFilterPluggingPoint { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MineralAcidity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? OxidationStability { get; set; }
        [Column(TypeName = "decimal(18,2)")]

        public decimal? OxidationStabilityAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? CopperCorrosion { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public decimal? MCR10 { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FTIRAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? WaterKarlFischer { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ShippingCharges { get; set; }
        [Column(TypeName = "decimal(4,2)")]
        public decimal? ParticulateContamination { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ParticulateAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? FreeGlycerin { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalGlycerin { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Lubricity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? LubricityAss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DieselRangeOrganics { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? APIGravity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? DistResLoss { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InjTemp10cst { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InjTemp13cst { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InjTemp15cst { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? MPT100cst { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NaVRatio { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NaK { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SupplierSulphur { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? NetSpecificEnergy { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? Polymer { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? TotalDeposits { get; set; }
      
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SupplierSulfur { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SSQuantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InjTemp12cst { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InjTemp14cst { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InjTemp20cst { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? InjTemp25cst { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? SupplierSulfurPPM { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal? ASSESS { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal? LID { get; set; }
       
    }
}
