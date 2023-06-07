using AutoMapper;
using Public.DTO.Identity;

namespace Public.DTO;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Domain.Identity.Role, Role>();
        CreateMap<Domain.Identity.User, User>();
        CreateMap<Domain.Identity.User, UserWithRoles>();
    }
}