using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mapper
{
	public class UserMapper: Profile
	{
        public UserMapper()
        {
            CreateMap<UserSignupDto, User>()
                .AfterMap((src, dest) =>
                {
                    dest.Email = src.Email.Trim().ToLower();
                });

            CreateMap<User, UserByIdDto>();

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.Name))
                .ForMember(dest => dest.LGA, opt => opt.MapFrom(src => src.LGA.Name));

            CreateMap<User, UserLoginResponse>().ReverseMap();

        }
    }
}

