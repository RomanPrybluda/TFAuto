using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using SendGrid.Helpers.Errors.Model;
using TFAuto.Domain.Services.UserUpdate.DTO;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.UserUpdate
{
    public class UserUpdateInfoService : IUserUpdateInfoService
    {
        private readonly IRepository<User> _repositoryUser;
        private readonly IMapper _mapper;

        public UserUpdateInfoService(IRepository<User> repositoryUser, IMapper mapper)
        {
            _repositoryUser = repositoryUser;
            _mapper = mapper;
        }

        public async ValueTask<UpdateUserInfoResponse> UpdateUserInfo(Guid userId, UserUpdateInfoRequest updateInfo)
        {
            var user = await _repositoryUser.GetAsync(t => t.Id == userId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var userMapped = _mapper.Map<User>(updateInfo);
            var updatedUser = await _repositoryUser.UpdateAsync(userMapped);
            var userUpdatedInfo = _mapper.Map<UpdateUserInfoResponse>(updatedUser);

            return userUpdatedInfo;
        }
    }
}