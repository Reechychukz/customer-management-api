using AutoMapper;
using Domain.Entities;
using Application.DTOs;

namespace Application.Mapper
{
	public class BankMapper: Profile
	{
		public BankMapper()
		{
            CreateMap<ExistingBank, ExistingBankDto>()
            .ForMember(dest => dest.bankName, opt => opt.MapFrom(src => src.BankName));
        }
	}
}

