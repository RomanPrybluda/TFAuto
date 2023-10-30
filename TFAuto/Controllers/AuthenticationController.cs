using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain;
using TFAuto.Domain.Services.Authentication;
using TFAuto.Domain.Services.Authentication.Models.Request;

namespace TFAuto.WebApp.Controllers;

[Route("authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _userService;

    public AuthenticationController(IAuthenticationService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    [SwaggerOperation(
    Summary = "Login authentication",
    Description = "Logs in with user's credentials and returns access and refresh tokens for authentication")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(LoginResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<LoginResponse>> LogInAsync([FromBody] LoginRequest loginCredentials)
    {
        var createdLogin = await _userService.LogInAsync(loginCredentials);
        return Ok(createdLogin);
    }

    [HttpPost("refresh-token")]
    [Authorize]
    [SwaggerOperation(
    Summary = "Get new tokens with Refresh token",
    Description = "Using valid refresh token get new pair of valid Access/Refresh tokens")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(LoginResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<LoginResponse>> GetNewTokensByRefreshAsync([FromBody] RefreshRequest refreshToken)
    {
        var createdLogin = await _userService.GetNewTokensByRefreshAsync(refreshToken);
        return Ok(createdLogin);
    }
}
