using TFAuto.Domain.Services.UserPassword.DTO;

namespace TFAuto.Domain.Services.UserPassword
{
    public interface IUserPasswordService
    {
        ValueTask<ForgotPasswordResponse> ForgotPasswordAsync(ForgotPasswordRequest request, string baseUrl);

        ValueTask<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}