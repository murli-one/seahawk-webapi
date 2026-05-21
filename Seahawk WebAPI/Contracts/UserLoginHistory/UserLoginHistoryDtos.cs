namespace Seahawk_WebAPI.Contracts.UserLoginHistory;

public class UserLoginHistoryListRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;

    public string? FilterUserName { get; set; }
    public string? FilterRole { get; set; }

    public DateTime? FilterFromDate { get; set; }
    public DateTime? FilterToDate { get; set; }
}

public class UserLoginHistoryListResponse
{
    public string? CurrentUser { get; set; }
    public string? CurrentUserRole { get; set; }

    public string? FilterUserName { get; set; }
    public string? FilterRole { get; set; }

    public DateTime? FilterFromDate { get; set; }
    public DateTime? FilterToDate { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    public List<UserLoginHistoryDto> LoginHistoryList { get; set; } = new();
}

public class UserLoginHistoryDto
{
    public int Id { get; set; }

    public string? UserId { get; set; }
    public string? UserName { get; set; }
    public string? Role { get; set; }

    public DateTime LoginTimeUtc { get; set; }

    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}