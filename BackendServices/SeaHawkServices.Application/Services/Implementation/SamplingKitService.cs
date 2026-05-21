using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;


namespace SeaHawkServices.Application.Services.Implementation
{
    public class SamplingKitService : ISamplingKitService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SamplingKitService( IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SamplingKit>> GetAllAsync()
        {
            return await _unitOfWork.SamplingKit.GetAllAsync();
        }

        public async Task<SamplingKit?> GetByIdAsync(int id)
        {
            return await _unitOfWork.SamplingKit.GetByIdAsync(id);
        }
        public async Task<IEnumerable<SamplingKit?>> GetAllForSpecificUser(string id)
        {
            return await _unitOfWork.SamplingKit.GetAllAsync(x=>x.ApplicationUserId == id);
        }

        public async Task AddAsync(SamplingKit samplingKit)
        {
            await _unitOfWork.SamplingKit.AddAsync(samplingKit);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(SamplingKit samplingKit)
        {
            await _unitOfWork.SamplingKit.UpdateAsync(samplingKit);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var samplingKit = await _unitOfWork.SamplingKit.GetByIdAsync(id);
            if (samplingKit != null)
            {
                await _unitOfWork.SamplingKit.RemoveAsync(samplingKit);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
