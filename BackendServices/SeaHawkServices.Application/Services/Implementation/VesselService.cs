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
    public class VesselService : IVesselService
    {
        private readonly IVesselRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public VesselService(IVesselRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<VesselDetail>> GetAllAsync()
        {
            return await _repository.GetAllAsync();
        }

        public async Task<VesselDetail?> GetByIdAsync(int id)
        {
            return await _repository.GetByIdAsync(id);
        }

        public async Task AddAsync(VesselDetail Vessel)
        {
            await _repository.AddAsync(Vessel);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var Vessel = await _repository.GetByIdAsync(id);
            if (Vessel != null)
            {
                await _repository.RemoveAsync(Vessel);
                await _unitOfWork.SaveAsync();
            }
        }
    }
}
