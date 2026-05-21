using SeaHawkServices.Domain.Entities;

namespace SeaHawkServices.Application.Services.Interface
{
    public interface IUserLoginHistoryService
    {
        Task<string?> LogAsync(ApplicationUser? user, string ip, string agent, bool isSuccess);
        Task<IEnumerable<UserLoginHistory>> GetAllAsync();
        Task<IEnumerable<UserLoginHistory>> GetFilteredAsync(string userName, DateTime? from, DateTime? to);

        Task<int> AddLoginAsync(UserLoginHistory history);
        Task<bool> MarkLogoutAsync(string sessionId, string logoutType = "Manual");
    }

}
