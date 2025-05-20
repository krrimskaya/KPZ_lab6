using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Models;
using NotesApp.Services;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [AllowAnonymous]
    public class AccountController : BaseController
    {
        private readonly AuthService _authService;
        private readonly PasswordService _passwordService;

        public AccountController(
            UserManager<IdentityUser> userManager,
            AuthService authService,
            PasswordService passwordService) : base(userManager)
        {
            _authService = authService;
            _passwordService = passwordService;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _authService.RegisterUserAsync(model.Email, model.Password);
            if (!result.Succeeded) return HandleIdentityErrors(result);

            var user = await _userManager.FindByEmailAsync(model.Email);
            await _authService.LoginAsync(model.Email, model.Password, false);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return NotFound();

            return View(new ProfileViewModel
            {
                Username = user.UserName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed
            });
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyEmailPost()
        {
            var user = await GetCurrentUserAsync();
            if (user == null) return NotFound();

            user.EmailConfirmed = true;
            await _userManager.UpdateAsync(user);

            return RedirectToAction("Profile");
        }
    }
}