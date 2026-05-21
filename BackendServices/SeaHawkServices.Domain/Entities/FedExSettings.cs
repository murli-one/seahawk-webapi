using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Domain.Entities
{
    #region OldApproach 
    //public sealed class FedExSettings
    //{
    //    public string TrackURL { get; set; } = string.Empty;
    //    public string ParentKey { get; set; } = string.Empty;
    //    public string ParentPassword { get; set; } = string.Empty;
    //    public string UserKey { get; set; } = string.Empty;
    //    public string UserPassword { get; set; } = string.Empty;
    //    public string AccountNumber { get; set; } = string.Empty;
    //    public string MeterNumber { get; set; } = string.Empty;
    //}
    #endregion

    public sealed class FedExSettings
    {
        public string OAuthBaseUrl { get; set; }   // e.g. https://apis.fedex.com
        public string ApiKey { get; set; }         // your FedEx API Key
        public string ApiSecret { get; set; }      // your FedEx API Secret

        // Optional but good to keep configurable
        public string TrackUrl { get; set; }       // e.g. https://apis.fedex.com/track/v1/trackingnumbers

        // Keep these if you need them later (labels, shipping, etc.)
        public string AccountNumber { get; set; }
        public string MeterNumber { get; set; }
    }
}
