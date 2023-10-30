using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services.Roles;
using TFAuto.Domain.Services.Roles.DTO;

namespace TFAuto.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("roles")]
    [Authorize(Policy = PermissionId.MANAGE_ARTICLES)]

    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleRepository;

        public RoleController(IRoleService roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RoleListResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<RoleListResponse>> GetRoles()
        {
            var roles = await _roleRepository.GetRolesAsync();
            return Ok(roles);
        }

        [HttpGet("{id:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RoleResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<RoleResponse>> GetRole([Required] Guid id)
        {
            var role = await _roleRepository.GetRoleAsync(id);
            return Ok(role);
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RoleCreateResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<RoleCreateResponse>> AddRole([FromBody] RoleCreateRequest newRole)
        {
            var role = await _roleRepository.AddRoleAsync(newRole);
            return Ok(role);
        }

        [HttpPut("{id:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RoleUpdateResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<RoleUpdateResponse>> UpdateRole([Required] Guid id, [FromBody] RoleUpdateRequest updatedRole)
        {
            var role = await _roleRepository.UpdateRoleAsync(id, updatedRole);
            return Ok(role);
        }

        [HttpDelete("{id:Guid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<IActionResult> DeleteRole([Required] Guid id)
        {
            await _roleRepository.DeleteRoleAsync(id);
            return NoContent();
        }
    }
}
