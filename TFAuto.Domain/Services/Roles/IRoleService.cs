using TFAuto.Domain.Services.Roles.DTO;

namespace TFAuto.Domain.Services.Roles
{
    public interface IRoleService
    {
        ValueTask<IEnumerable<RoleListResponse>> GetRolesAsync();

        ValueTask<RoleResponse> GetRoleAsync(Guid id);

        ValueTask<RoleCreateResponse> AddRoleAsync(RoleCreateRequest newRole);

        ValueTask<RoleUpdateResponse> UpdateRoleAsync(Guid id, RoleUpdateRequest updatedRole);

        ValueTask DeleteRoleAsync(Guid id);
    }
}
