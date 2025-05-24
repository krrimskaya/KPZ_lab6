using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Models;
using NotesApp.Services;
using System;

[AllowAnonymous]
[Route("Account")]
public class AuthController : Controller
{
    private readonly AuthService _authService;
    private readonly UserManager<IdentityUser> _userManager;

    public AuthController(AuthService authService, UserManager<IdentityUser> userManager)
    {
        _authService = authService;
        _userManager = userManager;
    }

    [HttpGet("Register")]
    public IActionResult Register() => View();

    [HttpPost("Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid) return View(model);

        var result = await _authService.RegisterUserAsync(model.Email, model.Password);
        if (!result.Succeeded) return View(model);

        await _authService.LoginAsync(model.Email, model.Password, false);
        return RedirectToAction("Index", "Home");
    }

    [HttpGet("Login")]
    public IActionResult Login(string returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost("Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        returnUrl ??= Url.Content("~/");

        if (!ModelState.IsValid) return View(model);

        var result = await _authService.LoginAsync(model.Email, model.Password, model.RememberMe);
        if (!result.Succeeded)
        {
            ModelState.AddModelError(string.Empty, "Невдала спроба входу");
            return View(model);
        }

        return LocalRedirect(returnUrl);
    }

    [HttpPost("Logout")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _authService.LogoutAsync();
        return RedirectToAction("Index", "Home");
    }
}
