using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Implementation
{
    public class UserLoginHistoryService : IUserLoginHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public UserLoginHistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        // Returns sessionId only when success
        public async Task<string?> LogAsync(ApplicationUser? user, string ip, string agent, bool isSuccess)
        {
            // If you want to log "user not found" too, then allow null user but store username separately.
            if (user == null) return null;

            var sessionId = isSuccess ? Guid.NewGuid().ToString("N") : null;

            var record = new UserLoginHistory
            {
                UserId = user.Id,
                UserName = user.UserName,
                Role = user.Role.ToString(),
                LoginTimeUtc = DateTime.UtcNow,
                IpAddress = ip ?? "",
                UserAgent = agent ?? "",
                IsSuccess = isSuccess,

                // NEW fields
                SessionId = sessionId,
                LogoutTimeUtc = null,
                LogoutType = null
            };

            await _unitOfWork.UserLoginHistory.AddAsync(record);
            await _unitOfWork.SaveAsync();

            return sessionId;
        }

        public async Task<IEnumerable<UserLoginHistory>> GetAllAsync()
        {
            return await _unitOfWork.UserLoginHistory.GetAllAsync(tracked: false);
        }

        public async Task<IEnumerable<UserLoginHistory>> GetFilteredAsync(string userName, DateTime? from, DateTime? to)
        {
            // NOTE: This still loads all data into memory.
            // Better is DB-level filtering, but keeping your current pattern.
            var all = await GetAllAsync();
            var q = all.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userName))
                q = q.Where(x => !string.IsNullOrEmpty(x.UserName) &&
                                 x.UserName.Contains(userName, StringComparison.OrdinalIgnoreCase));

            if (from.HasValue)
                q = q.Where(x => x.LoginTimeUtc >= from.Value);

            if (to.HasValue)
                q = q.Where(x => x.LoginTimeUtc <= to.Value);

            return q.OrderByDescending(x => x.LoginTimeUtc).ToList();
        }

        public async Task<int> AddLoginAsync(UserLoginHistory history)
        {
            await _unitOfWork.UserLoginHistory.AddAsync(history);
            await _unitOfWork.SaveAsync();
            return history.Id;
        }

        public async Task<bool> MarkLogoutAsync(string sessionId, string logoutType = "Manual")
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                return false;

            var rows = await _unitOfWork.UserLoginHistory
                .GetAllAsync(x => x.SessionId == sessionId && x.LogoutTimeUtc == null, tracked: true);

            var userSession = rows
                .OrderByDescending(x => x.LoginTimeUtc)
                .FirstOrDefault();

            if (userSession == null)
                return false;

            userSession.LogoutTimeUtc = DateTime.UtcNow;
            userSession.LogoutType = logoutType;

            // If your repo has Update(), call it; otherwise tracked entity will update on SaveAsync()
            // _unitOfWork.UserLoginHistory.Update(userSession);

            await _unitOfWork.SaveAsync();
            return true;
        }
    }
}
