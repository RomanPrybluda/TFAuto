using AutoMapper;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Services.Roles.DTO;

namespace TFAuto.Domain;

public class RoleUserMapper : Profile
{
    public RoleUserMapper()
    {
        CreateMap<Role, RoleListResponse>();

        CreateMap<Role, RoleResponse>();

        CreateMap<RoleCreateRequest, Role>();
        CreateMap<Role, RoleCreateResponse>();

        CreateMap<RoleUpdateRequest, Role>();
        CreateMap<Role, RoleUpdateResponse>();
    }
}