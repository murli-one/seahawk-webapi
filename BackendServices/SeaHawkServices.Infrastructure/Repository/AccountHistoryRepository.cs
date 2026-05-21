using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.Entities;
using SeaHawkServices.Infrastructure.Data;
using System.Data;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class AccountHistoryRepository : Repository<AccountHistoryWithVessel>, IAccountHistoryRepository
    {
        private readonly ApplicationDbContext _context;

        public AccountHistoryRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<AccountHistoryWithVessel>> GetAccountHistoryWithVesselAsync(
      string userId, bool IsAdmin,
      string fuelType,
      string specification,
      DateTime fromDate,
      DateTime toDate,
      string? vesselNameFilter,
      string? portBunkeredFilter)
        {
            #region OldStoredProcedure 
            //var userParam = new SqlParameter("@userId", userId ?? (object)DBNull.Value);
            //var fuelParam = new SqlParameter("@FuelType", fuelType ?? (object)DBNull.Value);
            //var specParam = new SqlParameter("@Specification", specification ?? (object)DBNull.Value);
            //var fromParam = new SqlParameter("@FromDate", fromDate) { SqlDbType = SqlDbType.Date };
            //var toParam = new SqlParameter("@ToDate", toDate) { SqlDbType = SqlDbType.Date };

            //return await _context.Set<AccountHistoryWithVessel>()
            //    .FromSqlRaw(
            //        "EXEC Philip_GetAccountHistoryWithVessel @userId, @FuelType, @Specification, @FromDate, @ToDate",
            //        userParam, fuelParam, specParam, fromParam, toParam)
            //    .ToListAsync();

            #endregion
            if (string.IsNullOrWhiteSpace(userId))
                return new List<AccountHistoryWithVessel>();

            var hasFuelFilter = !string.IsNullOrWhiteSpace(fuelType) &&
                                !fuelType.Equals("All", StringComparison.OrdinalIgnoreCase);

            var hasSpecFilter = !string.IsNullOrWhiteSpace(specification) &&
                                !specification.Equals("ALL", StringComparison.OrdinalIgnoreCase);

            var hasVesselNameFilter = !string.IsNullOrWhiteSpace(vesselNameFilter);
            var hasPortFilter = !string.IsNullOrWhiteSpace(portBunkeredFilter);

            var vesselUsers = _context.Set<VesselUser>();
            var companyUsers = _context.Set<CompanyUser>();
            var vessels = _context.Set<VesselDetail>();
            var results = _context.Set<AnalysisResult>();
            var users = _context.Set<ApplicationUser>();

            // --- Get current user and check SystemAdmin ---
            var currentUser = await users.FirstOrDefaultAsync(u => u.Id == userId);


            // 1. Check if this user is a vessel user
            var vesselUserVesselIds = await vesselUsers
                .Where(vu => vu.UserId == userId)
                .Select(vu => vu.VesselDetailId)
                .ToListAsync();

            // 2. If not a vessel user, check if management (company user)
            List<int> companyUserCompanyIds = new();
            if (!vesselUserVesselIds.Any())
            {
                companyUserCompanyIds = await companyUsers
                    .Where(cu => cu.UserId == userId)
                    .Select(cu => cu.CompanyId)
                    .ToListAsync();
            }

            IQueryable<AccountHistoryWithVessel> query;

            // ---------------- Scenario 0: SystemAdmin (see ALL data) ----------------
            if (IsAdmin)
            {
                // 1. Get all vessels that match the vessel name filter
                var AllVesselDetailIds = await vessels
                    .Where(v =>
                        v.VesselName != null &&
                        EF.Functions.Like(v.VesselName, "%" + vesselNameFilter + "%"))
                    .Select(v => v.Id)
                    .ToListAsync();

                if (!AllVesselDetailIds.Any())
                    return new List<AccountHistoryWithVessel>();

                // 3. Query AnalysisResult using these VesselDetailIds
                query = results
    .Where(ar =>
        ar.DateReceived >= fromDate &&
        ar.DateReceived <= toDate &&
        ar.VesselDetailId.HasValue &&
        AllVesselDetailIds.Contains(ar.VesselDetailId.Value) &&
        (string.IsNullOrEmpty(portBunkeredFilter) ||
         (ar.PortBunkered != null &&
          EF.Functions.Like(ar.PortBunkered, "%" + portBunkeredFilter + "%")))
    )
    .Join(
        vessels,
        ar => ar.VesselDetailId.Value,
        vd => vd.Id,
        (ar, vd) => new AccountHistoryWithVessel
        {
            VesselName = vd.VesselName,
            IMONumber = vd.IMONumber,
            PortBunkered = ar.PortBunkered,
            DateBunkered = ar.DateBunkered ?? DateTime.MinValue,
            Specification = !string.IsNullOrEmpty(ar.Specification)
                ? ar.Specification
                : ar.Grade,
            FuelType = ar.FuelType,
            DateReceived = (DateTime)(ar.SampleReportDate ?? ar.DateReceived),
            SampleNumber = ar.SampleNumber,
            Comment = ar.Comment
        });
            }
            // ---------------- Scenario 1: Vessel User ----------------
            else if (vesselUserVesselIds.Any())
            {
                query = results
                    .Where(ar =>
                        ar.DateReceived >= fromDate &&
                        ar.DateReceived <= toDate &&
                        ar.VesselDetailId.HasValue &&
                        vesselUserVesselIds.Contains(ar.VesselDetail.Id))
                    .Join(
                        vessels,
                        ar => ar.VesselDetailId.Value,
                        vd => vd.Id,
                        (ar, vd) => new AccountHistoryWithVessel
                        {
                            VesselName = vd.VesselName,
                            IMONumber = vd.IMONumber,
                            PortBunkered = ar.PortBunkered,
                            DateBunkered = ar.DateBunkered ?? DateTime.MinValue,
                            Specification = !string.IsNullOrEmpty(ar.Specification)
                                ? ar.Specification
                                : ar.Grade,
                            FuelType = ar.FuelType,
                            DateReceived = (DateTime)(ar.SampleReportDate ?? ar.DateReceived),
                            SampleNumber = ar.SampleNumber,
                            Comment = ar.Comment
                        });
            }
            // ---------------- Scenario 2: Management User ----------------
            else if (companyUserCompanyIds.Any())
            {
                var companyVessels = vessels
                    .Where(vd =>
                        vd.CompanyId.HasValue &&
                        companyUserCompanyIds.Contains(vd.CompanyId.Value));

                query = results
                    .Where(ar =>
                        ar.DateReceived >= fromDate &&
                        ar.DateReceived <= toDate &&
                        ar.VesselDetailId.HasValue)
                    .Join(
                        companyVessels,
                        ar => ar.VesselDetailId.Value,
                        vd => vd.Id,
                        (ar, vd) => new AccountHistoryWithVessel
                        {
                            VesselName = vd.VesselName,
                            IMONumber = vd.IMONumber,
                            PortBunkered = ar.PortBunkered,
                            DateBunkered = ar.DateBunkered ?? DateTime.MinValue,
                            Specification = !string.IsNullOrEmpty(ar.Specification) ? ar.Specification : ar.Grade,
                            FuelType = ar.FuelType,
                            DateReceived = (DateTime)(ar.SampleReportDate ?? ar.DateReceived),
                            SampleNumber = ar.SampleNumber,
                            Comment = ar.Comment
                        });
            }
            else
            {
                // Neither vessel user nor management user
                return new List<AccountHistoryWithVessel>();
            }

            // 3. Apply fuel & specification filters (all roles)
            if (hasFuelFilter)
                query = query.Where(x => x.FuelType == fuelType);

            if (hasSpecFilter)
                query = query.Where(x => x.Specification == specification);

            // 4. Apply VesselName & Port filters (all roles – esp. SystemAdmin)

            if (hasVesselNameFilter)
            {
                query = query.Where(x =>
                    x.VesselName != null &&
                    EF.Functions.Like(x.VesselName, "%" + vesselNameFilter + "%"));
                // or, if you prefer ToLower():
                // query = query.Where(x =>
                //     x.VesselName != null &&
                //     x.VesselName.ToLower().Contains(vesselNameFilter!.ToLower()));
            }

            if (hasPortFilter)
            {
                query = query.Where(x =>
                    x.PortBunkered != null &&
                    EF.Functions.Like(x.PortBunkered, "%" + portBunkeredFilter + "%"));
                // or ToLower variant:
                // query = query.Where(x =>
                //     x.PortBunkered != null &&
                //     x.PortBunkered.ToLower().Contains(portBunkeredFilter!.ToLower()));
            }

            // 5. Execute
            return await query
                .OrderByDescending(x => x.DateReceived)
                .ToListAsync();
        }
    }
}
