using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SimpleIdentityServer.Data;
using System.Linq;

namespace SimpleIdentityServer.Services
{
    public static class JwtHelper
    {
        public static string GenerateToken(string userId, IEnumerable<Claim> claims, int expiresMinutes = 15)
        {
            var creds = RsaKeyService.GetSigningCredentials();

            var token = new JwtSecurityToken(
                issuer: "https://localhost:5001",
                audience: "client1",
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
                if (string.IsNullOrEmpty(sub)) return null;
                // Find user by sub claim in the Users dictionary
                return InMemoryStore.Users.Values.FirstOrDefault(u => u.ContainsKey("sub") && u["sub"] == sub);
            }
            catch
            {
                return null;
            }
        }
    }

}
