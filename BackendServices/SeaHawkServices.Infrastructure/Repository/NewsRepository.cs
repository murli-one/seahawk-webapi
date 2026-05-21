using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Domain.StoredProcedures;
using SeaHawkServices.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class NewsRepository : Repository<NewsFeed>, INewsRepository
    {
        private readonly ApplicationDbContext _context;

        public NewsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<NewsFeed?> GetByIdAsync(int id)
        {
            return await _context.NewsFeed
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(NewsFeed NewsFeed)
        {
            var record = _context.NewsFeed.Update(NewsFeed);

        }

        public async  Task<IEnumerable<NewsFeed>> GetActiveNews()
        {
           var records = await _context.NewsFeed.Where(x => x.Status == Status.Active).ToListAsync();
            return records;
        }

       
    }
}
