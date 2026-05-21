using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ICompanyUserService
    {
        Task<IEnumerable<CompanyUser>> GetAllAsync();
        Task<IEnumerable<CompanyUser>> GetUsersByCompanyAsync();
        Task<bool> AssignUserToCompanyAsync(int companyId, string userId);
        Task RemoveUserFromCompanyAsync(int companyUserId);
        Task UpdateUserCompanyAsync(int companyUserId, int newCompanyId);

    }
}