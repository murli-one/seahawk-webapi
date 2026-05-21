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
    public interface IXMLReportRepository : IRepository<DistillateRow>
    {
        public Task<List<DistillateRow>> GetDistillateAsync(string sampleNumber);
        Task<List<ResidualRow>> GetResidualAsync(string sampleNumber);
    }

}
