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
    public class CompanyUserService : ICompanyUserService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CompanyUserService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<IEnumerable<CompanyUser>> GetAllAsync()
        {
            return await _unitOfWork.CompanyUser.GetAllAsync();
        }
       

        public async Task<IEnumerable<CompanyUser>> GetUsersByCompanyAsync() 
        {
            return await _unitOfWork.CompanyUser.GetAllAsync(includeProperties: "User,Company,Company.VesselDetailList", tracked: false); 
        }

        public async Task<bool> AssignUserToCompanyAsync(int companyId, string userId)
        {
            // ✅ Multi-company: only block duplicates for same (userId, companyId)
            // IMPORTANT: use a predicate that checks both.
            var alreadyExists = await _unitOfWork.CompanyUser
                .AnyAsync(x => x.UserId == userId && x.CompanyId == companyId);  // implement AnyAsync in repo

            if (alreadyExists)
                return false;

            var entity = new CompanyUser
            {
                CompanyId = companyId,
                UserId = userId
            };

            await _unitOfWork.CompanyUser.AddAsync(entity);
            await _unitOfWork.SaveAsync();

            return true;
        }



        public async Task RemoveUserFromCompanyAsync(int companyUserMappingId)
        {
            var entity = await _unitOfWork.CompanyUser.GetAsync(x => x.Id == companyUserMappingId);
            if (entity == null) return;

            await _unitOfWork.CompanyUser.RemoveAsync(entity);   // ✅ IMPORTANT
            await _unitOfWork.SaveAsync();
        }




        public async Task RemoveUserFromAllCompaniesAsync(string userId)
        {
            var rows = (await _unitOfWork.CompanyUser.GetAllAsync(x => x.UserId == userId)).ToList();
            if (rows.Count == 0) return;

            foreach (var r in rows)
                await _unitOfWork.CompanyUser.RemoveAsync(r);

            await _unitOfWork.SaveAsync();
        }

        public async Task<List<CompanyUser>> GetCompaniesForUserAsync(string userId)
        {
            return (await _unitOfWork.CompanyUser
                .GetAllAsync(x => x.UserId == userId, includeProperties: "Company", tracked: false))
                .ToList();
        }


        public async Task UpdateUserCompanyAsync(int companyUserId, int newCompanyId)
        {
            var entity = await _unitOfWork.CompanyUser.GetAsync(x => x.Id == companyUserId);

            if (entity == null) return;

            if (entity.CompanyId == newCompanyId) return;

            entity.CompanyId = newCompanyId;

            // ✅ your repository update method
            await _unitOfWork.CompanyUser.UpdateCompanyUserAsync(entity);
        }

        
    }
}
