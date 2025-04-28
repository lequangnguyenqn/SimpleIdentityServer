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
    }
}
