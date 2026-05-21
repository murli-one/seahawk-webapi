using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.StoredProcedures;
using SeaHawkServices.Infrastructure.Data;
using System.Data;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class LiveDataRepository : Repository<LiveDataRow>, ILiveDataRepository
    {
        private readonly ApplicationDbContext _context;

        public LiveDataRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
            public async Task<List<LiveDataRow>> GetLiveData(string userId, bool isSystemAdmin)
            {
            var userParam = new SqlParameter("@UserId", SqlDbType.UniqueIdentifier)
            {
                Value = Guid.Parse(userId)
            };

            var adminParam = new SqlParameter("@IsSystemAdmin", SqlDbType.Bit)
            {
                Value = isSystemAdmin
            };

            return await _context.Set<LiveDataRow>()
                .FromSqlRaw(
                    "EXEC dbo.Philip_GetLiveDataNew @UserId = @UserId, @IsSystemAdmin = @IsSystemAdmin",
                    userParam,
                    adminParam)
                .ToListAsync();
        }

    }
}
