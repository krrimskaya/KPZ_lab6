using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NotesApp.Data;
using NotesApp.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NotesApp.Controllers
{
    [Authorize]
    public class NotesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public NotesController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var now = DateTime.Now;
            var notes = await _context.Notes
                .Include(n => n.NoteTags)
                .ThenInclude(nt => nt.Tag)
                .Where(n => !n.ReminderAt.HasValue || n.ReminderAt > now)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View("~/Views/Home/Index.cshtml", notes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,ReminderAt")] Note note)
        {
            if (ModelState.IsValid)
            {
                note.CreatedAt = DateTime.Now;
                _context.Add(note);
                await _context.SaveChangesAsync();
                
                await RecordHistory(note.Id, "Created", note.Title, note.Content);
                return RedirectToAction(nameof(Index));
            }

            var notes = await _context.Notes
                .Where(n => !n.ReminderAt.HasValue || n.ReminderAt > DateTime.Now)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View("~/Views/Home/Index.cshtml", notes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, string title, string content, DateTime? reminderAt)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(title))
            {
                ModelState.AddModelError("Title", "Заголовок обов'язковий");
                var notes = await _context.Notes
                    .Where(n => !n.ReminderAt.HasValue || n.ReminderAt > DateTime.Now)
                    .OrderByDescending(n => n.CreatedAt)
                    .ToListAsync();
                return View("~/Views/Home/Index.cshtml", notes);
            }

            await RecordHistory(note.Id, "Updated", note.Title, note.Content);

            note.Title = title;
            note.Content = content;
            note.ReminderAt = reminderAt;

            _context.Update(note);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            await RecordHistory(note.Id, "Deleted", note.Title, note.Content);

            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearReminder(int id)
        {
            var note = await _context.Notes.FindAsync(id);
            if (note == null)
            {
                return NotFound();
            }

            note.ReminderAt = null;
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetNoteTags(int noteId)
        {
            var tags = await _context.NoteTags
                .Where(nt => nt.NoteId == noteId)
                .Include(nt => nt.Tag)
                .Select(nt => new
                {
                    id = nt.Tag.Id,
                    name = nt.Tag.Name,
                    color = nt.Tag.Color
                })
                .ToListAsync();

            return Ok(tags);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddTagToNote(int noteId, int tagId)
        {
            var note = await _context.Notes
                .Include(n => n.NoteTags)
                .FirstOrDefaultAsync(n => n.Id == noteId);
            if (note == null)
            {
                return NotFound();
            }

            var tag = await _context.Tags.FindAsync(tagId);
            if (tag == null)
            {
                return NotFound();
            }

            if (!note.NoteTags.Any(nt => nt.TagId == tagId))
            {
                note.NoteTags.Add(new NoteTag { NoteId = noteId, TagId = tagId });
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTagFromNote(int noteId, int tagId)
        {
            var noteTag = await _context.NoteTags
                .FirstOrDefaultAsync(nt => nt.NoteId == noteId && nt.TagId == tagId);
            if (noteTag != null)
            {
                _context.NoteTags.Remove(noteTag);
                await _context.SaveChangesAsync();
            }

            return Ok();
        }

        private async Task RecordHistory(int noteId, string changeType, string title, string content = null)
        {
            var history = new NoteHistory
            {
                NoteId = noteId,
                Title = title,
                Content = content,
                ChangeType = changeType,
                ChangedBy = User.Identity.Name,
                ChangedAt = DateTime.Now
            };

            _context.NoteHistories.Add(history);
            await _context.SaveChangesAsync();
        }
    }
}