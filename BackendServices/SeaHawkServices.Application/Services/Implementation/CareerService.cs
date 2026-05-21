using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;


namespace SeaHawkServices.Application.Services.Implementation
{
    public class CareerService : ICareerService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CareerService( IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Career>> GetAllAsync()
        {
            return await _unitOfWork.Career.GetAllAsync();
        }

        public async Task<Career?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Career.GetByIdAsync(id);
        }

        public async Task AddAsync(Career career)
        {
            await _unitOfWork.Career.AddAsync(career);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(Career career)
        {
            await _unitOfWork.Career.UpdateAsync(career);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var career = await _unitOfWork.Career.GetByIdAsync(id);
            if (career != null)
            {
                await _unitOfWork.Career.RemoveAsync(career);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
