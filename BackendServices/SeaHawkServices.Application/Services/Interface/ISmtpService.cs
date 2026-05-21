using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ISmtpService
    {
        Task<SmtpSetting?> GetAsync();
        Task UpdateAsync(SmtpSetting model);
    }
}