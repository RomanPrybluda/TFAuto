using Microsoft.Azure.CosmosRepository;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;

namespace TFAuto.Domain.Seeds
{
    public class RoleInitializer
    {
        private readonly IRepository<Role> _roleRepository;

        public RoleInitializer(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }
        public async Task InitializeRoles()
        {
            List<Role> roles = new()
            {
                new Role ()
                {
                    Id = RoleId.SUPER_ADMIN,
                    RoleName = RoleNames.SUPER_ADMIN,
                    PermissionIds = new List<string>
                    {
                        PermissionId.READ_ARTICLES,
                        PermissionId.MANAGE_ARTICLES,
                        PermissionId.MANAGE_ROLES,
                        PermissionId.EDIT_ARTICLES,
                        PermissionId.DELETE_COMMENT,
                        PermissionId.MANAGE_USERS
                    }
                },

                new Role ()
                {
                    Id = RoleId.AUTHOR,
                    RoleName = RoleNames.AUTHOR,
                    PermissionIds = new List<string>
                    {
                        PermissionId.READ_ARTICLES,
                        PermissionId.EDIT_ARTICLES,
                        PermissionId.DELETE_COMMENT
                    }
                },

                new Role ()
                {
                    Id = RoleId.USER,
                    RoleName = RoleNames.USER,
                    PermissionIds = new List<string>
                    {
                        PermissionId.READ_ARTICLES
                    }
                },
            };

            foreach (var role in roles)
            {
                var existingRole = await _roleRepository.ExistsAsync(role.Id, nameof(Role));

                if (!existingRole)
                {
                    await _roleRepository.CreateAsync(role);
                }
            }
        }
    }
}