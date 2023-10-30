using AutoMapper;
using TFAuto.Domain.Services.UserUpdate.DTO;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Mappers
{
    public class UserUpdateInfoMapper : Profile
    {
        public UserUpdateInfoMapper()
        {
            CreateMap<UserUpdateInfoRequest, User>();
            CreateMap<User, UpdateUserInfoResponse>();
        }
    }
}