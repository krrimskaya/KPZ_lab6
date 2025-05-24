using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Models;

[Authorize]
[Route("Account")]
public class ProfileController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;

    public ProfileController(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    private async Task<IdentityUser> GetCurrentUserAsync()
      => await _userManager.GetUserAsync(User);

    [HttpGet("Profile")]
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

    [HttpPost("VerifyEmailPost")]
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
