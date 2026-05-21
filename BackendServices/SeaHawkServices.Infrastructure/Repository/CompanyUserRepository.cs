using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Infrastructure.Data;
using System.Data;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class CompanyUserRepository : Repository<CompanyUser>, ICompanyUserRepository
    {
        private readonly ApplicationDbContext _context;

        public CompanyUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public object GetCompanyUser()
        {
            //return _context.CompanyUser
            //.Include(cu => cu.User)
            //.Include(cu => cu.Company)
            //.ToList();

            var companyUsers = _context.CompanyUser.ToList();
            var users = _context.ApplicationUsers.ToList();
            var companies = _context.Company.ToList();

            return new
            {
                CompanyUsers = companyUsers,
                Users = users,
                Companies = companies
            };
        }

        public bool Exists(int companyId, string userId)
        {
            return _context.CompanyUser.Any(cu => cu.CompanyId == companyId && cu.UserId == userId);
        }
        // ✅ ADD THIS UPDATE METHOD
        public async Task UpdateCompanyUserAsync(CompanyUser entity)
        {
            _context.CompanyUser.Update(entity);
            await _context.SaveChangesAsync();
        }

        public CompanyUser GetCompanyUserByUserId(string id)
        {
            return _context.CompanyUser.Where(x => x.UserId == id).Include(cu => cu.User)
                .Include(cu => cu.Company).ThenInclude(t => t.VesselDetailList)
                .FirstOrDefault();
        }
        
     
    }
}
