using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface ISampleTrackingRepository
    {
        Task SaveTrackingResultAsync(string awb, string courier, string requestXml, string responseXml, TrackingResult result, CancellationToken ct);
    }
}
