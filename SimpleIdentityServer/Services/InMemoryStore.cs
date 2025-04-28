using Microsoft.AspNetCore.Identity;
using SimpleIdentityServer.Data;
using System.Security.Claims;

namespace SimpleIdentityServer.Services
{
    public static class InMemoryStore
    {
        public static Dictionary<string, string> AuthorizationCodes = new();
        public static Dictionary<string, string> RefreshTokens = new();

        public static Dictionary<string, string> Clients = new()
        {
            { "client1", "https://localhost:5002/callback" },
            { "machine_client", null }
        };

        public static Dictionary<string, string> ClientSecrets = new()
        {
            { "machine_client", "supersecret123" }
        };

        public static List<string> ValidScopes = new() { "profile", "email" };

        public static Dictionary<string, Dictionary<string, string>> Users = new()
        {
            { "test", new Dictionary<string, string> {
                { "sub", "user1" },
                { "name", "Test User" },
                { "email", "test@example.com" }
            }}
        };

        public static string GenerateCode(string userId)
        {
            var code = Guid.NewGuid().ToString("N");
            AuthorizationCodes[code] = userId;
            return code;
        }

        public static string? GetUserByCode(string code)
        {
            if (AuthorizationCodes.TryGetValue(code, out var userId))
            {
                AuthorizationCodes.Remove(code);
                return userId;
            }

            return null;
        }

        public static string GenerateRefreshToken(string userId)
        {
            var token = Guid.NewGuid().ToString("N");
            RefreshTokens[token] = userId;
            return token;
        }

        public static string? GetUserByRefreshToken(string token)
        {
            RefreshTokens.TryGetValue(token, out var user);
            return user;
        }

        public static IEnumerable<Claim> GetUserClaims(User user, string scope)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtClaimTypes.Subject, user.Id)
            };
            var requestedScopes = (scope ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var validScopes = requestedScopes.Intersect(InMemoryStore.ValidScopes).ToList();

            foreach (var s in validScopes)
            {
                if (s == "profile") claims.Add(new Claim("name", user.UserName));
                if (s == "email") claims.Add(new Claim("email", user.Email));
            }
            return claims;
        }
    }
}
