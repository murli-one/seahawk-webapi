namespace Seahawk_WebAPI.Contracts.Roles;

public sealed class RoleListResponse
{
    public string CurrentTab { get; set; } = "Roles";
    public List<RoleDto> Items { get; set; } = new();
}

public sealed class RoleDto
{
    public int Id { get; set; }
    public string? RoleName { get; set; }
    public string? Description { get; set; }
}

public sealed class RoleUpsertRequest
{
    public string? RoleName { get; set; }
    public string? Description { get; set; }
}

public sealed class RoleMessageResponse
{
    public string Message { get; set; } = string.Empty;
}