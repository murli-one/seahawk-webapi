using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.CompanyUsers;

public sealed class CompanyUserQueryRequest
{
    public int? CompanyId { get; set; }

    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;

    public string? FilterUserName { get; set; }
    public string? FilterCompany { get; set; }
    public string? FilterVesselName { get; set; }
}
public sealed class UnassignedCompanyUsersResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? Search { get; set; }

    public string? SelectedUser { get; set; }
    public string? SelectedUserEmail { get; set; }
    public string? SelectedUserRole { get; set; }

    public List<ApplicationUserOptionDto> Items { get; set; } = new();
}
public sealed class CompanyUserPagedResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? SelectedUser { get; set; }
    public string? SelectedUserEmail { get; set; }
    public string? SelectedUserRole { get; set; }

    public int? SelectedCompanyId { get; set; }
    public string? FilterUserName { get; set; }
    public string? FilterCompany { get; set; }
    public string? FilterVesselName { get; set; }

    public List<CompanyUserDto> Items { get; set; } = new();
    public List<CompanyOptionDto> Companies { get; set; } = new();
    public List<ApplicationUserOptionDto> AvailableUsers { get; set; } = new();
}

public sealed class CompanyUserDto
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public string? CompanyName { get; set; }
    public string Vessels { get; set; } = "No Vessels Assigned";

    public DateTime? LastActivityAtUtc { get; set; }
    public string LastActivityText { get; set; } = "Never";
}

public sealed class CompanyOptionDto
{
    public int Id { get; set; }
    public string? CompanyName { get; set; }
}

public sealed class ApplicationUserOptionDto
{
    public string Id { get; set; } = string.Empty;
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }
    public Role Role { get; set; }
    public bool? IsApprove { get; set; }
}

public sealed class AssignCompanyUserRequest
{
    public int CompanyId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public sealed class RemoveCompanyUserRequest
{
    public int CompanyUserId { get; set; }
    public int CompanyId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public sealed class CompanyUserEditResponse
{
    public int CompanyUserId { get; set; }
    public string UserId { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public Role Role { get; set; }
    public string RoleName { get; set; } = string.Empty;

    public bool? IsApprove { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime? LastActivityAtUtc { get; set; }

    public List<AssignedCompanyRowDto> AssignedCompanies { get; set; } = new();
    public List<CompanyOptionDto> CompanyOptions { get; set; } = new();
}

public sealed class AssignedCompanyRowDto
{
    public int CompanyUserMappingId { get; set; }
    public int CompanyId { get; set; }
    public string CompanyName { get; set; } = string.Empty;
}

public sealed class UpdateCompanyUserRequest
{
    public string UserId { get; set; } = string.Empty;

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public Role Role { get; set; }
    public bool? IsApprove { get; set; }
}

public sealed class AddCompaniesToUserRequest
{
    public string UserId { get; set; } = string.Empty;
    public List<int> SelectedCompanyIds { get; set; } = new();
}

public sealed class RemoveCompanyFromUserRequest
{
    public int CompanyUserMappingId { get; set; }
    public int EditId { get; set; }
    public string UserId { get; set; } = string.Empty;
}

public sealed class CompanyUserMessageResponse
{
    public string Message { get; set; } = string.Empty;
    public int SuccessCount { get; set; }
    public int FailCount { get; set; }
    public int SkippedCount { get; set; }
}