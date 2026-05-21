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
    public class CompanyService : ICompanyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyService( IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Company>> GetAllAsync()
        {
            return await _unitOfWork.Companies.GetAllAsync(
                includeProperties: "VesselDetailList",
                tracked: false
            );
        }

        public async Task<Company?> GetByIdAsync(int id)
        {
            return await _unitOfWork.Companies.GetByIdAsync(id);
        }
        public async Task<Company?> GetByNameAsync(string CompanyName)
        {
            return await _unitOfWork.Companies.GetByCompanyNameAsync(CompanyName);
        }

        public async Task AddAsync(Company company)
        {
            await _unitOfWork.Companies.AddAsync(company);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(Company company)
        {
            await _unitOfWork.Companies.UpdateAsync(company);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var company = await _unitOfWork.Companies.GetByIdAsync(id);
            if (company != null)
            {
                await _unitOfWork.Companies.RemoveAsync(company);
                await _unitOfWork.SaveAsync();
            }
        }
        public async Task DeleteCompanyAsync(string companyName)
        {
            var company = await _unitOfWork.Companies.GetByCompanyNameAsync(companyName);
            if (company != null)
            {
                await _unitOfWork.Companies.RemoveAsync(company);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
