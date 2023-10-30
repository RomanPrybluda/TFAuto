using TFAuto.Domain.Services.UserInfo.DTO;

namespace TFAuto.Domain.Services.UserInfo
{
    public interface IUserInfoService
    {
        ValueTask<InfoUserResponse> GetUserInfo(Guid userId);
    }
}