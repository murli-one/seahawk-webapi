using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Infrastructure.Data;
using System.Linq.Expressions;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class VesselDetailRepository : Repository<VesselDetail>, IVesselDetailRepository
    {
        private readonly ApplicationDbContext _context;

        public VesselDetailRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

            public async Task<IEnumerable<VesselDetail>> GetAllAsync()
            {
                return await _context.VesselDetail
                                     .Include(v => v.Company).OrderBy(v=>v.Id)
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
        public async Task<VesselDetail?> GetCompanyByVesselDetailIdAsync(int VesselDetailId)
        {
            return await _context.VesselDetail
                                 .AsNoTracking().Include(x=>x.Company)
                                 .FirstOrDefaultAsync(c => c.Id == VesselDetailId);
        }
        public async Task<VesselDetail?> GetCompanyByVesselNameAsync(string VesselName)
        {
            return await _context.VesselDetail
                                 .AsNoTracking().Include(x=>x.Company)
                                 .FirstOrDefaultAsync(c => c.VesselName.ToLower() == VesselName.ToLower());
        }

        public IQueryable<VesselDetail> Query()
        {
            return _context.VesselDetail.AsNoTracking();
        }

        public async Task UpdateAsync(VesselDetail vesselDetails)
        {
            try
            {
                _context.ChangeTracker.Clear();
                var record = _context.VesselDetail.Update(vesselDetails);
            }
            catch (Exception ex)
            {

                throw;
            }
        

        }
    }
}
