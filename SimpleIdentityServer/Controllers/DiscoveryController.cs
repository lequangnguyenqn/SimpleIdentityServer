using Microsoft.AspNetCore.Mvc;
using SimpleIdentityServer.Services;

namespace SimpleIdentityServer.Controllers
{
    [ApiController]
    [Route(".well-known/openid-configuration")]
    public class DiscoveryController : ControllerBase
    {
        [HttpGet]
        public IActionResult OpenIdConfiguration()
        {
            var baseUrl = "http://localhost:5295";

            return Ok(new
            {
                issuer = baseUrl,
                authorization_endpoint = $"{baseUrl}/oauth/authorize",
                token_endpoint = $"{baseUrl}/oauth/token",
                userinfo_endpoint = $"{baseUrl}/oauth/userinfo",
                jwks_uri = $"{baseUrl}/.well-known/jwks.json",
                response_types_supported = new[] { "code" },
                subject_types_supported = new[] { "public" },
                id_token_signing_alg_values_supported = new[] { "RS256" },
                token_endpoint_auth_methods_supported = new[] { "client_secret_post" },
                scopes_supported = InMemoryStore.ValidScopes,
                claims_supported = new[] { "sub", "name", "email" },
                grant_types_supported = new[] { "authorization_code", "refresh_token" }
            });
        }

        [HttpGet("jwks")]
        public IActionResult Jwks()
        {
            var key = RsaKeyService.GetJsonWebKey();
            return Ok(new { keys = new[] { key } });
        }
    }
}
