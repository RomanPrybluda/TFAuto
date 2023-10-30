using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services.Admin;
using TFAuto.Domain.Services.Admin.DTO.Request;
using TFAuto.Domain.Services.Admin.DTO.Response;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("admin")]
    [Authorize(Policy = PermissionId.MANAGE_USERS)]

    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllUsersResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetAllUsersResponse>> GetAllUsersAsync([FromQuery] GetUsersPaginationRequest paginationRquest)
        {
            var users = await _adminService.GetAllUsersAsync(paginationRquest);
            return Ok(users);
        }

        [HttpPut("users/{id:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetUserResponse>> ChangeUserRoleAsync([Required] Guid id, [Required] string userNewRole)
        {
            var user = await _adminService.ChangeUserRoleAsync(id, userNewRole);
            return Ok(user);
        }

        [HttpDelete("users/{id:Guid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<IActionResult> DeleteUserAsync([Required] Guid id)
        {
            await _adminService.DeleteUserAsync(id);
            return NoContent();
        }

    }
}