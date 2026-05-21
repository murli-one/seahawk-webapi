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

namespace SeaHawkServices.Application.Services.Implementation
{
    public class AnalysisResultService : IAnalysisResultService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AnalysisResultService( IUnitOfWork unitOfWork)
        {
             _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<AnalysisResult>> GetAllAsync()
        {
            return await _unitOfWork.AnalysisResult.GetAllAsync();
        }

        public async Task<IEnumerable<AnalysisResult>> GetAllByIdAsync(int vesselDetailId)
        {
            return await _unitOfWork.AnalysisResult.GetByVesselIdAsync(vesselDetailId);
        }

        public async Task<AnalysisResult?> GetByIdAsync(int id)
        {
            return await _unitOfWork.AnalysisResult.GetByIdAsync(id);
        }

        public async Task AddAsync(AnalysisResult analysisResult)
        {
            await _unitOfWork.AnalysisResult.AddAsync(analysisResult);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(AnalysisResult analysisResult)
        {
            await _unitOfWork.AnalysisResult.UpdateAsync(analysisResult);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var analysisResult = await _unitOfWork.AnalysisResult.GetByIdAsync(id);
            if (analysisResult != null)
            {
                await _unitOfWork.AnalysisResult.RemoveAsync(analysisResult);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
