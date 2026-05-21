using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.StoredProcedures
{
    [Keyless]
    public class LiveDataRow
    {
        public string? SampleNumber { get; set; }
        public string? Specification { get; set; }
        public string? VesselName { get; set; }
        public string? SampleLocation { get; set; }
        public string? SampleSentOn { get; set; } 
        public string? Port { get; set; }
        public string? FuelType { get; set; }
        public decimal? Density { get; set; }
        public decimal? CstAt50 { get; set; }
        public decimal? Sulphur { get; set; }
        public decimal? PourPointSummStd { get; set; }
        public decimal? FlashPoint { get; set; }
        public decimal? Water { get; set; }
        public decimal? MCR { get; set; }
        public decimal? Aluminum { get; set; }
        public decimal? Silicon { get; set; }
        public decimal? AlSi { get; set; }
        public decimal? Ash { get; set; }
        public decimal? Vanadium { get; set; }
        public decimal? Sodium { get; set; }
        public decimal? TSP { get; set; }
        public decimal? CCAI { get; set; }
        public decimal? Ca { get; set; }
        public decimal? Zn { get; set; }
        public decimal? P { get; set; }
        public decimal? TotalAcid { get; set; }
        public decimal? CloudPoint { get; set; }
        public decimal? Cetane { get; set; }
        public string? Appearance { get; set; }
        public decimal? FTIR { get; set; }   // ✅ FIXED (was string)
        public decimal? NetCalVal { get; set; }
    }

}
