using TFAuto.Domain.Services.UserRegistration.Models.Request;
using TFAuto.Domain.Services.UserRegistration.Models.Response;

namespace TFAuto.Domain;

public interface IRegistrationService
{
    ValueTask<ConfirmRegistrationResponse> ConfirmEmailAsync(ConfirmRegistrationRequest userRequest, string baseUrl);

    ValueTask<RegistrationResponse> RegisterUserAsync(RegistrationRequest confirmEmailRequest);
}
