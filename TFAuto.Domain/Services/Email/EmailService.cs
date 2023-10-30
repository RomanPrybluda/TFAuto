using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using TFAuto.Domain.Configurations;
using TFAuto.Domain.Services.Email.Models.Request;
using TFAuto.Domain.Services.Email.Models.Response;

namespace TFAuto.Domain.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly TokenSettings _passwordResetSettings;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
            _passwordResetSettings = configuration.GetSection("TokenSettings").Get<TokenSettings>();
        }

        public async ValueTask SendConfirmationEmailAsync(string userEmail, string confirmationLink)
        {
            var subject = "Welcome to TFAuto! Confirm Your Email";
            var body = $"<a href='{confirmationLink}'>Click here to confirm your email</a>";
            await SendEmailAsync(userEmail, subject, body);
        }

        public async ValueTask SendPasswordResetEmailAsync(string userEmail, string resetToken, string resetLink)
        {
            var subject = "TFAuto. Password Reset Request";
            var body = $"<p>Use the following code to reset your password: <strong>{resetToken}</strong></p><p><a href='{resetLink}'>Click here to reset your password</a></p>";
            await SendEmailAsync(userEmail, subject, body);
        }

        public async ValueTask<ContactUsResponse> SendContactUsEmailAsync(ContactUsRequest contactUsRequest)
        {
            var sendGridSettings = _configuration.GetSection("SendGridSettings").Get<SendGridSettings>();

            var subject = "TFAuto. Contact Us form";
            var body = $"<p><strong>User's name: </strong>{contactUsRequest.UserName}</p><p> " +
                $"<p><strong>User's email: </strong>{contactUsRequest.UserEmail}</p><p>" +
                $"<p><strong>Message: <p></p></strong>{contactUsRequest.Text}</p><p>";

            await SendEmailAsync(sendGridSettings.ContactUsEmail, subject, body);
            var contactUsResponse = new ContactUsResponse { Message = "Your form is sent, thanks for your involvement, we'll contact you soon!" };

            return contactUsResponse;
        }

        private async ValueTask SendEmailAsync(string userEmail, string subject, string body)
        {
            var sendGridSettings = _configuration.GetSection("SendGridSettings").Get<SendGridSettings>();

            var apiKey = sendGridSettings.ApiKey;
            var fromName = sendGridSettings.FromName;
            var fromEmail = sendGridSettings.FromEmail;

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(userEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, body);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                throw new Exception("Failed to send email.");
        }
    }
}