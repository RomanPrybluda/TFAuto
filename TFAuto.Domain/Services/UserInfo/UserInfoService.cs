using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using SendGrid.Helpers.Errors.Model;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Services.UserInfo.DTO;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.UserInfo
{
    public class UserInfoService : IUserInfoService
    {
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<Role> _repositoryRole;

        public UserInfoService(
            IRepository<User> repositoryUser,
            IRepository<Role> repositoryRole)
        {
            _repositoryUser = repositoryUser;
            _repositoryRole = repositoryRole;
        }

        public async ValueTask<InfoUserResponse> GetUserInfo(Guid userId)
        {
            var user = await _repositoryUser.GetAsync(
                t => t.Id == userId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var role = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));

            if (role == null)
                throw new NotFoundException(ErrorMessages.ROLES_NOT_FOUND);

            var userInfo = new InfoUserResponse
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = role.RoleName
            };

            return userInfo;
        }
    }
}