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
    public interface IVesselUserRepository : IRepository<VesselUser>
    {
        Task<IEnumerable<VesselUser>> GetVesselUserAsync();
        Task<bool> AssignUserToVesselAsync(int vesselId, string userId);
        Task RemoveUserFromVesselAsync(int vesselName);
    }
}
