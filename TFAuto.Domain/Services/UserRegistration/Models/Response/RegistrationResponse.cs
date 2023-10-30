using TFAuto.Domain.Services.Authentication.Models;

namespace TFAuto.Domain.Services.UserRegistration.Models.Response;

public class RegistrationResponse
{
    public UserRegistrationResponse User { get; set; }

    public Token Tokens { get; set; }
}
