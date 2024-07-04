using System;
using System.Net;
using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Infrastructure.Repositories.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services.Implementations
{
	public class StateService : IStateService
	{
        private readonly IRepository<State> _stateRepository;
        private readonly IRepository<LGA> _lgaRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<LGA> _logger;

        public StateService(IRepository<State> stateRepository, IRepository<LGA> lgaRepository, IMapper mapper, ILogger<LGA> logger)
        {
            _stateRepository = stateRepository;
            _lgaRepository = lgaRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<SuccessResponse<IEnumerable<LGADto>>> GetAllLGAByStateId(int? stateId)
        {
            if (!stateId.HasValue)
                throw new RestException(HttpStatusCode.BadRequest, $"One or more validation error.");

            var requestByStateId = _lgaRepository.Query(x => x.StateId == stateId).AsEnumerable();

            var response = await Task.FromResult(_mapper.Map<IEnumerable<LGADto>>(requestByStateId));

            return new SuccessResponse<IEnumerable<LGADto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = response
            };

        }

        public async Task<SuccessResponse<IEnumerable<StateDto>>> GetStates()
        {

            var states = await _stateRepository.GetAllAsync();

            var statesResponse = _mapper.Map<IEnumerable<StateDto>>(states);

            return new SuccessResponse<IEnumerable<StateDto>>
            {
                Message = ResponseMessages.RetrievalSuccessResponse,
                Data = statesResponse
            };
        }
    }
}

