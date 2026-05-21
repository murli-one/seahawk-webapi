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
    public interface INewsRepository : IRepository<NewsFeed>
    {
        Task<NewsFeed?> GetByIdAsync(int id);
        Task UpdateAsync(NewsFeed newsFeed);
        Task<IEnumerable<NewsFeed>> GetActiveNews();
    }

}
