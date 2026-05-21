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
    public interface ILiveDataRepository : IRepository<LiveDataRow>
    {
        Task<List<LiveDataRow>> GetLiveData(string userId,bool IsSystemAdmin);
    }
}
