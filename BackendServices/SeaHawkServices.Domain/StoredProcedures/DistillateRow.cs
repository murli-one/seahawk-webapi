using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.StoredProcedures
{
    [Keyless]
    public  class DistillateRow
    {
        public string? SampleNumber { get; set; }
        public string? Laboratory { get; set; }
        public string? VesselName { get; set; }
        public string? PortBunkered { get; set; }
        public string? BunkerTanker { get; set; }
        public string? SampleLocation { get; set; }
        public string? Supplier { get; set; }
        public string? Grade { get; set; }
        public string? FuelType { get; set; }
        public string? SealNumber { get; set; }
        public string? Comment { get; set; }
        public string? FTIR { get; set; }
        public string? Appearance { get; set; }
        public string? FileName { get; set; }
        public decimal? LID { get; set; }
        public DateTime? DateBunkered { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? SampleReportDate { get; set; }

        public decimal? SupplierDensity { get; set; }
        public decimal? SupplierViscosity { get; set; }
        public decimal? SupplierSulfurPPM { get; set; }
        public decimal? ASSESS { get; set; }

        public decimal? Ash { get; set; }
        public decimal? AshAss { get; set; }
        public decimal? FlashPoint { get; set; }
        public decimal? FlashPointAss { get; set; }
        public decimal? FTIRAss { get; set; }
        public decimal? cstAt40 { get; set; }
        public decimal? cstAr40Ass { get; set; }
        public decimal? Density { get; set; }
        public decimal? DensityAss { get; set; }
        public decimal? Cetane { get; set; }
        public decimal? CetaneAss { get; set; }
        public decimal? SulphurPPM { get; set; }
        public decimal? SulphurPPMAss { get; set; }
        public decimal? H2S { get; set; }
        public decimal? H2SAss { get; set; }
        public decimal? TotalAcid { get; set; }
        public decimal? TotalAcidAss { get; set; }
        public decimal? ParticulateContamination { get; set; }
        public decimal? ParticulateAss { get; set; }
        public decimal? OxidationStability { get; set; }
        public decimal? OxidationStabilityAss { get; set; }
        public decimal? MCR { get; set; }
        public decimal? MCRAss { get; set; }
        public decimal? CloudPoint { get; set; }
        public decimal? CloudPointAss { get; set; }
        public decimal? PourPointSummStd { get; set; }
        public decimal? PourPointAss { get; set; }
        public decimal? AppearanceAss { get; set; }
        public decimal? Water { get; set; }
        public decimal? WaterAss { get; set; }
        public decimal? Lubricity { get; set; }
        public decimal? LubricityAss { get; set; }
    }

}
