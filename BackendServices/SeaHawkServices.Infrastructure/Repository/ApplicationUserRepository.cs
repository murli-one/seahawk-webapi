using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        private readonly ApplicationDbContext _context;

        public ApplicationUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ApplicationUser> GetByIdAsync(string id)
        {
            return await _context.ApplicationUsers.FindAsync(id);
        }

        public void Update(ApplicationUser user)
        {
            _context.ApplicationUsers.Update(user);
        }

        public Task<bool> ExistsByUserNameAsync(string userName)
        {
          
            return _context.ApplicationUsers
                           .AnyAsync(u => u.UserName == userName);
        }

        public Task<ApplicationUser> GetByUserNameAsync(string userName)
        {

            var User = _context.ApplicationUsers.FirstOrDefaultAsync
                           (u => u.UserName.ToLower() == userName.ToLower());

            return User;
        }
    }
}
