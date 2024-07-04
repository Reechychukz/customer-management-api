using Application.DTOs;
using Application.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Application.Services.Interfaces
{
    public interface IUserService: IAutoDependencyService
    {
        Task<SuccessResponse<UserDto>> CreateUser(UserSignupDto model, List<string> roles = null);
        Task<SuccessResponse<object>> CompleteUserOnboarding(VerifyTokenDTO token);
        Task<SuccessResponse<UserLoginResponse>> UserLogin(UserLoginDTO model);
        Task<SuccessResponse<UserByIdDto>> GetUserById(Guid userId);
        Task<PagedResponse<IEnumerable<UserDto>>> GetUsers(ResourceParameter parameter, string name, IUrlHelper urlHelper);
    }
}

