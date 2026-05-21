using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;
using System.Linq.Expressions;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class CareerRepository : Repository<Career>, ICareerRepository
    {
        private readonly ApplicationDbContext _context;

        public CareerRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Career?> GetByIdAsync(int id)
        {
            return await _context.Career
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(Career career)
        {
            var record = _context.Career.Update(career);

        }

    }
}
