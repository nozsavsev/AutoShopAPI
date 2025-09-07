using AutoMapper;
using AutoShopAPI.Models;
using AutoShopAPI.Models.DTOs;

namespace AutoShopAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Car mappings
            CreateMap<Car, CarDTO>();
            CreateMap<Car, CarBasicDTO>();
            CreateMap<CreateUpdateCarDTO, Car>();

            // User mappings
            CreateMap<User, UserDTO>();
            CreateMap<User, UserBasicDTO>();
            CreateMap<CreateUserDTO, User>();
            CreateMap<UpdateUserDTO, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
