using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface IApplicationUserRepository : IRepository<ApplicationUser>
    {
        Task<ApplicationUser> GetByIdAsync(string id);
        void Update(ApplicationUser user);
        Task<bool> ExistsByUserNameAsync(string userName);
        Task<ApplicationUser> GetByUserNameAsync(string userName);
    }
}
