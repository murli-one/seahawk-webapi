using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class VesselUserService : IVesselUserService
    {
        private readonly IVesselUserRepository _vesselUserRepository;

        public VesselUserService(IVesselUserRepository vesselUserRepository)
        {
            _vesselUserRepository = vesselUserRepository;
        }

        public async Task<IEnumerable<VesselUser>> GetVesselUserAsync()
        {
            return await _vesselUserRepository.GetVesselUserAsync();
        }

        public async Task<bool> AssignUserToVesselAsync(int vesselId, string userId)
        {
            return await _vesselUserRepository.AssignUserToVesselAsync(vesselId, userId);
        }

        public async Task RemoveUserFromVesselAsync(int vesselUserId)
        {
            await _vesselUserRepository.RemoveUserFromVesselAsync(vesselUserId);
        }
    }
}
