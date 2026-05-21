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
    public interface ICompanyRepository : IRepository<Company>
    {
        Task<Company?> GetByIdAsync(int id);
        Task<Company?> GetByCompanyNameAsync(string CompanyName);
        IQueryable<Company> Query();
        Task UpdateAsync(Company company);
    }
}
