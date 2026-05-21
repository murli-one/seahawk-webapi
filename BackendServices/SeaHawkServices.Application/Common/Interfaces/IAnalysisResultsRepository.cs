using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Common.Interfaces
{
    public interface IAnalysisResultRepository : IRepository<AnalysisResult>
    {
        Task<AnalysisResult?> GetByIdAsync(int id);
        Task<IEnumerable<AnalysisResult>> GetByVesselIdAsync(int vesselDetailId);
        Task UpdateAsync(AnalysisResult company);
    }
}
