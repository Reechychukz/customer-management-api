using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
	public interface IStateService: IAutoDependencyService
    {
        Task<SuccessResponse<IEnumerable<StateDto>>> GetStates();
        Task<SuccessResponse<IEnumerable<LGADto>>> GetAllLGAByStateId(int? stateId);
    }
}

