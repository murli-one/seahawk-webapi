using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ISampleCollectionsService
    {
        Task<IEnumerable<SampleCollections>> GetAllAsync();
        Task<IEnumerable<SampleCollections>> GetAllForSpecificUserAsync(string userId);
        Task<SampleCollections?> GetByIdAsync(int id);
        Task AddAsync(SampleCollections pickupRequest);
        Task UpdateAsync(SampleCollections pickupRequest);
        Task DeleteAsync(int id);
    }
}