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
    public class SamplingKitRepository : Repository<SamplingKit>, ISamplingKitRepository
    {
        private readonly ApplicationDbContext _context;

        public SamplingKitRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SamplingKit?> GetByIdAsync(int id)
        {
            return await _context.SamplingKit
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(SamplingKit samplingKit)
        {
            var record = _context.SamplingKit.Update(samplingKit);

        }

        public async Task DeleteUserAllEnteries(string Userid)
        {
            var AllEnteries = await _context.SamplingKit.Where(x => x.ApplicationUserId == Userid).ToListAsync();
            foreach (var Item in AllEnteries)
                if (Item != null)
                {
                    _context.SamplingKit.Remove(Item);
                    await _context.SaveChangesAsync();
                }
        }

    }
}
