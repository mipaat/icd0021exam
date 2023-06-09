using AutoMapper;
using Public.DTO.Identity;

namespace Public.DTO;

public class AutoMapperConfig : Profile
{
    public AutoMapperConfig()
    {
        CreateMap<Domain.Identity.Role, Role>();
        CreateMap<Domain.Identity.User, User>();
        CreateMap<Domain.Identity.User, UserWithRoles>().ForMember(u => u.Roles, o =>
            o.MapFrom(u => u.UserRoles!.Select(ur => ur.Role!)));

        CreateMap<Domain.Product, Product>().ReverseMap();
        CreateMap<Domain.ProductExistence, ProductExistence>();
        CreateMap<Domain.Recipe, Recipe>();
    }
}