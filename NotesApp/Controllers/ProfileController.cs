using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class ProfileController : BaseController
    {
        public ProfileController(UserManager<IdentityUser> userManager) : base(userManager)
        {
        }

        public async Task<IActionResult> Index()
        {
            var user = await GetCurrentUserAsync();
            return user == null ? NotFound() : View(user);
        }
    }
}