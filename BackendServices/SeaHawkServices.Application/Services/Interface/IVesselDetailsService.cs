using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IVesselDetailService
    {
        Task<IEnumerable<VesselDetail>> GetAllAsync();
        Task<VesselDetail?> GetByIdAsync(int id);
        Task<IEnumerable<Company>> GetCompanies();
        Task AddAsync(VesselDetail vesselDetails);
        Task UpdateAsync(VesselDetail vesselDetails);
        Task DeleteAsync(int id);
        // NEW: role-based vessels
        Task<IEnumerable<VesselDetail>> GetVesselsForUserAsync(
            string userId,
            int? companyId,
            Enums.Role role);
    }
}