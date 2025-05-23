using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotesApp.Models;
using NotesApp.Services.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        public async Task<IActionResult> Index(int? tagId)
        {
            var histories = await _historyService.GetAllAsync(tagId);

            // для фільтрації списку тегів у ViewBag
            var tags = histories
                .SelectMany(h => h.Note.NoteTags.Select(nt => nt.Tag))
                .Distinct()
                .ToList();

            ViewBag.Tags = tags;
            ViewBag.SelectedTagId = tagId;
            return View(histories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAll()
        {
            await _historyService.ClearAllAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearByTag(int tagId)
        {
            await _historyService.ClearByTagAsync(tagId);
            return RedirectToAction(nameof(Index));
        }
    }
}
