using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ISamplingKitService
    {
        Task<IEnumerable<SamplingKit>> GetAllAsync();
        Task<IEnumerable<SamplingKit>> GetAllForSpecificUser(string id);
        Task<SamplingKit?> GetByIdAsync(int id);
        Task AddAsync(SamplingKit samplingKit);
        Task UpdateAsync(SamplingKit samplingKit);
        Task DeleteAsync(int id);
    }
}