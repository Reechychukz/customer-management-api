using Application.Helpers;
using Domain.Entities;
using Application.Services.Implementations;

namespace Application.Services.Interfaces
{
    public interface IJwtAuthenticationManager : IAutoDependencyService
    {
        TokenReturnHelper Authenticate(User user, IList<string> roles = null);
        Guid GetUserIdFromAccessToken(string accessToken);
        string GenerateRefreshToken(Guid userId);
        bool ValidateRefreshToken(string refreshToken);
    }
}

