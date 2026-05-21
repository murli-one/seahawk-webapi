using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class VesselDetailService : IVesselDetailService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IVesselUserService _vesselUserService;

        public VesselDetailService(IUnitOfWork unitOfWork, IVesselUserService vesselUserService)
        {
            _unitOfWork = unitOfWork;
            _vesselUserService = vesselUserService;
        }

        public async Task<IEnumerable<VesselDetail>> GetAllAsync()
        {
            return await _unitOfWork.VesselDetail.GetAllAsync();
        }

        public async Task<VesselDetail?> GetByIdAsync(int id)
        {
            return await _unitOfWork.VesselDetail.GetByIdAsync(id);
        }

        public async Task AddAsync(VesselDetail vesseldetail)
        {
            await _unitOfWork.VesselDetail.AddAsync(vesseldetail);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateAsync(VesselDetail vesseldetail)
        {
            await _unitOfWork.VesselDetail.UpdateAsync(vesseldetail);
            await _unitOfWork.SaveAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var analysisResult = await _unitOfWork.VesselDetail.GetByIdAsync(id);
            if (analysisResult != null)
            {
                await _unitOfWork.VesselDetail.RemoveAsync(analysisResult);
                await _unitOfWork.SaveAsync();
            }
        }

        public async Task<IEnumerable<Company>> GetCompanies()
        {
            return await _unitOfWork.Companies.GetAllAsync();
        }

        // ======================================
        // Get vessels based on user + role
        // ======================================
        public async Task<IEnumerable<VesselDetail>> GetVesselsForUserAsync(
            string userId,
            int? vesselDetailsId,
            Enums.Role role)
        {
            // Load all vessels once (small table in most systems)
            var allVessels = await _unitOfWork.VesselDetail.GetAllAsync();

            // 1) System admin → everything
            if (role == Enums.Role.SystemAdmin)
            {
                return allVessels;
            }

            // 2) Management user → all vessels in their company
            if (role == Enums.Role.ManagementUser && vesselDetailsId.HasValue)
            {
                return allVessels.Where(v => v.Id == vesselDetailsId.Value);
            }

            // 3) Vessel user → only vessels assigned to them via VesselUser
            if (role == Enums.Role.VesselUser)
            {
                var allVesselUser = await _vesselUserService.GetVesselUserAsync();

                var vesselIds = allVesselUser
                    .Where(vu => vu.UserId == userId)
                    .Select(vu => vu.VesselDetailId)
                    .Distinct()
                    .ToList();

                if (!vesselIds.Any())
                    return Enumerable.Empty<VesselDetail>();

                return allVessels.Where(v => vesselIds.Contains(v.Id));
            }

            // 4) Any other roles → no vessels (or adjust as needed)
            return Enumerable.Empty<VesselDetail>();
        }
    }
}

