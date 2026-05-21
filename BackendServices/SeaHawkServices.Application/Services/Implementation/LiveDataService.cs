using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Domain.StoredProcedures;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class LiveDataService : ILiveDataService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LiveDataService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<LiveDataRow>> GetLiveDataAsync(string UserId, bool IsSystemAdmin)
        {
           return await _unitOfWork.LiveData.GetLiveData(UserId,IsSystemAdmin);
        }
        
    }
}
