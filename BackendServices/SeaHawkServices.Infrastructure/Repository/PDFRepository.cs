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
    public class PDFRepository : Repository<PDF>, IPDFRepository
    {
        private readonly ApplicationDbContext _context;

        public PDFRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PDF?> GetByIdAsync(int id)
        {
            return await _context.PDF
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(PDF pdf)
        {
            var record = _context.PDF.Update(pdf);

        }
        public async Task<PDF?> GetByFileNameAsync(string filename)
        {
            return await _context.PDF
                               .AsNoTracking()
                               .FirstOrDefaultAsync(c => c.Name == filename);
        }

    }
}
