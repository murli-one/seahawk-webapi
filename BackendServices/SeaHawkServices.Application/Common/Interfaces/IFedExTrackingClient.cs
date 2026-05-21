using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface IFedExTrackingClient
    {
        /// <summary>
        /// Calls FedEx Track Web Service and returns raw XML response.
        /// </summary>
        Task<string> TrackAsync(string trackingNumber, CancellationToken ct);
    }
}
