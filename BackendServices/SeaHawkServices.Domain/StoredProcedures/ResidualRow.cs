using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.StoredProcedures
{
    [Keyless]
    public class ResidualRow
    {
        // text
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
        public string? FileName { get; set; }

        // ids/dates
        public decimal? LID { get; set; }
        public DateTime? DateBunkered { get; set; }
        public DateTime? DateReceived { get; set; }
        public DateTime? SampleReportDate { get; set; }

        // supplier
        public decimal? SupplierDensity { get; set; }
        public decimal? SupplierViscosity { get; set; }
        public decimal? SupplierSulphur { get; set; }
        public decimal? ASSESS { get; set; }

        // results
        public decimal? cstAt50 { get; set; }
        public decimal? cstAt50Ass { get; set; }
        public decimal? Density { get; set; }
        public decimal? DensityAss { get; set; }
        public decimal? CCAI { get; set; }
        public decimal? CCAIAss { get; set; }
        public decimal? Sulphur { get; set; }
        public decimal? SulphurAss { get; set; }
        public decimal? FlashPoint { get; set; }
        public decimal? FlashPointAss { get; set; }
        public decimal? H2S { get; set; }
        public decimal? H2SAss { get; set; }
        public decimal? TotalAcid { get; set; }
        public decimal? TotalAcidAss { get; set; }
        public decimal? TSE { get; set; }
        public decimal? TSEAss { get; set; }
        public decimal? MCR { get; set; }
        public decimal? MCRAss { get; set; }
        public decimal? PourPointSummStd { get; set; }
        public decimal? PourPointAss { get; set; }
        public decimal? Water { get; set; }
        public decimal? WaterAss { get; set; }
        public decimal? Ash { get; set; }
        public decimal? AshAss { get; set; }
        public decimal? Sodium { get; set; }
        public decimal? SodiumAss { get; set; }
        public decimal? Vanadium { get; set; }
        public decimal? VanadiumAss { get; set; }
        public decimal? AlSi { get; set; }
        public decimal? AlSiAss { get; set; }
        public decimal? ULO { get; set; }
        public decimal? ULOAss { get; set; }
        public decimal? Zn { get; set; }
        public decimal? ZnAss { get; set; }
        public decimal? P { get; set; }
        public decimal? PAss { get; set; }
        public decimal? Ca { get; set; }
        public decimal? CaAss { get; set; }
    }

}
