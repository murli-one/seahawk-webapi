using Data;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class RolesRepository : Repository<Roles>, IRolesRepository
    {
        private readonly ApplicationDbContext _context;

        public RolesRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public Task<Roles?> GetByNameAsync(string name)
        {
            return _context.Roles
                           .AsNoTracking()
                           .FirstOrDefaultAsync(r => r.RoleName == name);
        } 
        public Task<bool> ExistsAsync(string name)
        {
            return _context.Roles
                           .AsNoTracking()
                           .AnyAsync(r => r.RoleName == name);
        }
        public async Task<Roles?> GetByIdAsync(int id)
        {
            return await _context.Roles
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

    }
}
