using interTekLabERP.ViewModels;
using interTekLabERP.Business.Services;
using interTekLabERP.Controllers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace interTekLabERP.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(
        IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = _userService.Login(model.UserName, model.Password);

        if (user == null)
        {
            ModelState.AddModelError(string.Empty,
                "Kullanıcı adı veya şifre hatalı.");

            return View(model);
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
        new Claim(ClaimTypes.Name, user.FullName),
        new Claim(ClaimTypes.Role, user.Role),
        new Claim("UserName", user.UserName)
    };

        var claimsIdentity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var authProperties = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8)
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            new ClaimsPrincipal(claimsIdentity),
            authProperties);

        return RedirectToAction(nameof(HomeController.Index), "Home");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme);

        HttpContext.Session.Clear();

        return RedirectToAction(
            "Login",
            "Account");
    }
}