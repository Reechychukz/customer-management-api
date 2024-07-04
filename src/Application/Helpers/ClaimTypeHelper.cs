using System.Security.Claims;

namespace Application.Helpers
{
    public class ClaimTypeHelper
    {
        public static string UserId { get; set; } = "UserId";
        public static string Email { get; set; } = "Email";
        public static string Role { get; set; } = ClaimTypes.Role;
    }
}

