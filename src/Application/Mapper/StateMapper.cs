using Application.DTOs;
using AutoMapper;
using Domain.Entities;

namespace Application.Mapper
{
	public class StateMapper: Profile
	{
		public StateMapper()
		{
            CreateMap<State, StateDto>();
        }
	}
}

