using Application.DTOs;
using Application.Helpers;
using Application.Services.Interfaces;
using Domain.Common;
using Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace Application.Services.Implementations
{
    public class JwtAuthenticationManager : IJwtAuthenticationManager
    {
        private readonly JwtConfigSettings _jwtConfigSettings;

        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
        private readonly SecurityKey _issuerSigningKey;
        private readonly SigningCredentials _signingCredentials;
        private readonly JwtHeader _jwtHeader;
        private readonly TokenValidationParameters _tokenValidationParameters;
        public JwtAuthenticationManager(IOptions<JwtConfigSettings> jwtConfigSettings)
        {
            _jwtConfigSettings = jwtConfigSettings.Value;
            _issuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfigSettings.Secret));
            _signingCredentials = new SigningCredentials(_issuerSigningKey, SecurityAlgorithms.HmacSha256);
            _jwtHeader = new JwtHeader(_signingCredentials);
            _tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                ValidateAudience = false,
                ValidateIssuer = false,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_jwtConfigSettings.Secret))
            };
        }

        public TokenReturnHelper Authenticate(User user, IList<string> roles = null)
        {
            var roleClaims = new List<Claim>();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypeHelper.Email, user.Email),
                new Claim(ClaimTypeHelper.UserId, user.Id.ToString()),
            };

            foreach (var role in roles)
            {
                roleClaims.Add(new Claim(ClaimTypeHelper.Role, role));
            }
            claims.AddRange(roleClaims);

            var nowUtc = DateTimeOffset.UtcNow;
            var expires = nowUtc.AddMinutes(_jwtConfigSettings.TokenLifespan);
            var centuryBegins = new DateTime(1970, 1, 1).ToUniversalTime();
            var exp = (long)new TimeSpan(expires.Ticks - centuryBegins.Ticks).TotalSeconds;
            var now = (long)new TimeSpan(nowUtc.Ticks - centuryBegins.Ticks).TotalSeconds;

            var payload = new JwtPayload
            {
                {"Id", user.Id},
                {"issuer", _jwtConfigSettings.ValidIssuer},
                {"iat", now},
                {"exp", exp},
                {"unique_name", user.Email},
                {"email", user.Email},
                {"phone", user.PhoneNumber},
                {"fulldetails", $"{user.Email} {user.PhoneNumber}"}
            };

            if (roleClaims != null && roleClaims.Count > 0)
                payload.Add("roles", roles);

            var jwt = new JwtSecurityToken(_jwtHeader, payload);
            var token = _jwtSecurityTokenHandler.WriteToken(jwt);

            return new TokenReturnHelper
            {
                ExpiresIn = expires,
                AccessToken = token,
                EmailAddress = user.Email,
            };
        }

        public string GenerateRefreshToken(Guid userId)
        {
            var jwtUserSecret = _jwtConfigSettings.Secret; //jwtSettings.GetSection("Secret").Value;
            var tokenHandler = new JwtSecurityTokenHandler();

            var key = Encoding.ASCII.GetBytes(jwtUserSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypeHelper.UserId, userId.ToString())
                }),
                Expires = DateTime.UtcNow.AddMinutes(300),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var jwt = tokenHandler.WriteToken(token);

            return jwt;
        }

        public bool ValidateRefreshToken(string refreshToken)
        {
            //var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = _jwtConfigSettings.Secret; //jwtSettings.GetSection("Secret").Value;

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtUserSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(refreshToken, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                return false;
            }

            var expiryAt = jwtSecurityToken.ValidTo;
            if (DateTime.UtcNow > expiryAt)
                return false;
            return true;
        }

        public Guid GetUserIdFromAccessToken(string accessToken)
        {
            //var jwtSettings = _configuration.GetSection("JwtSettings");
            var jwtUserSecret = _jwtConfigSettings.Secret; //jwtSettings.GetSection("Secret").Value;

            var tokenValidationParamters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtUserSecret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParamters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                                                    StringComparison.InvariantCultureIgnoreCase))
            {
                throw new RestException(HttpStatusCode.BadRequest, ResponseMessages.InvalidToken);
            }

            var userId = principal.FindFirst(ClaimTypeHelper.UserId)?.Value;

            if (userId == null)
                throw new RestException(HttpStatusCode.BadRequest, $"{ResponseMessages.MissingClaim} {ClaimTypeHelper.UserId}");

            return Guid.Parse(userId);
        }
    }

    public class TokenReturnHelper
    {
        public string AccessToken { get; set; }
        public DateTimeOffset ExpiresIn { get; set; }
        public string EmailAddress { get; set; }
    }
}

