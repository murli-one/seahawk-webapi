using Data;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class UserLoginHistoryRepository
        : Repository<UserLoginHistory>, IUserLoginHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public UserLoginHistoryRepository(ApplicationDbContext context)
            : base(context)
        {
            _context = context;
        }

        // You can rely on base Repository<T> methods like GetAllAsync/AddAsync,
        // but if you want explicit control/order you can override like this:
        public async Task<IEnumerable<UserLoginHistory>> GetAllAsync(
            bool tracked = false)
        {
            var query = _context.UserLoginHistory.AsQueryable();

            if (!tracked)
                query = query.AsNoTracking();

            return await query
                .OrderByDescending(x => x.LoginTimeUtc)
                .ToListAsync();
        }
    }
}
