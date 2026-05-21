using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface IDhlTrackingClient
    {
        Task<string> GetKnownTrackingAsync(string awb, CancellationToken ct);
    }
}
