using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IVesselService
    {
        Task<IEnumerable<VesselDetail>> GetAllAsync();
        Task<VesselDetail?> GetByIdAsync(int id);
        Task AddAsync(VesselDetail VesselDetail);
        Task DeleteAsync(int id);
    }
}