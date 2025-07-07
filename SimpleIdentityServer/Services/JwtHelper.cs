using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SimpleIdentityServer.Data;
using System.Linq;

namespace SimpleIdentityServer.Services
{
    public static class JwtHelper
    {
        public static string GenerateToken(string client_id, IEnumerable<Claim> claims, int expiresMinutes = 15)
        {
            var creds = RsaKeyService.GetSigningCredentials();

            var token = new JwtSecurityToken(
                issuer: "http://localhost:5295",
                audience: client_id,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public static Dictionary<string, string>? ValidateAccessTokenAndGetUser(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = RsaKeyService.GetTokenValidationParameters();
            try
            {
                var principal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                var sub = principal.FindFirst("sub")?.Value;
                return principal.Claims
                    .Select(c => new KeyValuePair<string, string>(
                        c.Type == ClaimTypes.Email ? "email" :
                        c.Type == ClaimTypes.NameIdentifier ? "sub" :
                        c.Type,
                        c.Value))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
            catch
            {
                return null;
            }
        }
    }

}
