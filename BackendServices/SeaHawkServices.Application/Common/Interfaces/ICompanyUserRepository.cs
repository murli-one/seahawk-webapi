using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface ICompanyUserRepository : IRepository<CompanyUser>
    {
        object GetCompanyUser();
        CompanyUser GetCompanyUserByUserId(string id);
        bool Exists(int companyId, string userId);

        // ✅ new
        Task UpdateCompanyUserAsync(CompanyUser entity);
    }
}
