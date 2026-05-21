using static SeaHawkServices.Domain.Entities.Enums;

namespace Seahawk_WebAPI.Contracts.VesselUsers;

public class VesselUserListRequest
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 15;

    public string? FilterUserName { get; set; }
    public string? FilterVesselName { get; set; }
}
public sealed class UnassignedVesselUsersResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages { get; set; }

    public string? Search { get; set; }

    public string? SelectedUser { get; set; }
    public string? SelectedUserEmail { get; set; }
    public string? SelectedUserRole { get; set; }

    public List<SelectOptionDto> Items { get; set; } = new();
}
public class VesselUserListResponse
{
    public List<VesselUserRowDto> VesselUserGrouped { get; set; } = new();

    public List<SelectOptionDto> Vessels { get; set; } = new();
    public List<SelectOptionDto> ApplicationUsers { get; set; } = new();

    public Dictionary<string, string> UserVesselNames { get; set; } = new();

    public string? FilterUserName { get; set; }
    public string? FilterVesselName { get; set; }

    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }

    public int TotalPages =>
        PageSize == 0 ? 0 : (int)Math.Ceiling((double)TotalCount / PageSize);

    public string SelectedUser { get; set; } = "";
    public string SelectedUserEmail { get; set; } = "";
    public string SelectedUserRole { get; set; } = "";
}

public class VesselUserRowDto
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";
    public string? UserName { get; set; }
    public string? Name { get; set; }
    public string? Email { get; set; }

    public int VesselDetailId { get; set; }
    public string? VesselName { get; set; }
    public string? IMONumber { get; set; }

    public string? VesselsText { get; set; }
}

public class VesselUserEditResponse
{
    public int VesselUserId { get; set; }

    public string UserId { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string? PhoneNumber { get; set; }

    public Role Role { get; set; }
    public string RoleText { get; set; } = "";

    public bool IsApprove { get; set; }

    public DateTime? LastLoginAtUtc { get; set; }
    public DateTime? LastActivityAtUtc { get; set; }

    public List<AssignedVesselDto> AssignedVessels { get; set; } = new();
    public List<SelectOptionDto> VesselOptions { get; set; } = new();
}

public class AssignedVesselDto
{
    public int VesselUserMappingId { get; set; }
    public int VesselId { get; set; }

    public string? VesselName { get; set; }
    public string? IMONumber { get; set; }
}

public class UpdateVesselUserRequest
{
    public string UserId { get; set; } = "";

    public string? Name { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }

    public Role Role { get; set; }
    public bool IsApprove { get; set; }
}

public class AssignVesselUserRequest
{
    public string? SelectedUser { get; set; }
    public List<int> SelectedVesselIds { get; set; } = new();
}

public class AddVesselToUserRequest
{
    public string UserId { get; set; } = "";
    public List<int> SelectedVesselIds { get; set; } = new();
}

public class RemoveVesselFromUserRequest
{
    public int VesselUserMappingId { get; set; }
    public int EditId { get; set; }
    public string UserId { get; set; } = "";
}

public class VesselUserApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";

    public int SuccessCount { get; set; }
    public int FailedCount { get; set; }

    public int? NextEditId { get; set; }
}

public class SelectOptionDto
{
    public string Value { get; set; } = "";
    public string Text { get; set; } = "";
}