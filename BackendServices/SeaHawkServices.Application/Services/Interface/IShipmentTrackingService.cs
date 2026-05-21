using Data;
using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IShipmentTrackingService
    {
        Task<TrackingResult> TrackAsync(CourierProvider courier, string awb, CancellationToken ct);
    }
}
