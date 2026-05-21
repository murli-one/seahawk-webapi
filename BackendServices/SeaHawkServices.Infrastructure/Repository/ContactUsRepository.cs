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

namespace SeaHawkServices.Infrastructure.Repository
{
    public class ContactUsRepository : Repository<ContactUs>, IContactUsRepository
    {
        private readonly ApplicationDbContext _context;

        public ContactUsRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<ContactUs?> GetByIdAsync(int id)
        {
            return await _context.ContactUs
                                 .AsNoTracking()
                                 .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task UpdateAsync(ContactUs ContactUs)
        {
            var record = _context.ContactUs.Update(ContactUs);

        }
    }
}
