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
    public interface IPDFRepository : IRepository<PDF>
    {
        Task<PDF?> GetByIdAsync(int id);
        Task UpdateAsync(PDF pdf);

        Task<PDF?> GetByFileNameAsync(string filename);
    }
}
