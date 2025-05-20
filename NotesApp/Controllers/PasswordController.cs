using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;

using NotesApp.Models;
using NotesApp.Services;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [AllowAnonymous]
    public class PasswordController : BaseController
    {
        private readonly PasswordService _passwordService;
        private readonly AuthService _authService;

        public PasswordController(
      UserManager<IdentityUser> userManager,
      PasswordService passwordService,
      AuthService authService) : base(userManager)
        {
            _passwordService = passwordService;
            _authService = authService;
        }


        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
            {
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var code = await _passwordService.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = Url.Action("ResetPassword", "Password", new { userId = user.Id, code }, Request.Scheme);


            return RedirectToAction("ForgotPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string code = null)
        {
            return code == null
                ? BadRequest("Для скидання пароля необхідно надати код.")
                : View(new ResetPasswordViewModel { Code = code });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var result = await _passwordService.ResetPasswordAsync(model.Email, model.Code, model.Password);
            if (!result.Succeeded) return HandleIdentityErrors(result);

            return RedirectToAction("ResetPasswordConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await GetCurrentUserAsync();
            if (user == null) return NotFound();

            var result = await _passwordService.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
            if (!result.Succeeded) return HandleIdentityErrors(result);

            await _authService.LogoutAsync();
            return RedirectToAction("ChangePasswordConfirmation");
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePasswordConfirmation()
        {
            return View();
        }
    }
}