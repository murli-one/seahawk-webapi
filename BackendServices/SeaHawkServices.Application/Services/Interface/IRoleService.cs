using Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IRoleService
    {
        Task<List<Roles>> GetAllAsync();
        Task<Roles?> GetByIdAsync(int id);
        Task<string?> CreateAsync(string roleName, string? description);

        Task<string?> UpdateAsync(Roles role);

        Task DeleteAsync(int id);
    }
}
