using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Infrastructure.Data;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _context.Company.Include(x=>x.VesselDetailList)
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }
        public async Task<Company?> GetByCompanyNameAsync(string CompanyName)
        {
            return await _context.Company
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.CompanyName.ToLower() == CompanyName.ToLower());
        }
        public IQueryable<Company> Query()
        {
            return _context.Company.AsNoTracking();
        }

        public async Task UpdateAsync(Company company)
        {
            try
            {
                var record = _context.Company.Update(company);
            }
            catch (Exception ex)
            {

             
            }
       
        }
    }
}
