using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Domain.StoredProcedures;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface ILiveDataService
    {
        Task<List<LiveDataRow>>GetLiveDataAsync(string UserId, bool IsSystemAdmin);
       
    }
}