using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;
using System.Linq.Expressions;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class AnalysisResultRepository : Repository<AnalysisResult>, IAnalysisResultRepository
    {
        private readonly ApplicationDbContext _context;

        public AnalysisResultRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        // ----------------------------
        // GET BY RECORD ID (INCLUDES VESSEL)
        // ----------------------------
        public async Task<AnalysisResult?> GetByIdAsync(int id)
        {
            return await _context.AnalysisResult
                .Include(x => x.VesselDetail)     // <-- IMPORTANT
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        // ----------------------------
        // GET ALL RECORDS (FOR SYSTEM ADMIN)
        // ----------------------------
        public async Task<IEnumerable<AnalysisResult>> GetAllAsync()
        {
            return await _context.AnalysisResult
                .Include(x => x.VesselDetail)     // <-- FIX: Always include vessel info
                .AsNoTracking()
                .ToListAsync();
        }

        // ----------------------------
        // GET ALL BY VESSELDETAILID (FOR MANAGEMENT USER)
        // ----------------------------
        public async Task<IEnumerable<AnalysisResult>> GetByVesselIdAsync(int vesselDetailId)
        {
            return await _context.AnalysisResult
                .Include(x => x.VesselDetail)     // <-- FIX: Include navigation property
                .AsNoTracking()
                .Where(x => x.VesselDetailId == vesselDetailId)
                .ToListAsync();
        }

        // ----------------------------
        // UPDATE RECORD
        // ----------------------------
        public async Task UpdateAsync(AnalysisResult analysisResult)
        {
            _context.AnalysisResult.Update(analysisResult);
            await _context.SaveChangesAsync();    // <-- You forgot the save!
        }
    }
}
