
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace CreatorHub.Web.Controllers;

public class LanguageController : Controller
{
    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult SetLanguage(string culture, string? returnUrl)
    {
        if (culture != "de-DE" && culture != "en-US")
        {
            return BadRequest();
        }

        Response.Cookies.Append(
            CookieRequestCultureProvider.DefaultCookieName,
            CookieRequestCultureProvider.MakeCookieValue(
                new RequestCulture(culture)),
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                IsEssential = true,
                Secure = Request.IsHttps,
                HttpOnly = true,
                SameSite = SameSiteMode.Lax
            });

        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            return RedirectToAction("Index", "Home");
        }

        return LocalRedirect(returnUrl);
    }
}
