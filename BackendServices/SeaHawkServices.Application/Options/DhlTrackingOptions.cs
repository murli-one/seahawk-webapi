using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Options
{
    public sealed class DhlTrackingOptions_Old
    {
        public string TrackURL { get; set; } = default!;
        public string MessageReference { get; set; } = default!;
        public string SiteID { get; set; } = default!;
        public string Password { get; set; } = default!;
        public string LanguageCode { get; set; } = "en";
        public string LevelOfDetails { get; set; } = "ALL_CHECK_POINTS";
        public string PiecesEnabled { get; set; } = "S";
        public int TimeoutSeconds { get; set; } = 300;
    }
    public sealed class DhlTrackingOptions
    {
        public string ApiKey { get; set; } = default!;
        public string ApiSecret { get; set; } = default!;
        public string BaseUrl { get; set; } = default!;
        public string Environment { get; set; } = default!;
        public string SiteID { get; set; } = default!;
        public string Password { get; set; } = default!;
        public int TimeoutSeconds { get; set; } = 300;
    }
}
