using Data;
using SeaHawkServices.Application.Common.Interfaces;
using SeaHawkServices.Application.Services.Interface;

public class RoleService : IRoleService
{
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<List<Roles>> GetAllAsync()
    {
        var items = await _unitOfWork.Roles.GetAllAsync(tracked: false);
        return items.ToList();
    }

    public Task<Roles?> GetByIdAsync(int id)
    {
        return _unitOfWork.Roles.GetAsync(r => r.Id == id);
    }

    public async Task<string?> CreateAsync(string roleName, string? description)
    {
        if (string.IsNullOrWhiteSpace(roleName))
            return "Role name is required.";

        roleName = roleName.Trim();

        if (await _unitOfWork.Roles.ExistsAsync(roleName))
            return $"Role '{roleName}' already exists.";

        var entity = new Roles
        {
            RoleName = roleName,
            Description = description?.Trim()
        };

        await _unitOfWork.Roles.AddAsync(entity);
        await _unitOfWork.SaveAsync();
        return null;
    }

    public async Task<string?> UpdateAsync(Roles Role)
    {
        if (string.IsNullOrWhiteSpace(Role.RoleName))
            return "Role name is required.";

        Role.RoleName = Role.RoleName.Trim();

        var entity = await _unitOfWork.Roles.GetAsync(r => r.Id == Role.Id, tracked: true);
        if (entity == null)
            return "Role not found.";

        var existing = await _unitOfWork.Roles.GetByNameAsync(Role.RoleName);
        if (existing != null && existing.Id != Role.Id)
            return $"Role '{Role.RoleName}' already exists.";

        entity.RoleName = Role.RoleName;
        entity.Description = Role.Description?.Trim();

        await _unitOfWork.SaveAsync();
        return null;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _unitOfWork.Roles.GetAsync(r => r.Id == id, tracked: true);
        if (entity == null) return;

        await _unitOfWork.Roles.RemoveAsync(entity);
        await _unitOfWork.SaveAsync();
    }
}
