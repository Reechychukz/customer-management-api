using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
	public class LGAMapper: Profile
	{
		public LGAMapper()
		{
            CreateMap<LGA, LGADto>();
        }
	}
}

