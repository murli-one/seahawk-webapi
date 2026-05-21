using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class SampleCollectionsRepository : Repository<SampleCollections>, ISampleCollectionsRepository
    {
        private readonly ApplicationDbContext _context;

        public SampleCollectionsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SampleCollections?> GetByIdAsync(int id)
        {
            return await _context.SampleCollections
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(SampleCollections pickupRequest)
        {
            var record = _context.SampleCollections.Update(pickupRequest);

        }
        public async Task DeleteUserAllEnteries(string Userid)
        {
            var AllEnteries = await _context.SampleCollections.Where(x => x.ApplicationUserId == Userid).ToListAsync();
            foreach (var Item in AllEnteries)
                if (Item != null)
                {
                    _context.SampleCollections.Remove(Item);
                    await _context.SaveChangesAsync();
                }
        }
    }
}
