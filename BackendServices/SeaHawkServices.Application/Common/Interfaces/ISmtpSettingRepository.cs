using SeaHawkServices.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface ISmtpSettingRepository : IRepository<SmtpSetting>
    {
        Task<SmtpSetting?> GetFirstAsync();
        Task UpdateAsync(SmtpSetting smtpSetting);
    }
}