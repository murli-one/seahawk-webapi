using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IApplicationUserService
    {
        Task<IEnumerable<ApplicationUser>> GetAllAsync();
        Task<IEnumerable<ApplicationUser>> GetAllUsersInsteadOfYours(string userName);
        Task<ApplicationUser> GetByIdAsync(string id);
        Task AddAsync(ApplicationUser user);
        Task UpdateAsync(ApplicationUser user);
        Task DeleteAsync(string id);
        Task<bool> UserExistsByUserNameAsync(string username);
        Task<ApplicationUser> GetUserByUserNameAsync(string username);
    }
}

