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
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class AccountHistoryService : IAccountHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AccountHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AccountHistoryWithVessel>> GetAccountHistoryWithVesselAsync(
      string userId,bool IsAdmin,
      FuelType fuelType,
      Specification specification,
      DateTime? fromDate,
      DateTime? toDate,
      string? vesselNameFilter,
      string? portBunkeredFilter)
        {
            var fuelTypeText = GeneralEnumDDLs.FuelTypeDesc(fuelType);
            var specificationText = GeneralEnumDDLs.SpecificationDesc(specification);

            return await _unitOfWork.AccountHistory.GetAccountHistoryWithVesselAsync(
                userId, IsAdmin,
                fuelTypeText,
                specificationText,
                (DateTime)fromDate!,
                (DateTime)toDate!,
                vesselNameFilter,
                portBunkeredFilter
            );
        }

    }
}
