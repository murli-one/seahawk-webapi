using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface IVesselDetailRepository : IRepository<VesselDetail>
    {
        Task<VesselDetail?> GetByIdAsync(int id);
        Task<VesselDetail?> GetCompanyByVesselDetailIdAsync(int VesselDetailId);
        Task<VesselDetail?> GetCompanyByVesselNameAsync(string VesselName);
        Task UpdateAsync(VesselDetail vesselDetails);

        IQueryable<VesselDetail> Query();

    }
}
