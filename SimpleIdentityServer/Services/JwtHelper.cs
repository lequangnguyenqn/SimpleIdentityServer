using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

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
    }

}
