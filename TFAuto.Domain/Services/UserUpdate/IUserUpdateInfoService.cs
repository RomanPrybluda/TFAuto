using TFAuto.Domain.Services.UserUpdate.DTO;

namespace TFAuto.Domain.Services.UserUpdate
{
    public interface IUserUpdateInfoService
    {
        ValueTask<UpdateUserInfoResponse> UpdateUserInfo(Guid userId, UserUpdateInfoRequest updateInfo);
    }
}