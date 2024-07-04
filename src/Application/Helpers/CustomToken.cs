using Domain.Entities;

namespace Application.Helpers
{
    public class CustomToken
    {
        public static string GenerateToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
        public static string GenerateOtp()
        {
            Random rnd = new Random();
            var randomNumber = (rnd.Next(100000, 999999)).ToString();
            return randomNumber;
        }

        public static bool IsTokenValid(Token token, double tokenLifeSpan)
        {
            var expiry = token.CreatedAt.AddHours(tokenLifeSpan);
            if (DateTime.UtcNow > expiry)
                return false;

            return true;
        }
    }
}

