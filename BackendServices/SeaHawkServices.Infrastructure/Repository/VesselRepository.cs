using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Infrastructure.Data;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class VesselRepository : Repository<VesselDetail>, IVesselRepository
    {
        private readonly ApplicationDbContext _context;

        public VesselRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VesselDetail>> GetAllAsync()
        {
            return await _context.VesselDetail
                .Include(x => x.Company)
                                 .AsNoTracking()
                                 .ToListAsync();
        }

        public async Task<VesselDetail?> GetByIdAsync(int id)
        {
            return await _context.VesselDetail
                           .Include(x => x.Company)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(VesselDetail VesselDetail)
        {
            await _context.VesselDetail.AddAsync(VesselDetail);
        }

        public async Task UpdateAsync(VesselDetail VesselDetail)
        {
            _context.VesselDetail.Update(VesselDetail);
            await Task.CompletedTask;
        }

        public async Task RemoveAsync(VesselDetail VesselDetail)
        {
            _context.VesselDetail.Remove(VesselDetail);
            await Task.CompletedTask;
        }
    }
}
