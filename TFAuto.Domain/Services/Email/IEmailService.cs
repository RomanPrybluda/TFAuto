using TFAuto.Domain.Services.Email.Models.Request;
using TFAuto.Domain.Services.Email.Models.Response;

namespace TFAuto.Domain.Services.Email
{
    public interface IEmailService
    {
        ValueTask SendConfirmationEmailAsync(string userEmail, string confirmationLink);

        ValueTask SendPasswordResetEmailAsync(string userEmail, string resetToken, string resetLink);

        ValueTask<ContactUsResponse> SendContactUsEmailAsync(ContactUsRequest contactUseRequest);
    }
}