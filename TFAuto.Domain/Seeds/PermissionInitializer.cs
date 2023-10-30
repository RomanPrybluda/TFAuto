using Microsoft.Azure.CosmosRepository;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;

namespace TFAuto.Domain.Seeds;

public class PermissionInitializer
{
    private readonly IRepository<Permission> _permissionRepository;

    public PermissionInitializer(IRepository<Permission> permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }
    public async Task InitializePermissions()
    {
        List<Permission> permissions = new()
            {
                new Permission ()
                {
                    Id = PermissionId.READ_ARTICLES,
                    PermissionName = PermissionNames.READ_ARTICLES,
                    RoleIds = new List<string> {RoleId.USER, RoleId.AUTHOR, RoleId.SUPER_ADMIN}
                },

                new Permission ()
                {
                    Id = PermissionId.EDIT_ARTICLES,
                    PermissionName = PermissionNames.EDIT_ARTICLES,
                    RoleIds = new List<string> {RoleId.AUTHOR, RoleId.SUPER_ADMIN}
                },

                new Permission ()
                {
                    Id = PermissionId.MANAGE_ARTICLES,
                    PermissionName = PermissionNames.MANAGE_ARTICLES,
                    RoleIds = new List<string> {RoleId.SUPER_ADMIN}
                },

                new Permission ()
                {
                    Id = PermissionId.MANAGE_ROLES,
                    PermissionName = PermissionNames.MANAGE_ROLES,
                    RoleIds = new List<string> {RoleId.SUPER_ADMIN}
                },

                new Permission ()
                {
                    Id = PermissionId.DELETE_COMMENT,
                    PermissionName = PermissionNames.DELETE_COMMENT,
                    RoleIds = new List<string> { RoleId.AUTHOR, RoleId.SUPER_ADMIN}
                },

                new Permission ()
                {
                    Id = PermissionId.MANAGE_USERS,
                    PermissionName = PermissionNames.MANAGE_USERS,
                    RoleIds = new List<string> {RoleId.SUPER_ADMIN}
                },
            };

        foreach (var permission in permissions)
        {
            var existingRole = await _permissionRepository.ExistsAsync(permission.Id, nameof(Permission));

            if (!existingRole)
            {
                await _permissionRepository.CreateAsync(permission);
            }
        }
    }
}
