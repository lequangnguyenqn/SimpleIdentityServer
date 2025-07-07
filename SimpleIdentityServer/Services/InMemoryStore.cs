using Microsoft.AspNetCore.Identity;
using SimpleIdentityServer.Data;
using System.Security.Claims;

namespace SimpleIdentityServer.Services
{
    public static class InMemoryStore
    {
        public static Dictionary<string, string> AuthorizationCodes = new();
        public static Dictionary<string, (string scope, string user)> RefreshTokens = new();

        public static Dictionary<string, string> Clients = new()
        {
            { "aspnetcore_client", "http://localhost:5006/home/callback" },
            { "nextjs_client", "http://localhost:3000/api/auth/callback/simple-identity-server" }
        };

        public static Dictionary<string, string> ClientSecrets = new()
        {
            { "aspnetcore_client", "supersecret123" },
            { "nextjs_client", "supersecret123" }
        };

        public static List<string> ValidScopes = new() { "profile", "email" };

        public static Dictionary<string, (string scope, string codeChallenge, string? codeChallengeMethod)> CodeChallenges = new();

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

        public static string GenerateRefreshToken(string scope, string userId)
        {
            var token = Guid.NewGuid().ToString("N");
            RefreshTokens[token] = (scope, userId);
            return token;
        }

        public static (string scope, string user)? GetUserByRefreshToken(string token)
        {
            RefreshTokens.TryGetValue(token, out var value);
            return value;
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

        public static void StoreCodeChallenge(string code, string scope, string codeChallenge, string? codeChallengeMethod)
        {
            CodeChallenges[code] = (scope, codeChallenge, codeChallengeMethod);
        }

        public static (string scope, string codeChallenge, string? codeChallengeMethod)? GetCodeChallenge(string code)
        {
            if (CodeChallenges.TryGetValue(code, out var value))
            {
                return value;
            }
            return null;
        }
    }
}
