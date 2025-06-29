using Microsoft.IdentityModel.Tokens;
using System.Security.Cryptography;

namespace SimpleIdentityServer.Services
{
    public static class RsaKeyService
    {
        private static readonly RSA _rsa = RSA.Create(2048);

        public static RsaSecurityKey Key { get; } = new RsaSecurityKey(_rsa)
        {
            KeyId = Guid.NewGuid().ToString("N")
        };

        public static SigningCredentials GetSigningCredentials()
        {
            return new SigningCredentials(Key, SecurityAlgorithms.RsaSha256);
        }

        public static JsonWebKey GetJsonWebKey()
        {
            return JsonWebKeyConverter.ConvertFromRSASecurityKey(Key);
        }

        public static TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = "http://localhost:5295",
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = Key,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromMinutes(1)
            };
        }
    }
}
