using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Models;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class BaseController : Controller
    {
        protected readonly UserManager<IdentityUser> _userManager;

        public BaseController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        protected async Task<IdentityUser> GetCurrentUserAsync()
        {
            return await _userManager.GetUserAsync(User);
        }

        protected IActionResult HandleIdentityErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            return View();
        }
    }
}