using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SimpleIdentityServer.Data;
using SimpleIdentityServer.Services;

namespace SimpleIdentityServer.Controllers
{
    [Route("authorize")]
    [Authorize]
    public class AuthController : Controller
    {
        public AuthController()
        {
        }
        [HttpGet]
        public async Task<IActionResult> Authorize(string response_type, string client_id, string redirect_uri, string state)
        {
            if (response_type != "code" || !InMemoryStore.Clients.ContainsKey(client_id))
                return BadRequest("Invalid client");
            var code = InMemoryStore.GenerateCode(User.Identity.Name);
            var redirect = $"{redirect_uri}?code={code}&state={state}";
            return Redirect(redirect);
        }
    }

}
