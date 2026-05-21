
using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkService.Application.Contract
{
    public interface IFedExTrackingService
    {
        Task<TrackingResult> TrackAsync(string awbNumber);
    }
}
