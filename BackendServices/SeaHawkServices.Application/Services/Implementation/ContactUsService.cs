using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class ContactUsService : IContactUsService
    {
 
        private readonly IUnitOfWork _unitOfWork;

        public ContactUsService( IUnitOfWork unitOfWork)
        {
          
            _unitOfWork = unitOfWork;
        }


        public async Task<IEnumerable<ContactUs>> GetAllAsync()
        {
            return await _unitOfWork.ContactUs.GetAllAsync();
        }

        public async Task<ContactUs?> GetByIdAsync(int id)
        {
            return await _unitOfWork.ContactUs.GetByIdAsync(id);
        }

        public async Task AddAsync(ContactUs ContactUs)
        {
            await _unitOfWork.ContactUs.AddAsync(ContactUs);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(ContactUs ContactUs)
        {
            await _unitOfWork.ContactUs.UpdateAsync(ContactUs);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var analysisResult = await _unitOfWork.ContactUs.GetByIdAsync(id);
            if (analysisResult != null)
            {
                await _unitOfWork.ContactUs.RemoveAsync(analysisResult);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
