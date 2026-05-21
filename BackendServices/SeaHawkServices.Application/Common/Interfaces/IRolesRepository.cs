using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Domain.StoredProcedures;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface IRolesRepository : IRepository<Roles>
    {
        public Task<Roles?> GetByNameAsync(string name);
        public Task<bool> ExistsAsync(string name);
        Task<Roles?> GetByIdAsync(int id);
    }
}
