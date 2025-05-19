using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class HistoryController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HistoryController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? tagId)
        {
            IQueryable<NoteHistory> query = _context.NoteHistories
                .Include(nh => nh.Note)
                .ThenInclude(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag);

            if (tagId.HasValue)
            {
                query = query.Where(nh => nh.Note.NoteTags.Any(nt => nt.TagId == tagId));
            }

            var histories = await query.OrderByDescending(nh => nh.ChangedAt).ToListAsync();
            
            var tags = await _context.Tags.ToListAsync();
            ViewBag.Tags = tags;
            ViewBag.SelectedTagId = tagId;

            return View(histories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearAll()
        {
            var allHistory = await _context.NoteHistories.ToListAsync();
            _context.NoteHistories.RemoveRange(allHistory);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearByTag(int tagId)
        {
            var historiesToDelete = await _context.NoteHistories
                .Include(nh => nh.Note)
                .ThenInclude(n => n.NoteTags)
                .Where(nh => nh.Note.NoteTags.Any(nt => nt.TagId == tagId))
                .ToListAsync();

            _context.NoteHistories.RemoveRange(historiesToDelete);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}