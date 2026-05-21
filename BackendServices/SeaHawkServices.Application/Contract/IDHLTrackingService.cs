using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Contract
{
    public interface IDHLTrackingService
    {
        Task<ShipmentTracking?> GetAsync(TrackingId id, CancellationToken ct = default);
    }
}
