using ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;

namespace ClientApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _identityServerUrl;

        public HomeController(IConfiguration configuration)
        {
            _identityServerUrl = configuration["IdentityServerUrl"] ?? "http://localhost:5295";
        }
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            var state = Guid.NewGuid().ToString("N");
            HttpContext.Session.SetString("state", state);
            var redirectUri = "http://localhost:5006/home/callback";

            // PKCE: generate code_verifier and code_challenge
            var codeVerifier = GenerateCodeVerifier();
            var codeChallenge = GenerateCodeChallenge(codeVerifier);
            HttpContext.Session.SetString("code_verifier", codeVerifier);

            var authUrl = $"http://localhost:5295/oauth/authorize?response_type=code&client_id=aspnetcore_client&scope={HttpUtility.UrlEncode("profile email")}&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&state={state}&code_challenge={codeChallenge}&code_challenge_method=S256";
            return Redirect(authUrl);
        }

        public async Task<IActionResult> Callback(string code,
            string state,
            string scope)
        {
            var codeVerifier = HttpContext.Session.GetString("code_verifier");
            var state_verifier = HttpContext.Session.GetString("state");
            if (!string.Equals(state_verifier, state, StringComparison.OrdinalIgnoreCase))
            {
                ViewData["Token"] = "State mismatch, possible CSRF attack.";
                return View("Token");
            }
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "http://localhost:5006/home/callback"),
                new KeyValuePair<string, string>("client_id", "aspnetcore_client"),
                new KeyValuePair<string, string>("client_secret", "supersecret123"),
                new KeyValuePair<string, string>("code_verifier", codeVerifier ?? "")
            });

            var client = new HttpClient();
            var response = await client.PostAsync($"{_identityServerUrl}/oauth/token", tokenRequest);
            var content = await response.Content.ReadAsStringAsync();

            ViewData["Token"] = content;
            return View("Token");
        }

        // PKCE helpers
        private static string GenerateCodeVerifier()
        {
            var bytes = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }
            return Base64UrlEncode(bytes);
        }

        private static string GenerateCodeChallenge(string codeVerifier)
        {
            using (var sha256 = System.Security.Cryptography.SHA256.Create())
            {
                var bytes = System.Text.Encoding.ASCII.GetBytes(codeVerifier);
                var hash = sha256.ComputeHash(bytes);
                return Base64UrlEncode(hash);
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            return System.Convert.ToBase64String(input)
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_');
        }

    }

}
