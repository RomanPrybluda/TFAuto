using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain;
using TFAuto.Domain.Services.UserRegistration.Models.Request;
using TFAuto.Domain.Services.UserRegistration.Models.Response;

namespace TFAuto.WebApp;

[ApiController]
[Route("registration")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _registrationService;

    public RegistrationController(IRegistrationService registrationServics)
    {
        _registrationService = registrationServics;
    }

    [HttpPost("confirm-email")]
    [SwaggerOperation(
     Summary = "Saves user's info and sends email with link to confirm one",
     Description = "Returns message about sent email")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ConfirmRegistrationResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<ConfirmRegistrationResponse>> ConfirmEmailAsync([FromBody] ConfirmRegistrationRequest userRequest)
    {
        string callingUrl = Request.GetTypedHeaders().Referer?.AbsoluteUri;
        var userResponse = await _registrationService.ConfirmEmailAsync(userRequest, callingUrl);
        return Ok(userResponse);
    }

    [HttpPost("users")]
    [SwaggerOperation(
     Summary = "Registers user after opening the confirmation link sent in the email ",
     Description = "Returns saved user with authentication tokens")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(RegistrationResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<RegistrationResponse>> RegisterUserAsync([FromQuery] RegistrationRequest confirmEmailRequest)
    {
        var userResponse = await _registrationService.RegisterUserAsync(confirmEmailRequest);
        return Ok(userResponse);
    }
}
