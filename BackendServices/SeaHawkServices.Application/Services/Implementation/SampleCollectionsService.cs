using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;


namespace SeaHawkServices.Application.Services.Implementation
{
    public class SampleCollectionsService : ISampleCollectionsService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SampleCollectionsService( IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<SampleCollections>> GetAllAsync()
        {
            return await _unitOfWork.SampleCollections.GetAllAsync();
        }
        public async Task<IEnumerable<SampleCollections>> GetAllForSpecificUserAsync(string userId)
        {
            return await _unitOfWork.SampleCollections.GetAllAsync(x=>x.ApplicationUserId == userId);
        }

        public async Task<SampleCollections?> GetByIdAsync(int id)
        {
            return await _unitOfWork.SampleCollections.GetByIdAsync(id);
        }

        public async Task AddAsync(SampleCollections pickupRequest)
        {
            await _unitOfWork.SampleCollections.AddAsync(pickupRequest);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(SampleCollections pickupRequest)
        {
            await _unitOfWork.SampleCollections.UpdateAsync(pickupRequest);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pickupRequest = await _unitOfWork.SampleCollections.GetByIdAsync(id);
            if (pickupRequest != null)
            {
                await _unitOfWork.SampleCollections.RemoveAsync(pickupRequest);
                await _unitOfWork.SaveAsync();
            }
        }
       
    }
}
