using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class SmtpSettingRepository : Repository<SmtpSetting>, ISmtpSettingRepository
    {
        private readonly ApplicationDbContext _context;

        public SmtpSettingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<SmtpSetting?> GetFirstAsync()
        {
            return await _context.SmtpSettings.FirstOrDefaultAsync();
        }

        public Task UpdateAsync(SmtpSetting smtpSetting)
        {
            _context.SmtpSettings.Update(smtpSetting);
            return Task.CompletedTask;
        }
    }
}