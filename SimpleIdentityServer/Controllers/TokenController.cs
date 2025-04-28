using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleIdentityServer.Data;
using SimpleIdentityServer.Services;
using System.Security.Claims;
using System.Text;

namespace SimpleIdentityServer.Controllers
{
    [Route("token")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly UserManager<User> _userManager;

        public TokenController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        [HttpPost]
        public async Task<IActionResult> Token([FromForm] string grant_type,
                           [FromForm] string? code,
                           [FromForm] string? refresh_token,
                           [FromForm] string client_id,
                           [FromForm] string? client_secret,
                           [FromForm] string? redirect_uri,
                           [FromForm] string? scope)
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

    }

}
