using Application.DTOs;
using Application.Helpers;

namespace Application.Services.Interfaces
{
	public interface IBankService: IAutoDependencyService
    {
        Task<SuccessResponse<IEnumerable<ExistingBankDto>>> GetExistingBanks();
    }
}

