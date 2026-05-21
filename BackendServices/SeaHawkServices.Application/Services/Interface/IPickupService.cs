using SeaHawkServices.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IPickupService
    {
        Task<SamplePickupResultVM> CreateDhlPickupAsync(SamplePickupRequestVM request, CancellationToken ct);
        Task<SamplePickupResultVM> CreateFedExPickupAsync(SamplePickupRequestVM request, CancellationToken ct);
    }
}
