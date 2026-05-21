using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using SeaHawkServices.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SeaHawkServices.Infrastructure.Repository
{
    public sealed class SampleReportRepository : ISampleReportRepository
    {
        private readonly string _cs;
        public SampleReportRepository(IConfiguration cfg)
            => _cs = cfg.GetConnectionString("DefaultConnection")
                     ?? throw new InvalidOperationException("DefaultConnection missing.");

        public async Task<string?> GetReportFileNameAsync(string sampleNumber)
        {
            await using var con = new SqlConnection(_cs);
            await con.OpenAsync().ConfigureAwait(false);

            await using var cmd = new SqlCommand("dbo.Philip_GetFileName", con)
            {
                CommandType = CommandType.StoredProcedure,
                CommandTimeout = 60
            };
            cmd.Parameters.Add("@SampleNumber", SqlDbType.NVarChar, 50)
               .Value = (object?)sampleNumber ?? DBNull.Value;

            var result = await cmd.ExecuteScalarAsync().ConfigureAwait(false);
            return result is null || result == DBNull.Value ? null : result.ToString();
        }
    }
}
