using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApp.Areas.Identity.ViewModels;
using User = Domain.Identity.User;

namespace WebApp.Areas.Identity.Controllers;

[Area("Identity")]
public class AccountController : Controller
{
    private readonly SignInManager<User> _signInManager;

    public AccountController(SignInManager<User> signInManager)
    {
        _signInManager = signInManager;
    }

    [HttpGet]
    public async Task<IActionResult> Login(string? returnUrl = null, string? errorMessage = null)
    {
        if (!string.IsNullOrEmpty(errorMessage))
        {
            ModelState.AddModelError(string.Empty, errorMessage);
        }

        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

        return View(new LoginModel
        {
            ReturnUrl = returnUrl,
        });
    }

    [HttpPost]
    [ActionName(nameof(Login))]
    public async Task<IActionResult> LoginPost([FromForm] LoginModel input)
    {
        if (!ModelState.IsValid)
        {
            return View(input);
        }

        var user = await _signInManager.UserManager.FindByNameAsync(input.UserName);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Invalid login attempt");
            return View(input);
        }

        var result = await _signInManager.PasswordSignInAsync(user, input.Password, input.RememberMe, false);
        if (result.Succeeded) return Redirect(input.ReturnUrl ?? Url.Content("~/"));

        ModelState.AddModelError(string.Empty, "Invalid login attempt");
        return View(input);
    }

    [HttpGet]
    public IActionResult Register(string? returnUrl = null)
    {
        return View(new RegisterModel
        {
            ReturnUrl = returnUrl,
        });
    }

    [HttpPost]
    [ActionName(nameof(Register))]
    public async Task<IActionResult> RegisterPost([FromForm] RegisterModel input)
    {
        if (!ModelState.IsValid) return View(input);

        var result =
            await _signInManager.UserManager.CreateAsync(new User { UserName = input.UserName }, input.Password);
        if (result.Succeeded)
        {
            var user = await _signInManager.UserManager.FindByNameAsync(input.UserName);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Failed to create user");
                return View(input);
            }

            await _signInManager.PasswordSignInAsync(user, input.Password, false, false);
            return Redirect(input.ReturnUrl ?? Url.Content("~/"));
        }

        foreach (var error in result.Errors)
        {
            ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(input);
    }

    public IActionResult Logout()
    {
        return View();
    }

    [HttpPost]
    [ActionName(nameof(Logout))]
    public async Task<IActionResult> LogoutPost()
    {
        await _signInManager.SignOutAsync();
        return Redirect(Url.Content("~/"));
    }
}