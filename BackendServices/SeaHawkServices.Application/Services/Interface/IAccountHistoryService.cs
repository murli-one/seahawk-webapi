using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IAccountHistoryService
    {
        Task<List<AccountHistoryWithVessel>> GetAccountHistoryWithVesselAsync(
            string UserId,bool IsAdmin ,FuelType fuelType, Specification specification, DateTime? fromDate, DateTime? toDate, string vesselNameFilter,
                string portBunkeredFilter);

        //Task<string?> GetAccountHistoryWithVesselAsync(string userId, string IMONumber, string fuelType, string? specification, DateTime? fromDate, DateTime? toDate);
    }
}