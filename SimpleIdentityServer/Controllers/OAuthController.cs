using Microsoft.AspNetCore.Mvc;
using SimpleIdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using SimpleIdentityServer.Data;
using Microsoft.AspNetCore.Authorization;

namespace SimpleIdentityServer.Controllers
{
    [ApiController]
    [Route("oauth")]
    public class ConnectController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public ConnectController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("authorize")]
        [Authorize]
        public async Task<IActionResult> Authorize(string response_type,
            string client_id,
            string redirect_uri,
            string state,
            string scope,
            string? code_challenge = null,
            string? code_challenge_method = null)
        {
            if (response_type != "code" || !InMemoryStore.Clients.ContainsKey(client_id))
                return BadRequest("Invalid client");

            // PKCE: Require code_challenge for public clients (or all clients if desired)
            if (string.IsNullOrEmpty(code_challenge))
                return BadRequest("Missing code_challenge for PKCE");

            // Generate code and store code_challenge and method with it
            var code = InMemoryStore.GenerateCode(User.Identity.Name);
            InMemoryStore.StoreCodeChallenge(code, scope, code_challenge, code_challenge_method);

            var redirect = $"{redirect_uri}?code={code}&state={state}";
            return Redirect(redirect);
        }

        [HttpPost("token")]
        [HttpGet("token")]
        public async Task<IActionResult> Token([FromForm] string grant_type,
                           [FromForm] string? code,
                           [FromForm] string? refresh_token,
                           [FromForm] string? client_id,
                           [FromForm] string? client_secret,
                           [FromForm] string? redirect_uri,
                           [FromForm] string? code_verifier)
        {
            string? userName = null;
            string scope = null;

            // Check client_id
            if (!string.IsNullOrEmpty(client_id) && !InMemoryStore.Clients.ContainsKey(client_id))
                return BadRequest("Invalid client_id");

            // Check client_secret for confidential clients
            if (!string.IsNullOrEmpty(client_secret) && InMemoryStore.ClientSecrets.TryGetValue(client_id, out var expectedSecret))
            {
                if (string.IsNullOrEmpty(client_secret) || client_secret != expectedSecret)
                    return BadRequest("Invalid client_secret");
            }

            // Check redirect_uri
            var registeredRedirectUri = InMemoryStore.Clients[client_id];
            if (string.IsNullOrEmpty(redirect_uri) || !string.Equals(redirect_uri, registeredRedirectUri, StringComparison.OrdinalIgnoreCase))
                return BadRequest("Invalid redirect_uri");

            if (grant_type == "authorization_code" && code != null)
            {
                // PKCE: Validate code_verifier
                var codeChallengeInfo = InMemoryStore.GetCodeChallenge(code);
                if (codeChallengeInfo != null)
                {
                    (scope, var codeChallenge, var codeChallengeMethod) = codeChallengeInfo.Value;
                    if (string.IsNullOrEmpty(code_verifier))
                        return BadRequest("Missing code_verifier for PKCE");
                    bool valid = false;
                    if (string.IsNullOrEmpty(codeChallengeMethod) || codeChallengeMethod == "plain")
                    {
                        valid = code_verifier == codeChallenge;
                    }
                    else if (codeChallengeMethod == "S256")
                    {
                        using (var sha256 = System.Security.Cryptography.SHA256.Create())
                        {
                            var hash = sha256.ComputeHash(System.Text.Encoding.ASCII.GetBytes(code_verifier));
                            var codeVerifierHash = System.Convert.ToBase64String(hash)
                                .TrimEnd('=')
                                .Replace('+', '-')
                                .Replace('/', '_');
                            valid = codeVerifierHash == codeChallenge;
                        }
                    }
                    if (!valid)
                        return BadRequest("Invalid code_verifier for PKCE");
                }
                userName = InMemoryStore.GetUserByCode(code);
            }
            else if (grant_type == "refresh_token" && refresh_token != null)
            {
                (scope, userName) = InMemoryStore.GetUserByRefreshToken(refresh_token).Value;
            }

            if (userName == null) return BadRequest("Invalid credentials");

            var user = await _userManager.FindByNameAsync(userName);
            var claims = InMemoryStore.GetUserClaims(user, scope);

            var token = JwtHelper.GenerateToken(client_id, claims);
            var newRefreshToken = InMemoryStore.GenerateRefreshToken(scope, userName);

            return Ok(new
            {
                access_token = token,
                token_type = "bearer",
                expires_in = 900,
                refresh_token = newRefreshToken
            });
        }

        [HttpPost("userinfo")]
        [HttpGet("userinfo")]
        public IActionResult UserInfo()
        {
            // Extract access token from Authorization header
            var authHeader = Request.Headers["Authorization"].ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized(new { error = "invalid_token" });
            }
            var token = authHeader.Substring("Bearer ".Length).Trim();

            // Validate the token and get user info (pseudo-code, replace with your logic)
            var user = JwtHelper.ValidateAccessTokenAndGetUser(token);
            if (user == null)
            {
                return Unauthorized(new { error = "invalid_token" });
            }

            // Return claims as per OpenID Connect spec
            return Ok(new
            {
                sub = user.ContainsKey("sub") ? user["sub"] : null,
                name = user.ContainsKey("name") ? user["name"] : null,
                email = user.ContainsKey("email") ? user["email"] : null
            });
        }
    }
}
