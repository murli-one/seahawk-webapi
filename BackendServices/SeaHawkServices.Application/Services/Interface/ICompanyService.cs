using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ICompanyService
    {
        Task<IEnumerable<Company>> GetAllAsync();
        Task<Company?> GetByIdAsync(int id);
        Task<Company?> GetByNameAsync(string CompanyName);
        Task AddAsync(Company company);
        Task UpdateAsync(Company company);
        Task DeleteAsync(int id);
        Task DeleteCompanyAsync(string companyName);
    }
}