using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class SmtpService : ISmtpService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SmtpService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SmtpSetting?> GetAsync()
        {
            return await _unitOfWork.SmtpSettings.GetFirstAsync();
        }

        public async Task UpdateAsync(SmtpSetting model)
        {
            var existing = await _unitOfWork.SmtpSettings.GetFirstAsync();

            if (existing == null)
            {
                await _unitOfWork.SmtpSettings.AddAsync(model);
            }
            else
            {
               existing.Email = model.Email;
               existing.Password = model.Password;

                await _unitOfWork.SmtpSettings.UpdateAsync(existing);
            }

            await _unitOfWork.SaveAsync();
        }
    }
}