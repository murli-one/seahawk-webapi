using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.Users;

public class UserListRequest
{
    public string? Tab { get; set; } = "approved";
    public string? Search { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 13;
}

public class UserListResponse
{
    public List<UserDto> Users { get; set; } = new();

    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? UserRole { get; set; }

    public string CurrentTab { get; set; } = "approved";
    public string? Search { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);
}

public class UserDto
{
    public string Id { get; set; } = "";

    public string? Name { get; set; }
    public string? UserName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public Role Role { get; set; }
    public string RoleText { get; set; } = "";

    public bool IsApprove { get; set; }
    public bool IsDeleted { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? Address { get; set; }
    public string? CompanyName { get; set; }
}

public class CreateUserRequest
{
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public int SelectedRole { get; set; }

    public int? SelectedCompanyId { get; set; }
    public int? SelectedVesselId { get; set; }

    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}
public class UpdateUserRequest
{
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public int SelectedRole { get; set; }

    public int? SelectedCompanyId { get; set; }
    public int? SelectedVesselId { get; set; }

    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class ChangePasswordRequest
{
    public string? Password { get; set; }
    public string? ConfirmPassword { get; set; }
}

public class UserEditOptionsResponse
{
    public UserDto User { get; set; } = new();
    public bool IsApprovedUser { get; set; }

    public List<SelectOptionDto> Vessels { get; set; } = new();
    public List<SelectOptionDto> Companies { get; set; } = new();
}

public class SelectOptionDto
{
    public string Value { get; set; } = "";
    public string Text { get; set; } = "";
}

public class UserApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
}