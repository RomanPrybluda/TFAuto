using TFAuto.Domain.Services.Authentication.Models.Request;

namespace TFAuto.Domain.Services.Authentication;

public interface IAuthenticationService
{
    public ValueTask<LoginResponse> LogInAsync(LoginRequest loginCredentials);

    public ValueTask<LoginResponse> GetNewTokensByRefreshAsync(RefreshRequest refreshToken);
}
