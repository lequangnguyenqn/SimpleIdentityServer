using Microsoft.AspNetCore.Mvc;
using SimpleIdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using SimpleIdentityServer.Data;
using Microsoft.AspNetCore.Authorization;

namespace SimpleIdentityServer.Controllers
{
    [ApiController]
    [Route("connect")]
    public class ConnectController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public ConnectController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("authorize")]
        [Authorize]
        public async Task<IActionResult> Authorize(string response_type, string client_id, string redirect_uri, string state)
        {
            if (response_type != "code" || !InMemoryStore.Clients.ContainsKey(client_id))
                return BadRequest("Invalid client");
            var code = InMemoryStore.GenerateCode(User.Identity.Name);
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
                           [FromForm] string? scope,
                           [FromForm] string? code_verifier,
                           [FromForm] string? username,
                           [FromForm] string? password)
        {
            string? userName = null;

            if (grant_type == "authorization_code" && code != null)
            {
                userName = InMemoryStore.GetUserByCode(code);
            }
            else if (grant_type == "refresh_token" && refresh_token != null)
            {
                userName = InMemoryStore.GetUserByRefreshToken(refresh_token);
            }

            if (userName == null) return BadRequest("Invalid credentials");

            var user = await _userManager.FindByNameAsync(userName);
            var claims = InMemoryStore.GetUserClaims(user, scope);

            var token = JwtHelper.GenerateToken(userName, claims);
            var newRefreshToken = InMemoryStore.GenerateRefreshToken(userName);

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
