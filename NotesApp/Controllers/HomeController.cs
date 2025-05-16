using Microsoft.AspNetCore.Mvc;

namespace NotesApp.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            // Просто перенаправляємо на Notes/Index, бо вся логіка нотаток там
            return RedirectToAction("Index", "Notes");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }
}
