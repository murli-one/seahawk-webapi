using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Domain.StoredProcedures;
using SeaHawkServices.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class XMLReportRepository : Repository<DistillateRow>, IXMLReportRepository
    {
        private readonly ApplicationDbContext _context;

        public XMLReportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<List<DistillateRow>> GetDistillateAsync(string sampleNumber)
        {
            try
            {
                var sn = sampleNumber?.Trim();
                var p = new SqlParameter("@SampleNumber", SqlDbType.NVarChar)
                {
                    Value = (object?)sn ?? DBNull.Value
                };

                const string sql = "EXEC dbo.Philip_GetDistillateSampleNumberDetails @SampleNumber";

                var Record = await _context.Set<DistillateRow>()
                     .FromSqlRaw(sql, p)
                     .AsNoTracking()
                     .ToListAsync();
                return Record;
            }
            catch (Exception e)
            {

            }
            return null;
        }
        public async Task<List<ResidualRow>> GetResidualAsync(string sampleNumber)
        {
            try
            {
                var sn = sampleNumber?.Trim();
                var p = new SqlParameter("@SampleNumber", SqlDbType.NVarChar)
                {
                    Value = (object?)sn ?? DBNull.Value
                };

                const string sql = "EXEC dbo.Philip_GetResidualSampleNumberDetails @SampleNumber";

                var Record = await _context.Set<ResidualRow>()
                     .FromSqlRaw(sql, p)
                     .AsNoTracking()
                     .ToListAsync();
                return Record;
            }
            catch (Exception e)
            {

            }
            return null;
        }
       
    }
}
