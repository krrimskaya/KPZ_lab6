using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace NotesApp.Controllers
{
    [Authorize]
    [Route("Account")]
    public class UserController : Controller
    {
        [HttpGet("GetCurrentUserId")]
        public IActionResult GetCurrentUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("Не вдалося отримати ID користувача");

            return Ok(userId);
        }
    }

}
