using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Services.Roles.DTO;

namespace TFAuto.Domain.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async ValueTask<IEnumerable<RoleListResponse>> GetRolesAsync()
        {
            var roleList = await _roleRepository.GetAsync(t => t.Type == nameof(Role));

            if (roleList == null)
                throw new ValidationException(ErrorMessages.ROLES_NOT_FOUND);

            var roleExistsList = _mapper.Map<IEnumerable<RoleListResponse>>(roleList);

            return roleExistsList;
        }

        public async ValueTask<RoleResponse> GetRoleAsync(Guid id)
        {
            var role = await _roleRepository.TryGetAsync(id.ToString(), nameof(Role));

            if (role == null)
                throw new ValidationException(ErrorMessages.ROLE_NOT_FOUND);

            var roleExists = _mapper.Map<RoleResponse>(role);

            return roleExists;
        }

        public async ValueTask<RoleCreateResponse> AddRoleAsync(RoleCreateRequest newRole)
        {
            var role = await _roleRepository.GetAsync(t => t.RoleName == newRole.RoleName);

            if (role.Any())
                throw new ValidationException(ErrorMessages.ROLE_ALREADY_EXISTS);

            var roleMapped = _mapper.Map<Role>(newRole);
            var createdRole = await _roleRepository.CreateAsync(roleMapped);
            var newRoleResponse = _mapper.Map<RoleCreateResponse>(roleMapped);

            return newRoleResponse;
        }

        public async ValueTask<RoleUpdateResponse> UpdateRoleAsync(Guid id, RoleUpdateRequest updatedRole)
        {
            var role = await _roleRepository.TryGetAsync(id.ToString(), nameof(Role));

            if (role == null)
                throw new ValidationException(ErrorMessages.ROLE_NOT_FOUND);

            role.RoleName = updatedRole.RoleName;
            await _roleRepository.UpdateAsync(role);
            var updatedRoleResponse = _mapper.Map<RoleUpdateResponse>(role);

            return updatedRoleResponse;
        }

        public async ValueTask DeleteRoleAsync(Guid id)
        {
            var role = await _roleRepository.TryGetAsync(id.ToString(), nameof(Role));

            if (role == null)
                throw new ValidationException(ErrorMessages.ROLE_NOT_FOUND);

            await _roleRepository.DeleteAsync(role);
        }
    }
}