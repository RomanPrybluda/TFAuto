using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain.Services.Email;
using TFAuto.Domain.Services.Email.Models.Request;
using TFAuto.Domain.Services.Email.Models.Response;
using TFAuto.Domain.Services.UserPassword.DTO;

namespace TFAuto.WebApp.Controllers;

[Route("contact-us")]
[ApiController]
public class ContactUsController : ControllerBase
{
    private readonly IEmailService _emailService;

    public ContactUsController(IEmailService emailService)
    {
        _emailService = emailService;
    }

    [HttpPost]
    [SwaggerOperation(
     Summary = "Send filled form by user via email")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(ContactUsResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<ForgotPasswordResponse>> SendContactUsEmailAsync(ContactUsRequest contactUsRequest)
    {
        ContactUsResponse contactUsResponse = await _emailService.SendContactUsEmailAsync(contactUsRequest);
        return Ok(contactUsResponse);
    }
}
