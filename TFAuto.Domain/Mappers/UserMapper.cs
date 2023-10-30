using AutoMapper;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<ConfirmRegistrationRequest, User>()
            .ForMember(dest => dest.Password, opt => opt.MapFrom(scr => BCrypt.Net.BCrypt.HashPassword(scr.Password)))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(scr => scr.Email.ToLower()));
        CreateMap<User, UserRegistrationResponse>();

        CreateMap<User, GetAuthorResponse>();
    }
}