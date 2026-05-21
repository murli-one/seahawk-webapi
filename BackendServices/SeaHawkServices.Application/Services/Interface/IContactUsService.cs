using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IContactUsService
    {
        Task<IEnumerable<ContactUs>> GetAllAsync();
        Task<ContactUs?> GetByIdAsync(int id);
        Task AddAsync(ContactUs ContactUs);
        Task UpdateAsync(ContactUs ContactUs);
        Task DeleteAsync(int id);
    }
}