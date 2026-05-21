using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ICareerService
    {
        Task<IEnumerable<Career>> GetAllAsync();
        Task<Career?> GetByIdAsync(int id);
        Task AddAsync(Career career);
        Task UpdateAsync(Career career);
        Task DeleteAsync(int id);
    }
}