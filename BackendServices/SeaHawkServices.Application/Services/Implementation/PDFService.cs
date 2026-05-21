using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;


namespace SeaHawkServices.Application.Services.Implementation
{
    public class PDFService : IPDFService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PDFService( IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<PDF>> GetAllAsync()
        {
            return await _unitOfWork.PDF.GetAllAsync();
        }

        public async Task<PDF?> GetByIdAsync(int id)
        {
            return await _unitOfWork.PDF.GetByIdAsync(id);
        }   
        public async Task<PDF?> GetByFileNameAsync(string? filename)
        {
            return await _unitOfWork.PDF.GetByFileNameAsync(filename);
        }

        public async Task AddAsync(PDF pdf)
        {
            await _unitOfWork.PDF.AddAsync(pdf);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(PDF pdf)
        {
            await _unitOfWork.PDF.UpdateAsync(pdf);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var pdf = await _unitOfWork.PDF.GetByIdAsync(id);
            if (pdf != null)
            {
                await _unitOfWork.PDF.RemoveAsync(pdf);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
