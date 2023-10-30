using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Services.UserInfo;
using TFAuto.Domain.Services.UserInfo.DTO;
using TFAuto.Domain.Services.UserPassword;
using TFAuto.Domain.Services.UserPassword.DTO;
using TFAuto.Domain.Services.UserUpdate;
using TFAuto.Domain.Services.UserUpdate.DTO;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Route("users")]

    public class UserController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IUserUpdateInfoService _userUpdateInfoService;
        private readonly IUserPasswordService _userPasswordService;

        public UserController(
            IUserInfoService userInfoService,
            IUserUpdateInfoService userUpdateInfoService,
            IUserPasswordService userPasswordService)
        {
            _userInfoService = userInfoService;
            _userUpdateInfoService = userUpdateInfoService;
            _userPasswordService = userPasswordService;
        }

        [HttpGet("{id:Guid}")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(InfoUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<InfoUserResponse>> GetUserInfo([Required] Guid id)
        {
            var user = await _userInfoService.GetUserInfo(id);
            return Ok(user);
        }

        [HttpPut("{id:Guid}")]
        [Authorize]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UpdateUserInfoResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<UpdateUserInfoResponse>> UpdateUserInfo([Required] Guid id, [FromQuery] UserUpdateInfoRequest updateInfo)
        {
            var user = await _userUpdateInfoService.UpdateUserInfo(id, updateInfo);
            return Ok(user);
        }

        [HttpPost]
        [Route("forgot-password")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ForgotPasswordResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordRequest request)
        {
            string callingUrl = Request.GetTypedHeaders().Referer?.AbsoluteUri;
            var response = await _userPasswordService.ForgotPasswordAsync(request, callingUrl);
            return Ok(response);
        }

        [HttpPost]
        [Route("reset-password")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ResetPasswordResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest request)
        {
            var response = await _userPasswordService.ResetPasswordAsync(request);
            return Ok(response);
        }
    }
}