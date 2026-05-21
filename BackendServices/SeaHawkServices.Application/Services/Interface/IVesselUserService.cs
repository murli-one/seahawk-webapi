using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IVesselUserService
    {
        Task<IEnumerable<VesselUser>> GetVesselUserAsync();
        Task<bool> AssignUserToVesselAsync(int vesselId, string userId);
        Task RemoveUserFromVesselAsync(int vesselUserId);
    }
}