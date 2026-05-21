using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IAnalysisResultService
    {
        Task<IEnumerable<AnalysisResult>> GetAllAsync();
        Task<IEnumerable<AnalysisResult>> GetAllByIdAsync(int vesselDetailId);
        Task<AnalysisResult?> GetByIdAsync(int id);
        Task AddAsync(AnalysisResult company);
        Task UpdateAsync(AnalysisResult company);
        Task DeleteAsync(int id);
    }
}