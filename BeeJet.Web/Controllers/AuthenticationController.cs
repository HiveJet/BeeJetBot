using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace BeeJet.Web.Controllers
{
    [ApiController]
    public class AuthenticationController : ControllerBase
    {

        [ResponseCache(NoStore = true, Duration = 0)]
        [HttpGet("~/signin/{DiscordId}")]
        public IActionResult SignIn(string DiscordId)
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "/steam-login/" + DiscordId,
                IsPersistent = true,
                IssuedUtc = DateTime.UtcNow,
                ExpiresUtc = DateTime.UtcNow.AddDays(7)
            }, "Steam");
        }
    }
}
