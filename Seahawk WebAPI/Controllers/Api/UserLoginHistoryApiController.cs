using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Seahawk_WebAPI.Contracts.UserLoginHistory;
using SeaHawkServices.Application.Services.Interface;
using SeaHawkServices.Domain.Entities;
using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Controllers.Api;

[ApiController]
[Route("api/user-login-history")]
[Authorize]
public class UserLoginHistoryApiController : ControllerBase
{
    private readonly IUserLoginHistoryService _loginHistoryService;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserLoginHistoryApiController(
        IUserLoginHistoryService loginHistoryService,
        UserManager<ApplicationUser> userManager)
    {
        _loginHistoryService = loginHistoryService;
        _userManager = userManager;
    }

    // GET: /api/user-login-history?page=1&pageSize=15&filterUserName=test&filterRole=SystemAdmin
    [HttpGet]
    public async Task<ActionResult<UserLoginHistoryListResponse>> GetLoginHistory(
        [FromQuery] UserLoginHistoryListRequest request)
    {
        if (request.Page < 1)
            request.Page = 1;

        if (request.PageSize < 1)
            request.PageSize = 15;

        var currentUser = await _userManager.GetUserAsync(User);

        if (currentUser == null)
        {
            return Unauthorized(new
            {
                message = "User is not logged in."
            });
        }

        if (!IsSystemAdmin(currentUser))
        {
            return Forbid();
        }

        var all = await _loginHistoryService.GetAllAsync();

        var query = all.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.FilterUserName))
        {
            var name = request.FilterUserName.Trim();

            query = query.Where(x =>
                !string.IsNullOrEmpty(x.UserName) &&
                x.UserName.Contains(name, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrWhiteSpace(request.FilterRole))
        {
            var role = request.FilterRole.Trim();

            query = query.Where(x =>
                !string.IsNullOrEmpty(x.Role) &&
                x.Role.Contains(role, StringComparison.OrdinalIgnoreCase));
        }

        if (request.FilterFromDate.HasValue)
        {
            var from = request.FilterFromDate.Value.Date;
            query = query.Where(x => x.LoginTimeUtc >= from);
        }

        if (request.FilterToDate.HasValue)
        {
            var to = request.FilterToDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(x => x.LoginTimeUtc <= to);
        }

        query = query.OrderByDescending(x => x.LoginTimeUtc);

        var totalCount = query.Count();

        var pageItems = query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        var response = new UserLoginHistoryListResponse
        {
            CurrentUser = currentUser.UserName,
            CurrentUserRole = currentUser.Role.ToString(),

            FilterUserName = request.FilterUserName,
            FilterRole = request.FilterRole,
            FilterFromDate = request.FilterFromDate,
            FilterToDate = request.FilterToDate,

            PageNumber = request.Page,
            PageSize = request.PageSize,
            TotalCount = totalCount,

            LoginHistoryList = pageItems.Select(ToDto).ToList()
        };

        return Ok(response);
    }

    private static bool IsSystemAdmin(ApplicationUser user)
    {
        return user != null && user.Role == Role.SystemAdmin;
    }

    private static UserLoginHistoryDto ToDto(UserLoginHistory history)
    {
        return new UserLoginHistoryDto
        {
            Id = history.Id,
            UserId = history.UserId,
            UserName = history.UserName,
            Role = history.Role,
            LoginTimeUtc = history.LoginTimeUtc,
            IpAddress = history.IpAddress,
            UserAgent = history.UserAgent
        };
    }
}