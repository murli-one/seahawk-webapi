using Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Infrastructure.Data;
using SendGrid.Helpers.Mail;
using System.Data;
using System.Numerics;

namespace SeaHawkServices.Infrastructure.Repository
{
    public class VesselUserRepository : Repository<VesselUser>, IVesselUserRepository
    {
        private readonly ApplicationDbContext _context;

        public VesselUserRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<VesselUser>> GetVesselUserAsync()
        {
            return await _context.VesselUser
                .Include(vu => vu.User)
                .Include(vu => vu.VesselDetail)
                .ToListAsync();
        }


        //public async Task<bool> AssignUserToVesselAsync(int vesselId, string userId)
        //{
        //    // If mapping already exists for this vessel + user, don't duplicate
        //    var alreadyAssigned = await _context.VesselUser
        //        .AnyAsync(vu => vu.VesselDetailId == vesselId && vu.UserId == userId);

        //    if (alreadyAssigned)
        //        return false;

        //    var user = await _context.Users
        //        .AsNoTracking()
        //        .FirstOrDefaultAsync(x => x.Id == userId);

        //    if (user == null)
        //        return false;

        //    // Optional: only allow VesselUser role to be assigned here
        //    if (user.Role != Domain.Entities.Enums.Role.VesselUser)
        //        return false;

        //    var vesselUser = new VesselUser
        //    {
        //        VesselDetailId = vesselId,
        //        UserId = userId
        //    };

        //    _context.VesselUser.Add(vesselUser);
        //    await _context.SaveChangesAsync();

        //    return true;
        //}
        public async Task<bool> AssignUserToVesselAsync(int vesselId, string userId)
        {
            // ✅ Only block if same user is already assigned to the SAME vessel
            var exists = await _context.VesselUser
                .AnyAsync(vu => vu.VesselDetailId == vesselId && vu.UserId == userId);

            if (exists)
                return false;

            var vesselUser = new VesselUser
            {
                VesselDetailId = vesselId,
                UserId = userId
            };

            await _context.VesselUser.AddAsync(vesselUser);
            await _context.SaveChangesAsync();

            return true;
        }





        public async Task RemoveUserFromVesselAsync(int vesselUserId)
        {
            var entity = await _context.VesselUser.FindAsync(vesselUserId);
            if (entity != null)
            {
                _context.VesselUser.Remove(entity);
                await _context.SaveChangesAsync();
            }
        }
    }
}
