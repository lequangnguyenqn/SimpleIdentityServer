using ClientApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;

namespace ClientApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login()
        {
            var state = Guid.NewGuid().ToString("N");
            var redirectUri = "http://localhost:5006/home/callback";

            var authUrl = $"http://localhost:5295/connect/authorize?response_type=code&client_id=client1&redirect_uri={HttpUtility.UrlEncode(redirectUri)}&state={state}";
            return Redirect(authUrl);
        }

        public async Task<IActionResult> Callback(string code, string state)
        {
            var tokenRequest = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("grant_type", "authorization_code"),
                new KeyValuePair<string, string>("code", code),
                new KeyValuePair<string, string>("redirect_uri", "http://localhost:5006/callback"),
                new KeyValuePair<string, string>("client_id", "client1"),
                new KeyValuePair<string, string>("scope", "profile email")
            });

            var client = new HttpClient();
            var response = await client.PostAsync("http://localhost:5295/connect/token", tokenRequest);
            var content = await response.Content.ReadAsStringAsync();

            ViewData["Token"] = content;
            return View("Token");
        }

    }

}
