using TFAuto.Domain.Services.Admin.DTO.Request;
using TFAuto.Domain.Services.Admin.DTO.Response;

namespace TFAuto.Domain.Services.Admin
{
    public interface IAdminService
    {
        ValueTask<GetAllUsersResponse> GetAllUsersAsync(GetUsersPaginationRequest paginationRequest);

        ValueTask<GetUserResponse> ChangeUserRoleAsync(Guid userId, string userNewRole);

        ValueTask DeleteUserAsync(Guid userId);
    }
}